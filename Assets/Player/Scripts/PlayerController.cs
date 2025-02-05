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
    [SerializeField] private float pickUpDistance = 3f;
    [SerializeField] private LayerMask pickUpLayerMask;

    private Grabbable grabbedObject;

    [Header("Crosshair")]
    [SerializeField] private CrosshairController crosshair;

    [Header("Audio")]
    [SerializeField] private PlayerAudio playerAudio;
    [SerializeField] private AudioSource audioSource;


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
        UpdateCrosshair();
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
        if (context.performed)
        {
            if (grabbedObject == null)
            {
                PickUp();
            }
            else
            {
                grabbedObject.Drop();
                grabbedObject = null;
            }
        }

    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (grabbedObject != null)
            {
                //Oggetto di tipo Pourable, gestione del versaggio del liquido dentro un oggetto Fillable
                if (grabbedObject.TryGetComponent<Pourable>(out var pourableObject))
                {
                    Transform cameraHolderTransform = cameraHolder.transform;
                    if (Physics.Raycast(cameraHolderTransform.position, cameraHolderTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
                    {
                        if (hit.transform.TryGetComponent<Fillable>(out var fillableObject))
                        {
                            pourableObject.Pour(fillableObject);
                        }
                    }
                }
                //Oggetto di tipo Dropper, gestione di PickUp e Drop
                else if (grabbedObject.TryGetComponent<Dropper>(out var dropperObject))
                {
                    if (dropperObject.GetFull())
                    {
                        Transform cameraHolderTransform = cameraHolder.transform;
                        if (Physics.Raycast(cameraHolderTransform.position, cameraHolderTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
                        {
                            if (hit.transform.TryGetComponent<Fillable>(out var fillableObject))
                            {
                                dropperObject.Drop(fillableObject);
                            }
                        }
                    }
                    else
                    {
                        Transform cameraHolderTransform = cameraHolder.transform;
                        if (Physics.Raycast(cameraHolderTransform.position, cameraHolderTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
                        {
                            if (hit.transform.TryGetComponent<Pourable>(out var pourableObject2))
                            {
                                dropperObject.PickUp(pourableObject2);
                            }
                        }
                    }
                }
                //oggetto di tipo filtro
                else if (grabbedObject.TryGetComponent<Filter>(out var filterObject))
                {
                    Transform cameraHolderTransform = cameraHolder.transform;
                    if (Physics.Raycast(cameraHolderTransform.position, cameraHolderTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
                    {
                        if (hit.transform.TryGetComponent<Becher>(out var becherObject))
                        {
                            filterObject.ApplyFilter(becherObject);
                        }
                    }
                }
            } else if (grabbedObject == null)
            {
                Transform cameraHolderTransform = cameraHolder.transform;
                if (Physics.Raycast(cameraHolderTransform.position, cameraHolderTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
                {
                    if (hit.transform.TryGetComponent<PrecisionBalance>(out var balance))
                    {
                        balance.Tare();
                    }
                }
            } 
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
        if (Physics.Raycast(cameraHolderTransform.position, cameraHolderTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
        {
            if (hit.transform.TryGetComponent(out grabbedObject))
            {
                if(grabbedObject.TryGetComponent<Becher>(out var becher))
                {
                    Debug.Log("da implementare");
                }
                grabbedObject.Grab(objectGrabPointTransform);
                audioSource.clip = playerAudio.GetAudioClip(PlayerAudioKey.PickUp);
                audioSource.Play();
            }
        }
    }

    public void UpdateCrosshair()
    {
        Transform cameraHolderTransform = cameraHolder.transform;
        if (Physics.Raycast(cameraHolderTransform.position, cameraHolderTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
        {
            if (hit.transform.TryGetComponent<Grabbable>(out var grabbable))
            {
                crosshair.SetCrosshairActive();
            } 
            if (hit.transform.TryGetComponent<Pourable>(out var pourable))
            {
                crosshair.SetCrosshairActive();
            } 
            if (hit.transform.TryGetComponent<Fillable>(out var fillable))
            {
                crosshair.SetCrosshairActive();
            }
        } else
        {
            crosshair.SetCrosshairInactive();
        }
    }
}
