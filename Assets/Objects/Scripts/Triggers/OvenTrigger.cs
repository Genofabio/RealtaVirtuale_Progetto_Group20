using System.Collections.Generic;
using UnityEngine;

public class OvenTrigger : MonoBehaviour
{
    [SerializeField] private Oven oven; 

    private void OnTriggerEnter(Collider other)
    {
        oven.IsEmpty = false;

        Fillable fillable = other.GetComponent<Fillable>();
        if (fillable != null && fillable.GetContainedSubstanceMixture() != null)
        {
            oven.InsertIntoOven(other.gameObject);
            if (fillable.GetContainedSubstanceMixture().ExperimentStepReached == 4)
            {
                oven.CookingMixtureContained = fillable.GetContainedSubstanceMixture();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        oven.IsEmpty = true;

        Fillable fillable = other.GetComponent<Fillable>();
        if (fillable != null && fillable.GetContainedSubstanceMixture() != null)
        {
            oven.RemoveFromOven(other.gameObject);
            oven.CookingMixtureContained = null;
        }
    }
}
