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


    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();

        defaultInterpolation = objectRigidbody.interpolation;
        defaultLinearDamping = objectRigidbody.linearDamping;

        initialVelocityMultiplier = 3f;
        finalVelocityMultiplier = 10f;
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {

            Vector3 distance = objectGrabPointTransform.position - transform.position;

            if(justGrabbed && distance.magnitude < 0.1f)
            {
                velocityMultiplier = finalVelocityMultiplier;
                justGrabbed = false;
            } 

            // Raggiunge la nuova posizione dell'oggetto
            objectRigidbody.linearVelocity = velocityMultiplier * (objectGrabPointTransform.position - transform.position);

            // Calcola il vettore di correzione
            Vector3 correctionAxis = Vector3.Cross(transform.up, Vector3.up);
            float correctionMagnitude = correctionAxis.magnitude;

            // Applica una forza rotazionale correttiva
            objectRigidbody.AddTorque(correctionAxis.normalized * correctionMagnitude * 10f);
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


}
