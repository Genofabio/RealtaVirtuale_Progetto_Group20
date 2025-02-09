using System.Collections.Generic;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    [SerializeField] private List<SubstanceMixture> steps;
    [SerializeField] private int[] numMixturePerStep;

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

    public void TryAdvanceToNextStep(SubstanceMixture mix)
    {
        int currentStepReached = mix.ExperimentStepReached;

        if (currentStepReached >= 0 && !isLastStepStillReached(mix))
        {
            HandleStepFailure(mix);
            return;
        }

        SubstanceMixture targetMix = steps[currentStepReached + 1];

        if (mix.CanBecome(targetMix))
        {
            if (mix.IsSimilarTo(targetMix))
            {
                AdvanceToNextStep(mix);
            }
        }
        else
        {
            HandleStepFailure(mix);
        }
    }

    public void AdvanceToNextStep(SubstanceMixture mix)
    {
        int mixStepReached = mix.ExperimentStepReached;
        if (mixStepReached >= 0)
        {
            numMixturePerStep[mixStepReached] -= 1;
        }
        mixStepReached = mixStepReached + 1;

        Debug.Log("Raggiunto step: " +  mixStepReached);

        numMixturePerStep[mixStepReached] += 1;
        mix.ExperimentStepReached = mixStepReached;
    }

    public void HandleStepFailure(SubstanceMixture mix)
    {
        Debug.Log("Chiamata HandleStepFailure");
        if(mix.ExperimentStepReached >= 0)
        {
            for (int i = mix.ExperimentStepReached; i >= 0; i--)
            {
                if(mix.CanBecome(steps[i]))
                {
                    mix.ExperimentStepReached = mix.ExperimentStepReached - 1;
                    if (mix.ExperimentStepReached > 0)
                    {
                        numMixturePerStep[mix.ExperimentStepReached] += 1;
                    }
                    return;
                } else
                {
                }
            }
            mix.ExperimentStepReached = -1;
        }
    }
}
