using UnityEngine;

public class Assistant : MonoBehaviour
{

    [SerializeField] private int highestStepReached;

    void Start()
    {
        highestStepReached = -1;
    }

    public void SetHighestStepReached(int newValue)
    {
        if (highestStepReached < newValue)
        {
            highestStepReached = newValue;
            Debug.Log("Complimenti hai raggiunto lo step: " + highestStepReached);
        } 
        else
        {
            highestStepReached = newValue;
            Debug.Log("Oh noooooooo hai raggiunto lo step: " + highestStepReached);
        }
    }
}
