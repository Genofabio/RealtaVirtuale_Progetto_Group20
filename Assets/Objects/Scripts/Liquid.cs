using UnityEngine;

public class Liquid : MonoBehaviour
{
    Renderer liquidRend;

    private void Awake()
    {
        liquidRend = GetComponent<Renderer>();
    }
    public void GetFillSize()
    {

    }
    public void SetFillSize(float volume)
    {
        liquidRend.material.SetFloat("_Fill", volume);
    }
}
