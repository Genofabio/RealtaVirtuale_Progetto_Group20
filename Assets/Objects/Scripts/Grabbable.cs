using UnityEngine;

public class Grabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

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

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();

        defaultInterpolation = objectRigidbody.interpolation;
        defaultLinearDamping = objectRigidbody.linearDamping;

        initialVelocityMultiplier = 5f;
        finalVelocityMultiplier = 10f;

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
        }

        hitMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitMarker.transform.localScale = Vector3.one * 0.1f;
        Destroy(hitMarker.GetComponent<Collider>());

        // Creazione di un materiale Unlit per evitare luci e ombre
        Material unlitMaterial = new Material(Shader.Find("Unlit/Color"));
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
            Vector3 distance = objectGrabPointTransform.position - transform.position;

            if (justGrabbed && distance.magnitude < 0.4f)
            {
                velocityMultiplier = finalVelocityMultiplier;
                justGrabbed = false;
            }

            objectRigidbody.linearVelocity = velocityMultiplier * (objectGrabPointTransform.position - transform.position);

            Vector3 correctionAxis = Vector3.Cross(transform.up, Vector3.up);
            float correctionMagnitude = correctionAxis.magnitude;
            objectRigidbody.AddTorque(correctionAxis.normalized * correctionMagnitude * 10f);
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
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
        objectRigidbody.linearDamping = defaultLinearDamping;
        objectRigidbody.angularDamping = defaultAngularDamping;
        objectRigidbody.interpolation = defaultInterpolation;
    }

    public void DeleteLineRenderer()
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        Destroy(hitMarker);
        hitMarker = null;
    }
}
