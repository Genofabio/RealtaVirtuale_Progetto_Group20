using UnityEngine;
using System.Collections;

public class Oven : MonoBehaviour
{
    [SerializeField] private Transform door;
    //private Collider doorCollider; 
    private Quaternion closedRotation;
    private Quaternion openRotation;
    public bool IsOpen { get; set;  } = false;
    public bool IsEmpty { get; set; } = true;
    public bool IsMoving { get; set; } = false;

    private Coroutine cookRoutine;

    private void Start()
    {
        door = transform.Find("Porta");
        if (door == null)
        {
            Debug.LogError("La porta non è stata trovata nel forno!");
            return;
        }

        //doorCollider = door.GetComponent<Collider>(); // Trova il Rigidbody della porta
        //if (doorCollider == null)
        //{
        //    Debug.LogError("La porta non ha un Rigidbody!");
        //    return;
        //}

        // Memorizza la rotazione iniziale (chiusa)
        closedRotation = door.localRotation;

        // Definisce la rotazione aperta: -90° sull'asse Y
        openRotation = Quaternion.Euler(closedRotation.eulerAngles.x, closedRotation.eulerAngles.y + 90, closedRotation.eulerAngles.z);
    }

    public void ToggleDoor()
    {
        if(IsMoving)
        {
            return;
        }
        if (IsOpen)
            StartCoroutine(CloseDoorCoroutine());
        else
            StartCoroutine(OpenDoorCoroutine());
    }

    private IEnumerator OpenDoorCoroutine()
    {
        IsMoving = true;

        //doorCollider.enabled = false;

        IsOpen = true;

        if (cookRoutine != null)
        {
            StopCoroutine(cookRoutine);
            cookRoutine = null;
            Debug.Log("Cottura interrotta!"); // Debug per confermare l'interruzione
        }

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            door.localRotation = Quaternion.Lerp(closedRotation, openRotation, t);
            yield return null;
        }

        door.localRotation = openRotation;
        //doorCollider.enabled = true;
        IsMoving = false;
    }

    private IEnumerator CloseDoorCoroutine()
    {
        IsMoving = true;
        //doorCollider.enabled = false;

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            door.localRotation = Quaternion.Lerp(openRotation, closedRotation, t);
            yield return null;
        }

        door.localRotation = closedRotation;

        IsOpen = false;
        //doorCollider.enabled = true;
        IsMoving = false;

        if(!IsEmpty)
        {
            if (cookRoutine == null)
            {
                cookRoutine = StartCoroutine(CookCoroutine());
            }
        }
    }

    private IEnumerator CookCoroutine()
    {
        Debug.Log("Inizio cottura");
        yield return new WaitForSeconds(3f);
        Debug.Log("Cotto!");

        cookRoutine = null;
    }

}
