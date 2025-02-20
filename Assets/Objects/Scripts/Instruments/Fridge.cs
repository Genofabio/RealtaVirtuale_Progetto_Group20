using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Fridge : MonoBehaviour
{
    [SerializeField] private Transform door;
    [SerializeField] private List<GameObject> contentObjects;
    //private Collider doorCollider; 
    [SerializeField] private List<AudioClip> audioList;

    private ExperimentManager experimentManager;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    public bool IsOpen { get; set; } = false;
    public bool IsEmpty { get; set; } = true;
    public bool IsMoving { get; set; } = false;

    private Coroutine freezeRoutine;

    private bool isPaused = false;
    private bool wasCooling = false;

    [SerializeField] private Assistant assistant;

    private SubstanceMixture coolingMixtureContained = null;

    public SubstanceMixture CoolingMixtureContained
    {
        get { return coolingMixtureContained; }
        set { coolingMixtureContained = value; }
    }

    private void Start()
    {
        door = transform.Find("Porta");
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

        openRotation = Quaternion.Euler(closedRotation.eulerAngles.x, closedRotation.eulerAngles.y + 90, closedRotation.eulerAngles.z);
    }

    private void Update()
    {
        if(coolingMixtureContained != null && !IsOpen && !coolingMixtureContained.Cooled)
        {
            Debug.Log("Tempo di raffreddamento: " + coolingMixtureContained.CoolingTime);
            assistant.notifyStateProcessingTime(coolingMixtureContained.CoolingTime);
        }
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

    public void TogglePause()
    {
        isPaused = !isPaused; // Inverti lo stato della pausa

        if (isPaused)
        {
            if(freezeRoutine != null)
            {
                wasCooling = true;
                StopCoroutine(freezeRoutine);
            }
        }
        else
        {
            if (wasCooling)
            {
                wasCooling = false;
                freezeRoutine = StartCoroutine(FreezeCoroutine());
            }
        }
    }

    public bool IsOpened()
    {
        return IsOpen;
    }

    private IEnumerator OpenDoorCoroutine()
    {
        IsMoving = true;

        //doorCollider.enabled = false;

        IsOpen = true;

        if (freezeRoutine != null)
        {
            StopCoroutine(freezeRoutine);
            freezeRoutine = null;
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

        if (!IsEmpty)
        {
            if (freezeRoutine == null)
            {
                freezeRoutine = StartCoroutine(FreezeCoroutine());
            }
        }
    }

    private IEnumerator FreezeCoroutine()
    {
        while (!IsEmpty)
        {
            foreach (GameObject obj in contentObjects)
            {
                if (obj != null)
                {
                    Becher becher = obj.GetComponent<Becher>();
                    SubstanceMixture fillableSubstanceMix = becher.GetContainedSubstanceMixture();
                    if (fillableSubstanceMix != null && !fillableSubstanceMix.Cooled)
                    {
                        fillableSubstanceMix.CoolDown(Time.deltaTime);
                        experimentManager.CheckAndModifyStep(fillableSubstanceMix.GetReference());
                        becher.UpdateSubstanceRenderFill();
                        becher.SetRenderColors();
                    }
                }
            }

            yield return null; // Aspetta un frame prima di continuare
        }

        freezeRoutine = null;
    }

    public void InsertIntoFridge(GameObject obj)
    {
        if (obj == null) return;

        if (!contentObjects.Contains(obj))
        {
            contentObjects.Add(obj);
        }
    }

    public void RemoveFromFridge(GameObject obj)
    {
        if (obj == null) return;

        if (contentObjects.Contains(obj))
        {
            Fillable fillable = obj.GetComponent<Fillable>();
            SubstanceMixture fillableSubstanceMix = fillable.GetContainedSubstanceMixture();
            fillableSubstanceMix.CoolingTime = 0;

            contentObjects.Remove(obj);
        }
    }
}
