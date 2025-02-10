using System.Collections.Generic;
using UnityEngine;

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

    // Gestione avanzamento step esperimento
    private ExperimentManager experimentManager;

    // Visualizzazione grafica del liquido
    private LiquidRenderer liquidRenderer;

    // Visualizzazione grafica del solido 
    private SolidRenderer solidRenderer;

    private void Start()
    {
        experimentManager = FindFirstObjectByType<ExperimentManager>();

        containedMixture = new SubstanceMixture(initialSubstances, false, -1, initialLiquidColor, initialSolidColor); // Nella build definitiva la lista sar� inizialmente vuota

        liquidRenderer = GetComponentInChildren<LiquidRenderer>();
        if (liquidRenderer == null)
        {
            Debug.LogWarning("Liquid NOT found");
        }
        else
        {
            liquidRenderer.SetFillSize(GetCurrentVolume() / maxCapacity);
            liquidRenderer.SetColor(initialLiquidColor);
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

    public void RefreshTotalWeight()
    {
        float liquidWeight = containedMixture.GetLiquidWeight();
        float solidWeight = containedMixture.GetSolidWeight();
        becherRigidbody.mass = emptyBecherMass + liquidWeight + solidWeight;
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

        experimentManager.CheckAndModifyStep(containedMixture);

        liquidRenderer.SetFillSize(GetCurrentVolume() / maxCapacity);
        solidRenderer.SetFillSize(GetSolidVolume() / maxCapacity);
        liquidRenderer.SetColor(containedMixture.MixtureLiquidColor);
        solidRenderer.SetColor(containedMixture.MixtureSolidColor);
        RefreshTotalWeight();
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
        float totalAmount = GetCurrentVolume();
        float targetRemainingVolume = targetContainer.GetRemainingVolume();
        if (totalAmount <= 0 || amountToPour <= 0 || targetRemainingVolume <= 0) return;
        if (amountToPour > totalAmount) amountToPour = totalAmount;
        if (amountToPour > targetRemainingVolume) amountToPour = targetRemainingVolume;

        SubstanceMixture pouredMix = new SubstanceMixture(new List<Substance>(), containedMixture.Mixed, containedMixture.ExperimentStepReached, containedMixture.MixtureLiquidColor, containedMixture.MixtureSolidColor);
        pouredMix.Substances = containedMixture.ExtractSubstances(amountToPour);

        liquidRenderer.SetFillSize(GetCurrentVolume() / maxCapacity);
        solidRenderer.SetFillSize(GetSolidVolume() / maxCapacity);
        RefreshTotalWeight();

        targetContainer.Fill(pouredMix);

        experimentManager.CheckAndModifyStep(containedMixture);
    }

    public SubstanceMixture PickUpVolume(float amountToExtract)
    {
        float totalAmount = GetCurrentVolume();
        SubstanceMixture extractedMix = new SubstanceMixture(new List<Substance>(), containedMixture.Mixed, containedMixture.ExperimentStepReached, containedMixture.MixtureLiquidColor, containedMixture.MixtureSolidColor);
        if (totalAmount == 0 || amountToExtract <= 0) return extractedMix;
        if (amountToExtract > totalAmount) amountToExtract = totalAmount;

        List<Substance> extractedSubstances = containedMixture.ExtractSubstances(amountToExtract);
        extractedMix.Substances = extractedSubstances;

        liquidRenderer.SetFillSize(GetCurrentVolume() / maxCapacity);
        solidRenderer.SetFillSize(GetSolidVolume() / maxCapacity);
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

}
