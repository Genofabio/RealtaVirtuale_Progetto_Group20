using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MovementController : MonoBehaviour
{
    public Transform player;
    public Transform labPosition;

    public float speed = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Mantieni solo la rotazione sul piano orizzontale

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        targetRotation *= Quaternion.Euler(0, 90, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);

        if (labPosition != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, labPosition.position, speed * Time.deltaTime);
        }
    }
}
