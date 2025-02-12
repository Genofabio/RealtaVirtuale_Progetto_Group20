using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Openable
{
    protected Quaternion closedRotation;
    protected Quaternion openRotation;
    private BoxCollider boxCollider;

    [SerializeField] private float openAngle = 90f; // Angolo personalizzabile dall'Inspector
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // Asse di rotazione personalizzabile

    public bool IsOpen { get; protected set; } = false;
    public bool IsMoving { get; protected set; } = false;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        closedRotation = transform.localRotation;

        // Calcola la rotazione aperta aggiungendo l'angolo all'asse scelto
        openRotation = Quaternion.Euler(closedRotation.eulerAngles + rotationAxis * openAngle);
    }

    public void ToggleDoor()
    {
        if (IsMoving) return;

        if (IsOpen)
            StartCoroutine(CloseDoorCoroutine());
        else
            StartCoroutine(OpenDoorCoroutine());
    }

    protected virtual IEnumerator OpenDoorCoroutine()
    {
        IsMoving = true;
        IsOpen = true;
        if (boxCollider) boxCollider.enabled = false;
        yield return RotateDoor(openRotation);
        if (boxCollider) boxCollider.enabled = true;
        IsMoving = false;
    }

    protected virtual IEnumerator CloseDoorCoroutine()
    {
        IsMoving = true;
        if (boxCollider) boxCollider.enabled = false;
        yield return RotateDoor(closedRotation);
        if (boxCollider) boxCollider.enabled = true;
        IsOpen = false;
        IsMoving = false;
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        float duration = 1f;
        float elapsed = 0f;

        Quaternion startRotation = transform.localRotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.localRotation = targetRotation;
    }
}
