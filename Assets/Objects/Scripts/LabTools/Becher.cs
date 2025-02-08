using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Experimental.GraphView.Port;
using static UnityEngine.Rendering.DebugUI;

public class Becher : MonoBehaviour, Fillable, Pourable
{
    [SerializeField] private SubstancesMix substancesMix;
    [SerializeField] private float capacity;

    //private LiquidRenderer liquidRenderer;
    private MeshRenderer becherRenderer;
    //[SerializeField] private Material errorMaterial;
    //private bool errorMix;

    //private bool isMixed = false;

    private bool isFilterOn = false;
    private Filter filter = null;

    private Rigidbody becherRigidbody;
    private float becherMass;

    private ExperimentController experimentController;

    private LiquidRenderer liquidRenderer;
    //private StepSubstanceMix stepSubstanceMix;

    //private List<Substance> starterMix;
    //private List<Substance> finalMix;

    //public List<Substance> Contents => substancesMix;

    void Start()
    {
        if (capacity <= 0)
        {
            Debug.LogWarning("Capacity deve essere maggiore di 0. Impostazione modificata a 20ml.");
            capacity = 20f;
        }

        liquidRenderer = GetComponentInChildren<LiquidRenderer>();
        if (liquidRenderer == null)
        {
            Debug.Log("Liquid NOT found");
        }
        else
        {
            liquidRenderer.SetFillSize(GetCurrentVolume() / capacity);
        }
        becherRenderer = GetComponent<MeshRenderer>();

        becherRigidbody = GetComponent<Rigidbody>();
        becherMass = becherRigidbody.mass;

        RefreshTotalWeight();

        experimentController = FindFirstObjectByType<ExperimentController>();

        //if (stepSubstanceMix == null)
        //{
        //    experimentController = FindFirstObjectByType<ExperimentController>();
        //    stepSubstanceMix = experimentController.GetCurrentStep();
        //    finalMix = stepSubstanceMix.finalMix;
        //    experimentController.OnUpdateStep += UpdateStep;
        //    CheckMixAndSetMaterial(finalMix);
        //}

    }

    private void OnValidate()
    {
        if (capacity <= 0)
        {
            Debug.LogWarning("maxVolume deve essere maggiore di 0. Impostazione modificata a 20ml.");
            capacity = 20f;
        }
    }

    private float GetCurrentVolume()
    {
        return substancesMix.GetCurrentVolume();
    }

    public float GetRemainingVolume()
    {
        return capacity - GetCurrentVolume();
    }

    //public void AddSubstance(Substance substance)
    //{
    //    substancesMix.AddSubstance(substance);
    //    liquidRenderer.SetFillSize(GetCurrentVolume() / capacity);
    //}

    public void MixSubstances()
    {
        substancesMix.MixSubstances();
    }

    public void RefreshTotalWeight()
    {
        TryGetComponent<Rigidbody>(out var rb);
        float liquidWeight = substancesMix.GetLiquidWeight();  
        rb.mass = becherMass + liquidWeight;
    }

    public void Fill(SubstancesMix mix)
    {
        if(isFilterOn)
        {
            filter.FilterLiquid(mix.Substances);
        }

        substancesMix.AddSubstancesMix(mix);

        RefreshTotalWeight();

        liquidRenderer.SetFillSize(GetCurrentVolume() / capacity);

        substancesMix.Mixed = false;

        experimentController.TryAdvanceToNextStep(substancesMix);

        //CheckMixAndSetMaterial(finalMix);

        //if (HasMix(finalMix))
        //{
        //    stepSubstanceMix.Notify(this);
        //}
    }

    public void Pour(Fillable targetContainer, float amountToPour)
    {
        float totalAmount = GetCurrentVolume();
        float targetRemainingVolume = targetContainer.GetRemainingVolume();
        if (totalAmount <= 0 || amountToPour <= 0 || targetRemainingVolume <= 0) return;
        if (amountToPour > totalAmount) amountToPour = totalAmount;
        if (amountToPour > targetRemainingVolume) amountToPour = targetRemainingVolume;

        List<Substance> pouredSubstances = substancesMix.ExtractSubstances(amountToPour);
        SubstancesMix pouredMix = new SubstancesMix(pouredSubstances, substancesMix.Mixed, substancesMix.ExperimentStepReached);

        liquidRenderer.SetFillSize(GetCurrentVolume() / capacity);
        RefreshTotalWeight();

        targetContainer.Fill(pouredMix);

        experimentController.isLastStepStillReached(substancesMix);

        //CheckMixAndSetMaterial(finalMix);
    }

    public SubstancesMix PickUpVolume(float amountToExtract)
    {
        float totalAmount = GetCurrentVolume();
        SubstancesMix extractedMix = new SubstancesMix(new List<Substance>(), substancesMix.Mixed, substancesMix.ExperimentStepReached);
        if (totalAmount == 0 || amountToExtract <= 0) return extractedMix;
        if (amountToExtract > totalAmount) amountToExtract = totalAmount;

        List<Substance> extractedSubstances = substancesMix.ExtractSubstances(amountToExtract);
        extractedMix.Substances = extractedSubstances;

        liquidRenderer.SetFillSize(GetCurrentVolume() / capacity);
        RefreshTotalWeight();

        return extractedMix;
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
            Debug.Log("C'è già un filtro sul becher"); 
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
            Debug.Log("Non c'è nessun filtro"); //tecnicamente non deve mai comparirre sto messaggio
        }
    }

    public bool HasMixSimilarTo(SubstancesMix mix)
    {
        return substancesMix.IsSimilarTo(mix);
    }

    //private void CheckMixAndSetMaterial(List<Substance> mix)
    //{
    //    if (GetCurrentVolume() < 0.001 || ((CanStillBecomeMix(mix) || substancesMix.Count() < 2) && errorMix == false))
    //    {
    //        SetNormalMaterial();
    //    }
    //    else
    //    {
    //        SetErrorMaterial();
    //        errorMix = true;
    //    }
    //}

    //private void SetErrorMaterial()
    //{
    //    Material[] materials = becherRenderer.materials;
    //    if(materials.Length < 2)
    //    {
    //        System.Array.Resize(ref materials, materials.Length + 1);
    //        materials[materials.Length - 1] = errorMaterial;

    //        becherRenderer.materials = materials;
    //    }
    //}

    //private void SetNormalMaterial()
    //{
    //    Material[] materials = becherRenderer.materials;

    //    if (materials.Length > 0) 
    //    {
    //        becherRenderer.materials = new Material[] { materials[0] };
    //    }
    //}

    public bool CanStillBecome(SubstancesMix mix)
    {
        return substancesMix.CanBecome(mix);
    }

//private void UpdateStep()
//    {
//        stepSubstanceMix = experimentController.GetCurrentStep();
//        finalMix = stepSubstanceMix.finalMix;
//        CheckMixAndSetMaterial(finalMix);
//    }
}
