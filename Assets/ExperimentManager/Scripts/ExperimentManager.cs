using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ExperimentManager : MonoBehaviour
{
    [SerializeField] private List<ExperimentStep> steps;
    [SerializeField] private int[] numMixturePerStep;
    [SerializeField] private int highestStepReached;

    [SerializeField] private Assistant assistant;

    public List<ExperimentStep> Steps => steps;

    public int HighestStepReached
    {
        get => highestStepReached;
        set
        {
            highestStepReached = value;
            assistant.SetHighestStepReached(value);
        }
    }

    private void Start()
    {
        numMixturePerStep = new int[steps.Count];
        highestStepReached = -1;
    }

    public bool IsLastStepStillReached(SubstanceMixture mix)
    {
        if(mix.ExperimentStepReached >= 0)
        {
            ExperimentStep lastStep = steps[mix.ExperimentStepReached];
            if (!mix.HasSameSubstancePercentage(lastStep.GetResultingSubstanceMixture()))
            {
                return false;
            }
        }
        return true;
    }

    public void CheckAndModifyStep(SubstanceMixture mix)
    {
        int currentStepReached = mix.ExperimentStepReached;

        if (currentStepReached >= 0 && !IsLastStepStillReached(mix))
        {
            HandleStepFailure(mix);
            return;
        }

        if (currentStepReached + 1 >= steps.Count) return;

        ExperimentStep nextStep = steps[currentStepReached + 1];
        SubstanceMixture targetMixture = nextStep.GetRequiredSubstanceMixture();
        SubstanceMixture resultMixture = nextStep.GetResultingSubstanceMixture();

        if(targetMixture.CanBecome(resultMixture))
        {
            if (mix.CanBecome(targetMixture))
            {
                if (mix.IsSimilarTo(targetMixture))
                {
                    Debug.Log("Avanzamento");
                    AdvanceToNextStep(mix);
                }
            }
            else
            {
                if (currentStepReached >= 0)
                {
                    Debug.Log("Sbagliato");
                    HandleStepFailure(mix);
                }
            }
        } else
        {
            if (mix.IsQuantityWithinTolerance(targetMixture))
            {
                if (mix.IsSimilarTo(targetMixture))
                {
                    Debug.Log("Avanzamento");
                    AdvanceToNextStep(mix);
                }
            }
            else
            {
                if (currentStepReached >= 0)
                {
                    Debug.Log("Sbagliato");
                    HandleStepFailure(mix);
                }
            }
        }
    }

    private void AdvanceToNextStep(SubstanceMixture mix)
    {
        ExperimentStep nextStep = steps[mix.ExperimentStepReached + 1];

        if (mix.ExperimentStepReached + 1 > highestStepReached)
        {
            HighestStepReached = mix.ExperimentStepReached + 1;
            numMixturePerStep[mix.ExperimentStepReached + 1] += 1;
        }

        nextStep.ApplyStepEffect(mix);
        SetMixtureStepAndUpdateCount(mix, mix.ExperimentStepReached + 1);  
    }

    public void SetMixtureStepAndUpdateCount(SubstanceMixture mix, int nextStep)
    {
        int currentStep = mix.ExperimentStepReached;
        if (currentStep >= 0)
        {
            numMixturePerStep[currentStep] -= 1;

            Debug.Log("Raggiunto step: " + nextStep);

            HighestStepReached = GetHighestStepReached();

            numMixturePerStep[nextStep] += 1;
        }
        
        mix.ExperimentStepReached = nextStep;
    }

    private void HandleStepFailure(SubstanceMixture mix)
    {
        Debug.Log("Chiamata HandleStepFailure");
        int currentStep = mix.ExperimentStepReached;
        if (mix.ExperimentStepReached >= 0)
        {
            numMixturePerStep[currentStep] -= 1;

            if (numMixturePerStep[currentStep] == 0)
            {
                HighestStepReached = GetHighestStepReached();
            }

            mix.ExperimentStepReached = - 1;
        }
    }

    public int GetHighestStepReached()
    {
        for (int i = numMixturePerStep.Length - 1; i >= 0; i--)
        {
            if (numMixturePerStep[i] > 0)
            {
                return i;
            }
        }
        return -1;
    }
}
