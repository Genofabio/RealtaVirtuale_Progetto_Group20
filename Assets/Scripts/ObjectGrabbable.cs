using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    private float defaultLinearDamping;
    private float holdingLinearDamping = 6;

    private float defaultAngularDamping;
    private float holdingAngularDamping = 6;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        objectRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        defaultLinearDamping = objectRigidbody.linearDamping;
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(newPosition);
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
    }


}
