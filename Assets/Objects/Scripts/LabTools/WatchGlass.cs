using UnityEngine;

public class WatchGlass : MonoBehaviour, Fillable
{
    private SubstanceMixture containedMixture;
    private SolidRenderer solidRenderer;
    [SerializeField] private float maxCapacity;
    public float actualVolume = 0f;

    private ExperimentManager experimentManager;

    void Start()
    {
        solidRenderer = GetComponentInChildren<SolidRenderer>();
        containedMixture = new SubstanceMixture(new System.Collections.Generic.List<Substance>(), false, false, 0, false, 0, -1, Color.clear, Color.green);

        experimentManager = FindFirstObjectByType<ExperimentManager>();
    }

    public void Fill(SubstanceMixture mix)
    {
        foreach (var sub in mix.Substances)
        {
            if (!sub.IsSolid)
            {
                Debug.LogWarning("il vetrino da orologiaio non può contenere liquidi");
                return;
            }
        }

        if (containedMixture.ExperimentStepReached < mix.ExperimentStepReached)
        {
            experimentManager.SetMixtureStepAndUpdateCount(containedMixture, mix.ExperimentStepReached);
        }

        containedMixture.AddSubstanceMixture(mix);

        //experimentManager.CheckAndModifyStep(containedMixture);

        actualVolume = 0f; 
        foreach (var sub in containedMixture.Substances)
        {
            actualVolume += sub.Quantity;
        }

        if (containedMixture != null)
        {
            experimentManager.CheckAndModifyStep(containedMixture);
        }

        solidRenderer.SetFillSize(actualVolume / maxCapacity);
        solidRenderer.SetColor(containedMixture.MixtureSolidColor);
        //RefreshTotalWeight();
    }

    public float GetRemainingVolume()
    {
        return maxCapacity - actualVolume;
    }

    public void StirContents()
    {
        throw new System.NotImplementedException();
    }

    public bool CanContainLiquid()
    {
        return false;
    }

    public SubstanceMixture GetContainedSubstanceMixture()
    {
        return containedMixture;
    }
}
