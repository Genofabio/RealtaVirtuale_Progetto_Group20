using UnityEngine;

public class SolidRenderer : MonoBehaviour
{
    private Renderer solidRend;
    private float solidAmount = 0f;

    void Awake()
    {
        solidRend = GetComponent<Renderer>();
    }

    private void Update()
    {
        Vector3 yGlobal = Vector3.up;
        Vector3 scale = transform.lossyScale;

        float dotX = Vector3.Dot(transform.right, yGlobal);
        float dotY = Vector3.Dot(transform.up, yGlobal);
        float dotZ = Vector3.Dot(transform.forward, yGlobal);

        float scaleToUse = 1;

        // Trova quale asse è più allineato con la Y globale
        if (Mathf.Abs(dotX) > Mathf.Abs(dotY) && Mathf.Abs(dotX) > Mathf.Abs(dotZ))
        {
            scaleToUse = scale.x;
        }
        else if (Mathf.Abs(dotY) > Mathf.Abs(dotX) && Mathf.Abs(dotY) > Mathf.Abs(dotZ))
        {
            scaleToUse = scale.y;
        }
        else
        {
            scaleToUse = scale.z;
        }

        // Aggiorna i valori nel materiale
        solidRend.material.SetFloat("_ScaleToUse", scaleToUse);
        solidRend.material.SetFloat("_Fill", solidAmount);
    }

    public void SetFillSize(float volume)
    {
        solidAmount = volume;
    }

    public void SetColor(Color color)
    {
        solidRend.material.SetColor("_LiquidColor", color);

        Color lighterColor = Color.Lerp(color, Color.white, 0.2f);
        solidRend.material.SetColor("_SurfaceColor", lighterColor);

        solidRend.material.SetColor("_FresnelColor", color);
    }
}
