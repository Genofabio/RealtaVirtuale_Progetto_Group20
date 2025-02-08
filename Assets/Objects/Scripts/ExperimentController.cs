using System.Collections.Generic;
using UnityEngine;
using System;

public class ExperimentController : MonoBehaviour
{

    [SerializeField] private List<SubstancesMix> steps;
    [SerializeField] private List<int> numMixPerStep;

    public List<SubstancesMix> Steps => steps;

    private void Start()
    {
        numMixPerStep = new List<int>(new int[steps.Count]);
    }

    public bool isLastStepStillReached(SubstancesMix mix)
    {
        if(mix.ExperimentStepReached >= 0)
        {
            SubstancesMix targetMix = steps[mix.ExperimentStepReached];
            if (!mix.HasSameSubstancePercentage(targetMix))
            {
                return false;
            }
        }
        return true;
    }

    public void TryAdvanceToNextStep(SubstancesMix mix)
    {
        int currentStepReached = mix.ExperimentStepReached;

        if (currentStepReached >= 0 && !isLastStepStillReached(mix))
        {
            HandleStepFailure(mix);
            return;
        }

        SubstancesMix targetMix = steps[currentStepReached + 1];

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

    public void AdvanceToNextStep(SubstancesMix mix)
    {
        int mixStepReached = mix.ExperimentStepReached;
        if (mixStepReached >= 0)
        {
            numMixPerStep[mixStepReached] -= 1;
        }
        mixStepReached = mixStepReached + 1;

        Debug.Log("Raggiunto step: " +  mixStepReached);

        numMixPerStep[mixStepReached] += 1;
        mix.ExperimentStepReached = mixStepReached;
    }

    public void HandleStepFailure(SubstancesMix mix)
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
                        numMixPerStep[mix.ExperimentStepReached] += 1;
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
