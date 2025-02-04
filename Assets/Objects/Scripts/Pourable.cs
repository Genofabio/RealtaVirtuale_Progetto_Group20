using UnityEngine;

public class Pourable : MonoBehaviour
{
    public void Pour(Fillable contenitor) 
    { 
        Debug.Log($"Pourable object {name} versato in Fillable object {contenitor.name}!"); 
    }
}
