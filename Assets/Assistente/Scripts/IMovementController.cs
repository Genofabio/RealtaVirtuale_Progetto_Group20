using UnityEngine;

public interface IMovementController
{
    public void NotifyNewStep(int highestStepReached);

    public void TogglePause();
    
}
