using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private Transform objectGrabPointTransform;

    private Rigidbody rb;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float rotationSensitivity = 0.3f;
    [SerializeField] private float maxForce = 1f;

    private Vector2 movementInput;
    private Vector2 lookInput;
    private float verticalLookRotation;

    [Header("PickUp Settings")]
    [SerializeField] private float pickUpDistance = 2f;
    [SerializeField] private LayerMask pickUpLayerMask;

    private ObjectGrabbable grabbedObject;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabbedObject = null;

        Cursor.lockState = CursorLockMode.Locked; // nasconde l'icona del cursore
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void LateUpdate()
    {
        RotateCamera();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void TogglePickUp(InputAction.CallbackContext context)
    {
        if (grabbedObject == null)
        {
            PickUp();
        } else
        {
            grabbedObject.Drop();
            grabbedObject = null;
        }
        
    }

    public void MovePlayer()
    {
        // Calcola la velocità target
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 targetVelocity = new Vector3(movementInput.x, 0, movementInput.y) * movementSpeed;
        targetVelocity = transform.TransformDirection(targetVelocity);

        // Calcola il cambiamento di velocità richiesto
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange.y = 0;

        // Limita la forza applicata
        velocityChange = Vector3.ClampMagnitude(velocityChange, maxForce);

        // Applica la forza
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    public void RotateCamera()
    {
        // Rotazione orizzontale
        Quaternion horizontalRotation = Quaternion.Euler(0f, lookInput.x * rotationSensitivity, 0f);
        rb.MoveRotation(rb.rotation * horizontalRotation);

        // Rotazione verticale
        verticalLookRotation -= lookInput.y * rotationSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -85f, 85f);

        // Applica la rotazione verticale
        cameraHolder.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    public void PickUp()
    {
        Transform cameraHolderTransform = cameraHolder.transform;
        if(Physics.Raycast(cameraHolderTransform.position, cameraHolderTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
        {
            if (hit.transform.TryGetComponent(out grabbedObject))
            {
                grabbedObject.Grab(objectGrabPointTransform);
                Debug.Log(grabbedObject.transform);
            }
        }
    }
}
