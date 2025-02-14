using UnityEngine;

[System.Serializable]
public class ExperimentStep
{
    [Header("Step Info")]
    [SerializeField] private string stepName;
    [SerializeField] private string description;

    [Header("Requirement")]
    [SerializeField] private SubstanceMixture requiredMixture;

    [Header("Result")]
    [SerializeField] private SubstanceMixture resultingMixture;

    public ExperimentStep(string stepName, string description, SubstanceMixture requiredMixture, SubstanceMixture resultingMixture)
    {
        this.stepName = stepName;
        this.description = description;
        this.requiredMixture = requiredMixture;
        this.resultingMixture = resultingMixture; 
    }

    public string StepName
    {
        get { return stepName; }
        set { stepName = value; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    public SubstanceMixture GetResultingSubstanceMixture()
    {
        SubstanceMixture res;
        if (resultingMixture.Substances.Count > 0)
        {
            res = resultingMixture.Clone();
        } else
        {
            res = requiredMixture.Clone();
            res.Mixed = resultingMixture.Mixed;
            res.Dried = resultingMixture.Dried;
            res.Cooled = resultingMixture.Cooled;
            res.MixtureLiquidColor = resultingMixture.MixtureLiquidColor;
            res.MixtureSolidColor = resultingMixture.MixtureSolidColor;
        }
        return res;
    }

    public SubstanceMixture GetRequiredSubstanceMixture()
    {
        return requiredMixture.Clone();  // Restituisci una copia
    }

    public void ApplyStepEffect(SubstanceMixture availableMixture)
    {
        SubstanceMixture res = GetResultingSubstanceMixture();
        if (resultingMixture.Substances.Count > 0)
        {
            availableMixture.Substances = res.Substances;
        }
        availableMixture.Mixed = res.Mixed;
        availableMixture.Dried = res.Dried;
        availableMixture.Cooled = res.Cooled;
        if (res.MixtureLiquidColor != Color.clear)
        {
            availableMixture.MixtureLiquidColor = res.MixtureLiquidColor;
        }
        if (res.MixtureSolidColor != Color.clear)
        {
            availableMixture.MixtureSolidColor = res.MixtureSolidColor;
        }
    }
}