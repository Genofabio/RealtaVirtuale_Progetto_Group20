using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    private float defaultLinearDamping;
    private float holdingLinearDamping = 5;

    private float defaultAngularDamping;
    private float holdingAngularDamping = 10;

    private RigidbodyInterpolation defaultInterpolation;
    private RigidbodyInterpolation holdingInterpolation = RigidbodyInterpolation.Interpolate;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();

        defaultInterpolation = objectRigidbody.interpolation;

        defaultLinearDamping = objectRigidbody.linearDamping;
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            // Raggiunge la nuova posizione dell'oggetto
            objectRigidbody.linearVelocity = 10 * (objectGrabPointTransform.position - transform.position);

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
