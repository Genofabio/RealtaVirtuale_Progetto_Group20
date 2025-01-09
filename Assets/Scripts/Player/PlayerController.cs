using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;

    // --- Movimento ---
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;

    private Vector2 movementInput;

    // --- Telecamera ---
    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float lookSensityvityX = 30f;
    [SerializeField] private float lookSensitivityY = 30f;
    
    private float cameraPitch = 0f;
    private Vector2 lookInput;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // nasconde l'icona del cursore
    }

    void Update()
    {
        movePlayer();
        playerLook();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void movePlayer()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
        characterController.Move(transform.TransformDirection(moveDirection) * movementSpeed * Time.deltaTime);
    }

    public void playerLook()
    {
        // Rotazione sull'asse X (su/giù)
        cameraPitch -= lookInput.y * lookSensitivityY * Time.deltaTime;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        // Rotazione sull'asse Y (destra/sinistra)
        float yRotation = lookInput.x * lookSensityvityX * Time.deltaTime;
        transform.Rotate(Vector3.up * yRotation);
    }
}
