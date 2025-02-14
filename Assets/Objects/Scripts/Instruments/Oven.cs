using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Oven : MonoBehaviour, Openable
{
    [SerializeField] private Transform door;
    [SerializeField] private List<GameObject> contentObjects;
    //private Collider doorCollider;
    [SerializeField] private List<AudioClip> audioList;

    private ExperimentManager experimentManager;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    [SerializeField] private float openSpeed = 1f;

    public bool IsOpen { get; set;  } = false;
    public bool IsEmpty { get; set; } = true;
    public bool IsMoving { get; set; } = false;

    private Coroutine cookRoutine;

    private void Start()
    {
        if (door == null)
        {
            Debug.LogError("La porta non è stata trovata nel forno!");
            return;
        }

        contentObjects = new List<GameObject>();

        experimentManager = FindFirstObjectByType<ExperimentManager>();

        //doorCollider = door.GetComponent<Collider>(); // Trova il Rigidbody della porta
        //if (doorCollider == null)
        //{
        //    Debug.LogError("La porta non ha un Rigidbody!");
        //    return;
        //}

        // Memorizza la rotazione iniziale (chiusa)
        closedRotation = door.localRotation;

        // Definisce la rotazione aperta: -90� sull'asse Y
        openRotation = Quaternion.Euler(closedRotation.eulerAngles.x, closedRotation.eulerAngles.y, closedRotation.eulerAngles.z + 90);
    }

public void ToggleDoor()
{
    if (IsMoving)
    {
        return;
    }
    if (IsOpen)
    {
        StartCoroutine(CloseDoorCoroutine());
        GetComponent<AudioSource>().clip = audioList[0]; // Suono chiusura
        GetComponent<AudioSource>().Play();
    }
    else
    {
        StartCoroutine(OpenDoorCoroutine());
        GetComponent<AudioSource>().clip = audioList[1]; // Suono apertura
        GetComponent<AudioSource>().Play();
    }
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

        float duration = 1f / openSpeed;
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

        float duration = 1f / openSpeed;
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
        while (!IsEmpty)
        {
            foreach (GameObject obj in contentObjects)
            {
                if (obj != null)
                {
                    Becher becher = obj.GetComponent<Becher>();
                    SubstanceMixture fillableSubstanceMix = becher.GetContainedSubstanceMixture();
                    if (fillableSubstanceMix != null && !fillableSubstanceMix.Dried)
                    {
                        fillableSubstanceMix.Dry(Time.deltaTime);
                        experimentManager.CheckAndModifyStep(fillableSubstanceMix.GetReference());
                    }
                }
            }

            yield return null; // Aspetta un frame prima di continuare
        }

        cookRoutine = null;
    }

    public void InsertIntoOven(GameObject obj)
    {
        if (obj == null) return;

        if (!contentObjects.Contains(obj))
        {
            contentObjects.Add(obj);
        }
    }

    public void RemoveFromOven(GameObject obj)
    {
        if (obj == null) return;

        if (contentObjects.Contains(obj))
        {
            Fillable fillable = obj.GetComponent<Fillable>();
            SubstanceMixture fillableSubstanceMix = fillable.GetContainedSubstanceMixture();
            fillableSubstanceMix.DryingTime = 0;

            contentObjects.Remove(obj);
        }
    }

}
