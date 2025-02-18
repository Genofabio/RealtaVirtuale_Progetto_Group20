using System.Collections.Generic;
using UnityEngine;

public class SubstanceVial : MonoBehaviour, Pourable
{
    [SerializeField] private Substance substance;
    [SerializeField] private Color substanceColor;

    [SerializeField] private float maxVolume;

    // Gestione rotazione versamento
    [SerializeField] Transform pivot; // Il secondo pivot, es. imboccatura della bottiglia
    //public float rotationSpeed = 45f; // Gradi al secondo

    //Gestione temporizzazione versamento
    private bool firstPouring = true;
    private bool isPouring = false;
    private float pourTimer = 0f;
    private float pourDuration = 0.05f;

    private LiquidRenderer liquid;
    private SolidRenderer solid;

    private void Start()    
    {
        liquid = GetComponentInChildren<LiquidRenderer>();
        if (liquid == null)
        {
            Debug.Log("Liquid NOT found");
        }
        else
        {
            if(!substance.IsSolid)
            {
                liquid.SetFillSize(GetCurrentVolume() / maxVolume);
            } else
            {
                liquid.SetFillSize(0);
            }
            liquid.SetColor(substanceColor);
        }

        solid = GetComponentInChildren<SolidRenderer>();
        if (solid == null)
        {
            Debug.Log("Liquid NOT found");
        }
        else
        {
            if (substance.IsSolid)
            {
                solid.SetFillSize(GetCurrentVolume() / maxVolume);
            }
            else
            {
                solid.SetFillSize(0);
            }
            solid.SetColor(substanceColor);
        }
    }

    private void OnValidate()
    {
        if (maxVolume <= 0)
        {
            Debug.LogWarning("maxVolume deve essere maggiore di 0. Fialetta piena di 10ml.");
            maxVolume = 10f;
        }
    }

    void Update()
    {
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

    public void Pour(Fillable targetContainer, float amountToPour)
    {
        float totalAmount = GetCurrentVolume();
        float targetRemainingVolume = targetContainer.GetRemainingVolume();

        if (totalAmount <= 0 || amountToPour <= 0 || targetRemainingVolume <= 0) return;
        if (amountToPour > totalAmount) amountToPour = totalAmount;
        if (amountToPour > targetRemainingVolume) amountToPour = targetRemainingVolume;

        List<Substance> pouredSubstance = new List<Substance> { new Substance(substance.SubstanceName, amountToPour, substance.IsSolid) };
        substance.Quantity -= amountToPour;
        SubstanceMixture pouredMix;
        if (substance.IsSolid)
        {
            pouredMix = new SubstanceMixture(pouredSubstance, false, false, 0, false, 0, -1, Color.clear, substanceColor);
        }
        else
        {
            pouredMix = new SubstanceMixture(pouredSubstance, false, false, 0, false, 0, -1, substanceColor, Color.clear);
        }

        if (!substance.IsSolid)
        {
            liquid.SetFillSize(GetCurrentVolume() / maxVolume);
        } 
        else
        {
            solid.SetFillSize(GetCurrentVolume() / maxVolume);
        }
 

        targetContainer.Fill(pouredMix);

        // Imposta isPouring e avvia il timer
        if (firstPouring == true)
        {
            firstPouring = false;
            TryGetComponent<Grabbable>(out var grab);
            grab.StartRotating(pivot, CalculateInitalRotation());
        }
        isPouring = true;
        pourTimer = pourDuration;
    }

    public string SubstanceContained()
    {
        return substance.SubstanceName;
    }

    public float CalculateInitalRotation()
    {
        //calcola la percentuale di volume pieno sul volume totale
        float volumePercentage = GetCurrentVolume() / maxVolume;

        //normalizza la percentuale tra -90 e -30
        return Mathf.Lerp(-90, -30, volumePercentage);
    }

    public float GetCurrentVolume()
    {
        return substance.Quantity;
    }

    public SubstanceMixture PickUpVolume(float amountToExtract)
    {
        float totalAmount = GetCurrentVolume();
        SubstanceMixture extractedMix;
        if (substance.IsSolid)
        {
            extractedMix = new SubstanceMixture(new List<Substance>(), false, false, 0, false, 0, -1, Color.clear, substanceColor);
        }
        else
        {
            extractedMix = new SubstanceMixture(new List<Substance>(), false, false, 0, false, 0, -1, substanceColor, Color.clear);
        }
        if (totalAmount == 0 || amountToExtract <= 0) return extractedMix;
        if (amountToExtract > totalAmount) amountToExtract = totalAmount;

        List<Substance> extractedSubstance = new List<Substance> { new Substance(substance.SubstanceName, amountToExtract, substance.IsSolid) };
        substance.Quantity -= amountToExtract;

        extractedMix.Substances = extractedSubstance;

        if (!substance.IsSolid)
        {
            liquid.SetFillSize(GetCurrentVolume() / maxVolume);
        }
        else
        {
            solid.SetFillSize(GetCurrentVolume() / maxVolume);
        }

        return extractedMix;
    }

}
