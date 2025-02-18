using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Becher : MonoBehaviour, Fillable, Pourable
{
    // Attributi serializzati
    [SerializeField] private List<Substance> initialSubstances = new List<Substance>(); //Nella build definitiva pu� essere tolto perch� i becher saranno inizialmente tutti vuoti
    [SerializeField] private Color initialLiquidColor; //Nella build definitiva pu� essere tolto perch� i becher saranno inizialmente tutti vuoti
    [SerializeField] private Color initialSolidColor; //Nella build definitiva pu� essere tolto perch� i becher saranno inizialmente tutti vuoti
    [SerializeField] private float maxCapacity;

    // Sostanze contenute
    [SerializeField] private SubstanceMixture containedMixture;

    // Gestione filtro
    private bool isFilterOn = false;
    private Filter filter = null;

    // Gestione fisica
    private Rigidbody becherRigidbody;
    private float emptyBecherMass;

    // Gestione rotazione versamento
    [SerializeField] Transform pivot; // Il secondo pivot, es. imboccatura della bottiglia
    //public float rotationSpeed = 45f; // Gradi al secondo

    //Gestione temporizzazione versamento
    private bool firstPouring = true;
    private bool isPouring = false;
    private float pourTimer = 0f;
    private float pourDuration = 0.05f;

    // Gestione avanzamento step esperimento
    private ExperimentManager experimentManager;

    // Visualizzazione grafica del liquido
    private LiquidRenderer liquidRenderer;

    // Visualizzazione grafica del solido 
    private SolidRenderer solidRenderer;

    private void Start()
    {
        experimentManager = FindFirstObjectByType<ExperimentManager>();

        containedMixture = new SubstanceMixture(initialSubstances, false, false, 0, false, 0, -1, initialLiquidColor, initialSolidColor); // Nella build definitiva la lista sar� inizialmente vuota

        liquidRenderer = GetComponentInChildren<LiquidRenderer>();
        if (liquidRenderer == null)
        {
            Debug.LogWarning("Liquid NOT found");
        }
        else
        {
            if (GetLiquidVolume() > 0.01)
            {
                liquidRenderer.SetFillSize(GetCurrentVolume() / maxCapacity);
                liquidRenderer.SetColor(initialLiquidColor);
            }
        }

        solidRenderer = GetComponentInChildren<SolidRenderer>();
        if (solidRenderer == null)
        {
            Debug.LogWarning("Solid NOT found");
        }
        else
        {
            solidRenderer.SetFillSize(GetSolidVolume() / maxCapacity);
            solidRenderer.SetColor(initialSolidColor);
        }

        becherRigidbody = GetComponent<Rigidbody>();
        emptyBecherMass = becherRigidbody.mass;
        RefreshTotalWeight();
    }

    private void OnValidate()
    {
        if (maxCapacity <= 0)
        {
            Debug.LogWarning("maxVolume deve essere maggiore di 0. Impostazione modificata a 20ml.");
            maxCapacity = 20f;
        }
    }
    void Update()
    {
        if (becherRigidbody.linearVelocity.magnitude > 0.2) Debug.Log(becherRigidbody.linearVelocity.magnitude);

        if (isPouring)
        {
            pourTimer -= Time.deltaTime;
            if (pourTimer <= 0f)
            {
                isPouring = false;
                firstPouring = true;
                TryGetComponent<Grabbable>(out var grab);
                grab.StopRotating();
            }
        }
    }

    public void RefreshTotalWeight()
    {
        float liquidWeight = containedMixture.GetLiquidWeight();
        float solidWeight = containedMixture.GetSolidWeight();
        becherRigidbody.mass = emptyBecherMass + liquidWeight + solidWeight * 100;
    }

    // Implementazione interfaccia Fillable
    public void Fill(SubstanceMixture mix)
    {
        if(isFilterOn)
        {
            mix = filter.FilterLiquid(mix);
        }

        if (containedMixture.ExperimentStepReached < mix.ExperimentStepReached)
        {
            experimentManager.SetMixtureStepAndUpdateCount(containedMixture, mix.ExperimentStepReached);
        }

        containedMixture.AddSubstanceMixture(mix);

        if(containedMixture != null)
        {
            experimentManager.CheckAndModifyStep(containedMixture);
            UpdateSubstanceRenderFill();
            SetRenderColors();
        }

        RefreshTotalWeight();
    }

    public void SetRenderColors()
    {
        liquidRenderer.SetColor(containedMixture.MixtureLiquidColor);
        solidRenderer.SetColor(containedMixture.MixtureSolidColor);
    }

    public void StirContents()
    {
        containedMixture.StirSubstances();
        experimentManager.CheckAndModifyStep(containedMixture);
        liquidRenderer.SetColor(containedMixture.MixtureLiquidColor);
    }

    public float GetRemainingVolume()
    {
        return maxCapacity - GetCurrentVolume();
    }

    // Implementazione interfaccia Pourable
    public void Pour(Fillable targetContainer, float amountToPour)
    {
        if (targetContainer.CanContainLiquid() == false)
        {
            foreach (var sub in containedMixture.Substances)
            {
                if (!sub.IsSolid)
                {
                    Debug.LogWarning("Il contenitore di destinazione non può contenere liquidi");
                    return;
                }
            }
        }

        float totalAmount = GetCurrentVolume();
        float targetRemainingVolume = targetContainer.GetRemainingVolume();
        if (totalAmount <= 0 || amountToPour <= 0 || targetRemainingVolume <= 0) return;
        if (amountToPour > totalAmount) amountToPour = totalAmount;
        if (amountToPour > targetRemainingVolume) amountToPour = targetRemainingVolume;

        SubstanceMixture pouredMix = new SubstanceMixture(new List<Substance>(), containedMixture.Mixed, containedMixture.Dried, containedMixture.DryingTime, containedMixture.Cooled, containedMixture.CoolingTime, containedMixture.ExperimentStepReached, containedMixture.MixtureLiquidColor, containedMixture.MixtureSolidColor);
        pouredMix.Substances = containedMixture.ExtractSubstances(amountToPour);
        UpdateSubstanceRenderFill();
        RefreshTotalWeight();

        targetContainer.Fill(pouredMix);

        experimentManager.CheckAndModifyStep(containedMixture);

        // 🔥 Imposta isPouring e avvia il timer
        if(firstPouring == true)
        {
            firstPouring = false;
            TryGetComponent<Grabbable>(out var grab);
            grab.StartRotating(pivot, CalculateInitalRotation());
        }
        isPouring = true;
        pourTimer = pourDuration;
    }

    public float CalculateInitalRotation()
    {
        //calcola la percentuale di volume pieno sul volume totale
        float volumePercentage = GetCurrentVolume() / maxCapacity;

        //normalizza la percentuale tra -90 e -30
        return Mathf.Lerp(-90, -30, volumePercentage);
    }

    public void UpdateSubstanceRenderFill()
    {
        if (GetLiquidVolume() > 0.01)
        {
            liquidRenderer.SetFillSize(GetCurrentVolume() / maxCapacity);
        }
        solidRenderer.SetFillSize(GetSolidVolume() / maxCapacity);
    }

    public SubstanceMixture PickUpVolume(float amountToExtract)
    {
        float totalAmount = GetCurrentVolume();
        SubstanceMixture extractedMix = new SubstanceMixture(new List<Substance>(), containedMixture.Mixed, containedMixture.Dried, containedMixture.DryingTime, containedMixture.Cooled, containedMixture.CoolingTime, containedMixture.ExperimentStepReached, containedMixture.MixtureLiquidColor, containedMixture.MixtureSolidColor);
        if (totalAmount == 0 || amountToExtract <= 0) return extractedMix;
        if (amountToExtract > totalAmount) amountToExtract = totalAmount;

        List<Substance> extractedSubstances = containedMixture.ExtractSubstances(amountToExtract);
        extractedMix.Substances = extractedSubstances;

        UpdateSubstanceRenderFill();
        RefreshTotalWeight();

        return extractedMix;
    }

    public float GetCurrentVolume()
    {
        return containedMixture.GetCurrentVolume();
    }

    public float GetLiquidVolume()
    {
        return containedMixture.GetLiquidVolume();
    }

    public float GetSolidVolume()
    {
        return containedMixture.GetSolidVolume();
    }

    public void SetFilterOn(Filter filter)
    {
        if(isFilterOn == false) { 
            this.filter = filter;
            isFilterOn = true;
            Debug.Log("Filtro applicato");
        } else
        {
            Debug.Log(isFilterOn);
            Debug.Log("C'� gi� un filtro sul becher"); 
        }
    }

    public void SetFilterOff()
    {
        if(isFilterOn == true /*&& filter != null*/)
        {
            filter = null;
            isFilterOn = false;

        } else
        {
            Debug.Log("Non c'� nessun filtro"); //tecnicamente non deve mai comparirre sto messaggio
        }
    }

    public bool CanContainLiquid()
    {
        return true;
    }

    public SubstanceMixture GetContainedSubstanceMixture()
    {
        return containedMixture;
    }

    private void OnDestroy()
    {
        experimentManager.SetMixtureStepAndUpdateCount(containedMixture, -1);
        //liquidRenderer.SetFillSize(0);
        //liquidRenderer.Set
        //(Color.clear);
        //solidRenderer.SetFillSize(0);
        //solidRenderer.SetColor(Color.clear);
    }
}
