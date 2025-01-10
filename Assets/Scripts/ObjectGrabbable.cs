using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    private float defaultLinearDamping;
    private float holdingLinearDamping = 10;

    private float defaultAngularDamping;
    private float holdingAngularDamping = 10;

    private RigidbodyInterpolation defaultInterpolation;
    private RigidbodyInterpolation holdingInterpolation = RigidbodyInterpolation.Interpolate;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();

        defaultInterpolation = objectRigidbody.interpolation;
        objectRigidbody.interpolation = holdingInterpolation;

        defaultLinearDamping = objectRigidbody.linearDamping;
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            objectRigidbody.linearVelocity = 10 * (objectGrabPointTransform.position - transform.position);
            Debug.Log(objectRigidbody.linearVelocity);
        }
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
        objectRigidbody.linearDamping = holdingLinearDamping;
        objectRigidbody.angularDamping = holdingAngularDamping;
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
