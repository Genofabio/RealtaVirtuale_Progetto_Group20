
using UnityEngine;

public class Liquid : MonoBehaviour
{
    private Renderer liquidRend;

    private Vector3 lastPos;
    private Vector3 velocity;
    private Vector3 lastRot;
    private Vector3 angularVelocity;
    public float MaxWobble = 0.001f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    private float wobbleAmountX;
    private float wobbleAmountZ;
    private float wobbleAmountToAddX;
    private float wobbleAmountToAddZ;
    private float pulse;
    private float time = 0.5f;

    private float liquidAmount = 0f;

    void Start()
    {
        liquidRend = GetComponent<Renderer>();
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
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

        liquidRend.material.SetFloat("_ScaleToUse", scaleToUse);
        liquidRend.material.SetFloat("_Fill", liquidAmount);

        float deltaTime = Mathf.Max(Time.deltaTime, 0.0001f);
        time += deltaTime;

        // decrease wobble over time
        float recoveryFactor = deltaTime * Recovery;
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, recoveryFactor);
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, recoveryFactor);

        // make a sine wave of the decreasing wobble
        pulse = 2 * Mathf.PI * WobbleSpeed;
        float wobbleScale = Mathf.Clamp01(liquidAmount);
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time) * wobbleScale;
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time) * wobbleScale;

        // send it to the shader
        liquidRend.material.SetFloat("_WobbleX", wobbleAmountX);
        liquidRend.material.SetFloat("_WobbleZ", wobbleAmountZ);

        // velocity
        velocity = (lastPos - transform.position) / deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;

        // add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }

    public void SetFillSize(float volume)
    {
        liquidAmount = volume;
    }
}

