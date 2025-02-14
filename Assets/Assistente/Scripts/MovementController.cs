using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MovementController : MonoBehaviour
{
    public Transform player;
    public Transform labPosition;

    public float speed = 0.0000000001f;
    private bool hasStartedMoving = false;

    void Start()
    {
        // Inizia la coroutine per il ritardo
        StartCoroutine(WaitBeforeMoving());
    }

    // Coroutine che aspetta 1 secondo prima di iniziare a muoversi
    private IEnumerator WaitBeforeMoving()
    {
        // Aspetta 1 secondo
        yield return new WaitForSeconds(0.3f);

        // Imposta che il movimento può iniziare
        hasStartedMoving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStartedMoving)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Mantieni solo la rotazione sul piano orizzontale

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            targetRotation *= Quaternion.Euler(0, 90, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);

            if (labPosition != null)
            {
                transform.position = Vector3.Lerp(transform.position, labPosition.position, speed * Time.deltaTime);
            }
        }
       
    }
}
