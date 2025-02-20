using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Audio;

public class Oven : MonoBehaviour, Openable
{
    [SerializeField] private Transform door;
    [SerializeField] private List<GameObject> contentObjects;
    [SerializeField] private List<AudioClip> audioList;

    private ExperimentManager experimentManager;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    [SerializeField] private float openSpeed = 1f;

    [SerializeField] private int temperature = 100;
    private const int minTemperature = 30;
    private const int maxTemperature = 250;

    [SerializeField] private GameObject panelSettingPower;
    [SerializeField] private TextMeshProUGUI powerText;

    [SerializeField] private GameObject panelDrying;
    [SerializeField] private GameObject panelStopped;

    private AudioSource audioSource;

    public bool IsOpen { get; set;  } = false;
    public bool IsEmpty { get; set; } = true;
    public bool IsMoving { get; set; } = false;
    public bool IsCooking { get; private set; } = false;

    private Coroutine cookRoutine;

    [SerializeField] private Assistant assistant;

    private SubstanceMixture cookingMixtureContained = null;

    public SubstanceMixture CookingMixtureContained
    {
        get { return cookingMixtureContained; }
        set { cookingMixtureContained = value; }
    }

    private void Start()
    {
        if (door == null)
        {
            Debug.LogError("La porta non è stata trovata nel forno!");
            return;
        }

        audioSource = GetComponent<AudioSource>();

        contentObjects = new List<GameObject>();
        experimentManager = FindFirstObjectByType<ExperimentManager>();


        closedRotation = door.localRotation;
        openRotation = Quaternion.Euler(closedRotation.eulerAngles.x, closedRotation.eulerAngles.y, closedRotation.eulerAngles.z + 90);

        ShowPowerSettingPanel();
        UpdatePowerText();
    }

    private void Update()
    {
        if (cookingMixtureContained != null && IsCooking && !cookingMixtureContained.Dried)
        {
            assistant.notifyStateProcessingTime(cookingMixtureContained.DryingTime);
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
            audioSource.clip = audioList[0]; // Suono chiusura
            audioSource.Play();
        }
        else
        {
            StartCoroutine(OpenDoorCoroutine());
            audioSource.clip = audioList[1]; // Suono apertura
            audioSource.Play();
        }
    }

    public bool IsOpened()
    {
        return IsOpen;
    }

    private IEnumerator OpenDoorCoroutine()
    {
        IsMoving = true;
        IsOpen = true;

        StopCooking();

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
        IsMoving = false;
    }

    private IEnumerator CloseDoorCoroutine()
    {
        IsMoving = true;

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
        IsMoving = false;
    }

    public void IncreaseTemperature()
    {
        temperature = Mathf.Min(temperature + 10, maxTemperature);
        UpdatePowerText();
    }

    public void DecreaseTemperature()
    {
        temperature = Mathf.Max(temperature - 10, minTemperature);
        UpdatePowerText();
    }

    private void UpdatePowerText()
    {
        powerText.text = "" + temperature + "°C";
    }

    public void ToggleCooking()
    {
        if (IsCooking)
        {
            StopCooking();
        }
        else if (!IsOpen && !IsEmpty)
        {
            StartCooking();
        }
    }

    private void StartCooking()
    {
        if (cookRoutine == null)
        {
            cookRoutine = StartCoroutine(CookCoroutine());
            IsCooking = true;

            ShowCookingPanel();
        }
    }

    private void StopCooking()
    {
        if (cookRoutine != null)
        {
            StopCoroutine(cookRoutine);
            cookRoutine = null;
            IsCooking = false;
            GetComponent<AudioSource>().Stop();

            StartCoroutine(ShowStoppedPanel());
        }

        audioSource.loop = false;
    }

    private IEnumerator CookCoroutine()
    {
        audioSource.clip = audioList[2]; // Suono di funzionamento
        audioSource.loop = true; // Imposta il suono in loop finché il forno è acceso
        audioSource.Play();

        while (!IsEmpty)
        {
            foreach (GameObject obj in contentObjects)
            {
                if (obj != null)
                {
                    Fillable fillable = obj.GetComponent<Fillable>();
                    SubstanceMixture fillableSubstanceMix = fillable.GetContainedSubstanceMixture();
                    if (fillableSubstanceMix != null && !fillableSubstanceMix.Dried)
                    {
                        fillableSubstanceMix.Dry(Time.deltaTime, temperature);
                        experimentManager.CheckAndModifyStep(fillableSubstanceMix.GetReference());
                    }
                }
            }

            yield return null; // Aspetta un frame prima di continuare
        }

        // Ferma il suono quando il forno è vuoto
        audioSource.Stop();
        IsCooking = false;
        cookRoutine = null;
        audioSource.loop = false;

        StartCoroutine(ShowStoppedPanel());
    }

    private IEnumerator ShowStoppedPanel()
    {
        panelStopped.SetActive(true);
        panelDrying.SetActive(false);
        panelSettingPower.SetActive(false);

        yield return new WaitForSeconds(1f);

        panelStopped.SetActive(false);
        ShowPowerSettingPanel();
    }

    private void ShowCookingPanel()
    {
        panelDrying.SetActive(true);
        panelStopped.SetActive(false);
        panelSettingPower.SetActive(false);
    }

    private void ShowPowerSettingPanel()
    {
        panelSettingPower.SetActive(true);
        panelDrying.SetActive(false);
        panelStopped.SetActive(false);
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
            if (fillableSubstanceMix != null)
            {
                fillableSubstanceMix.DryingTime = 0;
                fillableSubstanceMix.DryingTemperature = 0;
            }
            contentObjects.Remove(obj);
        }
    }

}
