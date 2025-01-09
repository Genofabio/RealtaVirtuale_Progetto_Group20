using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Camera")]
    [SerializeField] private GameObject cameraHolder;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float rotationSensitivity = 0.3f;
    [SerializeField] private float maxForce = 1f;

    private Rigidbody rb;
    private Vector2 movementInput, lookInput;
    private float verticalLookRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        transform.Rotate(Vector3.up * lookInput.x * rotationSensitivity);

        // Rotazione verticale
        verticalLookRotation -= lookInput.y * rotationSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -85f, 85f);

        // Applica la rotazione verticale
        cameraHolder.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }
}
