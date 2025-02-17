using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    [Header("Crosshair and Context UI")]
    [SerializeField] private CrosshairController crosshair;
    [SerializeField] private ContextUIController contextUIController;

    [Header("Audio")]
    [SerializeField] private PlayerAudio playerAudio;
    [SerializeField] private AudioSource audioSource;

    private bool isPouring = false;

    // controllo sulla modalità menu iniziale
    private bool inMainMenu = false;
    private bool inPause = false;

    void Awake()
    {
        pickUpLayerMask = ~LayerMask.GetMask("Ignore Raycast");
        rb = GetComponent<Rigidbody>();
        grabbedObject = null;

        Cursor.lockState = CursorLockMode.Locked; // nasconde l'icona del cursore

        // controllo sulla modalità menu iniziale
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            inMainMenu = true;
        }
    }

    void Update()
    {
        if (isPouring && grabbedObject != null)
        {
            if (grabbedObject.TryGetComponent<Pourable>(out var pourableObject))
            {
                Transform cameraHolderTransform = cameraHolder.transform;
                if (Physics.Raycast(cameraHolderTransform.position, cameraHolderTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
                {
                    if (hit.transform.TryGetComponent<Fillable>(out var fillableObject))
                    {
                        float pourAmount = Time.deltaTime * 10;
                        pourableObject.Pour(fillableObject, pourAmount);
                    }
                    else
                    {
                        isPouring = false;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void LateUpdate()
    {
        RotateCamera();
        UpdateCrosshair();
        UpdateContextUI();
    }

    public void TogglePause()
    {
        if (inPause)
        {
            //Time.timeScale = 1;
            inPause = false;
        }
        else
        {
            //Time.timeScale = 0;
            inPause = true;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (inMainMenu || inPause) return;
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();

    }

    public void TogglePickUp(InputAction.CallbackContext context)
    {
        if (inMainMenu || inPause) return;

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

    public void MoveGrabbedObject(InputAction.CallbackContext context)
    {
        Debug.Log("Scrolling");
        float scrollValue = context.ReadValue<float>(); // Ottieni il valore della rotella

        if (grabbedObject == null) return; // Se non c'è un oggetto preso, esci

        float minDistance = 0.3f;
        float maxDistance = 3.0f;

        // Usa la forward della camera per determinare la direzione
        Vector3 cameraForward = cameraHolder.transform.forward;
        Vector3 direction = cameraForward * 0.1f;

        Vector3 newPosition = objectGrabPointTransform.position;

        if (scrollValue > 0)
        {
            newPosition += direction;
        }
        else if (scrollValue < 0)
        {
            newPosition -= direction;
        }

        // Calcola la distanza solo lungo la forward della camera
        Vector3 offset = newPosition - cameraHolder.transform.position;
        float projectedDistance = Vector3.Dot(offset, cameraForward);

        if (projectedDistance >= minDistance && projectedDistance <= maxDistance)
        {
            Debug.Log($"Distanza lungo la forward: {projectedDistance}");
            objectGrabPointTransform.position = newPosition;
        }
    }


    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SimulateUIClick();

            if (inMainMenu || inPause) return;

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
                            isPouring = true;
                        }
                    }
                }
                //Oggetto di tipo Dropper, gestione di PickUp e Drop
                else if (grabbedObject.TryGetComponent<Dropper>(out var dropperObject))
                {
                    if (dropperObject.IsFull())
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
                                dropperObject.Suck(pourableObject2);
                            }
                        }
                    }
                }
                //Oggetto di tipo StirringRod, gestione del mescolamento di sostanze
                else if (grabbedObject.TryGetComponent<StirringRod>(out var rod))
                {
                    Transform cameraHolderTransform = cameraHolder.transform;
                    if (Physics.Raycast(cameraHolderTransform.position, cameraHolderTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
                    {
                        if (hit.transform.TryGetComponent<Fillable>(out var fillableObject))
                        {
                            rod.Mix(fillableObject);
                        }
                    }
                }
            } else if (grabbedObject == null)
            {
                Transform cameraHolderTransform = cameraHolder.transform;
                if (Physics.Raycast(cameraHolderTransform.position, cameraHolderTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
                {
                    // Tara della bilancia
                    if (hit.transform.TryGetComponent<BalanceButton>(out var button))
                    {
                        button.Tare();
                    }
                    // Apertura forno
                    else if (hit.transform.TryGetComponent<Openable>(out var openable))
                    {
                        openable.ToggleDoor();
                    }
                    // Apertura frigorifero
                    else if (hit.transform.TryGetComponent<Fridge>(out var fridge))
                    {
                        fridge.ToggleDoor();
                    }
                }
            } 
        } else if(context.canceled)
        {
            isPouring = false;
        }
    }

    void SimulateUIClick()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = new Vector2(Screen.width / 2, Screen.height / 2); // Punto centrale dello schermo

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            GameObject clickedObject = results[0].gameObject;
            Debug.Log("UI Clicked: " + clickedObject.name);

            ExecuteEvents.Execute(clickedObject, pointerData, ExecuteEvents.pointerClickHandler);
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
                // Ottieni la posizione locale dell'oggetto rispetto alla telecamera
                Vector3 localObjectPosition = cameraHolderTransform.InverseTransformPoint(grabbedObject.transform.position);

                // Imposta la Y come quella del point di grab, per non cambiarla
                localObjectPosition.y = cameraHolderTransform.InverseTransformPoint(objectGrabPointTransform.position).y;

                // Allinea la X in base alla posizione di objectGrabPointTransform
                localObjectPosition.x = cameraHolderTransform.InverseTransformPoint(objectGrabPointTransform.position).x;

                // Calcola la nuova posizione globale
                Vector3 newPosition = cameraHolderTransform.TransformPoint(localObjectPosition);

                // Imposta la nuova posizione di objectGrabPointTransform
                objectGrabPointTransform.position = newPosition;
                grabbedObject.Grab(objectGrabPointTransform);

                // Riproduci l'audio
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

    public void UpdateContextUI()
    {
        if (inMainMenu || inPause) return;

        Transform cameraHolderTransform = cameraHolder.transform;
        if (Physics.Raycast(cameraHolderTransform.position, cameraHolderTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
        {
            contextUIController.CheckContextState(grabbedObject, hit);
        }
        else
        {
            contextUIController.HideContextUI();
        }
    }
}
