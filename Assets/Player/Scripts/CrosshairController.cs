using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    private float inactiveOpacity = 0.5f;
    private float inactiveSize = 6;

    private float activeOpacity = 1f;
    private float activeSize = 8;

    private RectTransform rectTransform;
    private Image image;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        rectTransform.sizeDelta = new Vector2(inactiveSize, inactiveSize);
        Color tmpColor = image.color;
        tmpColor.a = inactiveOpacity;
        image.color = tmpColor;
    }

    public void SetCrosshairActive()
    {
        rectTransform.sizeDelta = new Vector2(activeSize, activeSize);
        Color tmpColor = image.color;
        tmpColor.a = activeOpacity;
        image.color = tmpColor;
    }

    public void SetCrosshairInactive()
    {
        rectTransform.sizeDelta = new Vector2(inactiveSize, inactiveSize);
        Color tmpColor = image.color;
        tmpColor.a = inactiveOpacity;
        image.color = tmpColor;
    }
}
