using System.Collections.Generic;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    [SerializeField] private List<SubstanceMixture> steps;
    private int[] numMixturePerStep;

    public List<SubstanceMixture> Steps => steps;

    private void Start()
    {
        numMixturePerStep = new int[steps.Count];
    }

    public bool isLastStepStillReached(SubstanceMixture mix)
    {
        if(mix.ExperimentStepReached >= 0)
        {
            SubstanceMixture targetMix = steps[mix.ExperimentStepReached];
            if (!mix.HasSameSubstancePercentage(targetMix))
            {
                return false;
            }
        }
        return true;
    }

    public void CheckAndModifyStep(SubstanceMixture mix)
    {
        int currentStepReached = mix.ExperimentStepReached;

        if (currentStepReached >= 0 && !isLastStepStillReached(mix))
        {
            Debug.Log("Vuoto");
            HandleStepFailure(mix);
            return;
        }

        SubstanceMixture targetMix = steps[currentStepReached + 1];

        if (mix.CanBecome(targetMix))
        {
            if (mix.IsSimilarTo(targetMix))
            {
                Debug.Log("Avanzamento");
                AdvanceToNextStep(mix);
            }
        }
        else
        {
            if(currentStepReached >= 0)
            {
                Debug.Log("Sbagliato");
                HandleStepFailure(mix);
            }
        }
    }

    public void AdvanceToNextStep(SubstanceMixture mix)
    {
        SetMixtureStepAndUpdateCount(mix, mix.ExperimentStepReached + 1);
    }

    public void SetMixtureStepAndUpdateCount(SubstanceMixture mix, int nextStep)
    {
        int currentStep = mix.ExperimentStepReached;
        if (currentStep >= 0)
        {
            numMixturePerStep[currentStep] -= 1;
        }

        Debug.Log("Raggiunto step: " + nextStep);

        numMixturePerStep[nextStep] += 1;
        mix.ExperimentStepReached = nextStep;
    }

    public void HandleStepFailure(SubstanceMixture mix)
    {
        Debug.Log("Chiamata HandleStepFailure");
        int currentStep = mix.ExperimentStepReached;
        if (mix.ExperimentStepReached >= 0)
        {
            numMixturePerStep[currentStep] -= 1;
            mix.ExperimentStepReached = - 1;
        }
    }
}
