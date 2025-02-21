using System;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public event Action OnDropped; // Evento che segnala il rilascio dell'oggett

    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    private Transform actualPivotPoint;
    private Transform rotationPourPivot;
    private bool isRotating;

    private float minRotationAngle = 0; // Angolo minimo di inclinazione
    private float maxRotationAngle = -180f; // Angolo massimo di inclinazione

    private float currentRotationAngle = 0f; // Angolo attuale dell'oggetto
    private float initialRotationAngle = 0f; // Angolo iniziale dell'oggetto

    private float defaultLinearDamping;
    private float holdingLinearDamping = 5;

    private float defaultAngularDamping;
    private float holdingAngularDamping = 10;

    private RigidbodyInterpolation defaultInterpolation;
    private RigidbodyInterpolation holdingInterpolation = RigidbodyInterpolation.Interpolate;

    private bool justGrabbed;

    private float velocityMultiplier;
    [SerializeField] private float initialVelocityMultiplier;
    [SerializeField] private float finalVelocityMultiplier;

    [SerializeField] private LineRenderer lineRenderer;
    private GameObject hitMarker;
    private Vector3 targetHitPosition;

    [Header("Audio")]
    [SerializeField] private AudioClip pickUpSound; // Suono di pick up
    [SerializeField] private AudioClip dropSound;   // Suono di drop
    private AudioSource audioSource;

    [SerializeField] private Material lineMat;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>(); // Assicurati che l'oggetto abbia un AudioSource

        actualPivotPoint = transform; // Di default il pivot è l'oggetto stesso
        rotationPourPivot = transform;

        defaultInterpolation = objectRigidbody.interpolation;
        defaultLinearDamping = objectRigidbody.linearDamping;

        initialVelocityMultiplier = 5f;
        finalVelocityMultiplier = 10f;

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.material = lineMat;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
        }

        hitMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitMarker.transform.localScale = Vector3.one * 0.04f;
        Destroy(hitMarker.GetComponent<Collider>());

        // Creazione di un materiale Unlit per evitare luci e ombre
        Material unlitMaterial = lineMat;
        unlitMaterial.color = new Color(1f, 1f, 1f, 0.5f);

        // Assegna il materiale all'hit marker
        Renderer hitRenderer = hitMarker.GetComponent<Renderer>();
        hitRenderer.material = unlitMaterial;
        hitRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        hitMarker.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            if (!isRotating)
            {
                Vector3 distance = objectGrabPointTransform.position - transform.position;

                if (justGrabbed && distance.magnitude < 0.4f)
                {
                    velocityMultiplier = finalVelocityMultiplier;
                    justGrabbed = false;
                }

                objectRigidbody.linearVelocity = velocityMultiplier * distance;
            }

            if (!isRotating)
            {
                Vector3 correctionAxis = Vector3.Cross(transform.up, Vector3.up);
                float correctionMagnitude = correctionAxis.magnitude;
                objectRigidbody.AddTorque(correctionAxis.normalized * correctionMagnitude * 40 * 20f);
            }
            else if (isRotating)
            {
                if (actualPivotPoint != null)
                {
                    float baseRotationSpeed = -27f * Time.deltaTime;
                    float rotationSpeed = baseRotationSpeed;

                    if (currentRotationAngle >= initialRotationAngle)
                    {
                        rotationSpeed *= 10f;
                    }

                    float newRotationAngle = currentRotationAngle + rotationSpeed;

                    if (newRotationAngle <= minRotationAngle && newRotationAngle >= maxRotationAngle)
                    {
                        transform.RotateAround(rotationPourPivot.position, transform.right, rotationSpeed);
                        currentRotationAngle = newRotationAngle; // Aggiorna l'angolo attuale
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (lineRenderer == null || hitMarker == null) return;

        if (objectGrabPointTransform != null)
        {
            Vector3 rayStart = transform.position + Vector3.up * 0.01f;
            Vector3 rayDirection = Vector3.down;
            float rayLength = 5f;
            RaycastHit hit;

            if (Physics.Raycast(rayStart, rayDirection, out hit, rayLength))
            {
                rayLength = hit.distance;
                targetHitPosition = hit.point;
                hitMarker.SetActive(true);
            }
            else
            {
                targetHitPosition = rayStart + rayDirection * rayLength;
                hitMarker.SetActive(false);
            }

            lineRenderer.SetPosition(0, rayStart);
            lineRenderer.SetPosition(1, targetHitPosition);

            if (hitMarker.activeSelf)
            {
                hitMarker.transform.position = targetHitPosition;
            }
        }
        else
        {
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.zero);
            hitMarker.SetActive(false);
        }
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
        objectRigidbody.linearDamping = holdingLinearDamping;
        objectRigidbody.angularDamping = holdingAngularDamping;
        objectRigidbody.interpolation = holdingInterpolation;
        justGrabbed = true;
        velocityMultiplier = initialVelocityMultiplier;

        PlayPickUpSound(); // Riproduci suono di pick up
    }

    public void Drop()
    {
        objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
        objectRigidbody.linearDamping = defaultLinearDamping;
        objectRigidbody.angularDamping = defaultAngularDamping;
        objectRigidbody.interpolation = defaultInterpolation;

        PlayDropSound(); // Riproduci suono di drop

        OnDropped?.Invoke(); // Notifica tutti gli iscritti che l'oggetto è stato rilasciato
    }

    private void PlayPickUpSound()
    {
        if (audioSource && pickUpSound)
        {
            audioSource.clip = pickUpSound;
            audioSource.Play(); // Riproduce il suono di pick up
        }
    }

    private void PlayDropSound()
    {
        if (audioSource && dropSound)
        {
            audioSource.clip = dropSound;
            audioSource.Play(); // Riproduce il suono di drop
        }
    }

    public void StartRotating(Transform pivotPoint, float initialRotation)
    {
        isRotating = true;

        objectRigidbody.isKinematic = true;

        rotationPourPivot = pivotPoint;

        initialRotationAngle = initialRotation-90f;
        //Debug.Log("Initial rotation angle: " + initialRotationAngle);

        // L'asse di rotazione è "right"
        Vector3 rotationAxis = transform.right;

        // Proietta la direzione 'forward' dell'oggetto sul piano di rotazione
        Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, rotationAxis);

        // Usa Vector3.down come riferimento invece di Vector3.forward
        Vector3 referenceDirection = Vector3.ProjectOnPlane(Vector3.down, rotationAxis);

        // Calcola l'angolo rispetto alla nuova direzione di riferimento (Vector3.down)
        float angleRelativeToPivot = Vector3.SignedAngle(referenceDirection, projectedForward, rotationAxis);

        // Salviamo l'angolo attuale con il nuovo sistema di riferimento
        currentRotationAngle = angleRelativeToPivot;

        //Debug.Log("Current rotation angle (Y-negative reference): " + currentRotationAngle);

        GameObject tempPivot = new GameObject("TempPivot");
        tempPivot.transform.position = new Vector3(pivotPoint.position.x, transform.position.y, pivotPoint.position.z);
        tempPivot.transform.rotation = transform.rotation;

        actualPivotPoint = tempPivot.transform;
    }

    public void StopRotating()
    {
        isRotating = false;
        actualPivotPoint = transform; // Usa il pivot di default
        currentRotationAngle = 0f; // Resetta l'angolo
        initialRotationAngle = 0f; // Resetta l'angolo iniziale
        objectRigidbody.isKinematic = false;
    }

    public void DeleteLineRenderer()
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        Destroy(hitMarker);
        hitMarker = null;
    }

    public bool IsGrabbed()
    {
        return objectGrabPointTransform != null;
    }
}
