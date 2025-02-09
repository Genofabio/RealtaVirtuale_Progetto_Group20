using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExperimentStep
{
    [Header("Step Info")]
    [SerializeField] private string stepName;
    [SerializeField] private string description;

    [Header("Requisiti")]
    [SerializeField] private SubstanceMixture requiredMixture;

    [Header("Risultati")]
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
        if (resultingMixture.Substances.Count > 0)
        {
            return resultingMixture.Clone();
        }
        return requiredMixture.Clone();
    }

    public SubstanceMixture GetRequiredSubstanceMixture()
    {
        return requiredMixture.Clone();  // Restituisci una copia
    }

    public void ApplyStepEffect(SubstanceMixture availableMixture)
    {
        if (resultingMixture.Substances.Count > 0)
        {
            availableMixture.Substances = GetResultingSubstanceMixture().Substances;
            availableMixture.Mixed = GetResultingSubstanceMixture().Mixed;
        }
    }
}