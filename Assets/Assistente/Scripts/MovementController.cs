using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UIElements;
using System.Linq;

public class MovementController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    public Transform player;
    public Transform pos0;
    public Transform pos1;
    public Transform pos2;
    public Transform pos3;
    public Transform posX;
    public Transform pos5;
    public Transform pos6;
    public Transform pos7;
    private bool pathCompleted = false;

    private bool newStepReached = false;
    private int highestStepReached = -1;

    private Transform[] positions;  // Array delle posizioni
    private int currentIndex = 0;   // Indice della posizione corrente

    private float yRobot;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        yRobot = transform.position.y;

        CheckStepAndMove();

        //StartCoroutine(DelayedStepChange());
    }

    //IEnumerator DelayedStepChange()
    //{
    //    yield return new WaitForSeconds(20f); // Aspetta 10 secondi
    //    NotifyNewStep(++highestStepReached);  // Chiama NotifyNewStep(0)
    //}

    void Update()
    {
        // ruota l'assistente verso il player
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            transform.rotation = lookRotation * Quaternion.Euler(0, 90, 0);
        }

        // Effetto di oscillazione verticale (fluttuazione)
        float oscillationSpeed = 1f;  // Velocità dell'oscillazione
        float oscillationHeight = 0.2f; // Altezza massima dell'oscillazione
        float newY = Mathf.Sin(Time.time * oscillationSpeed) * oscillationHeight;

        // Applica la fluttuazione alla posizione dell'oggetto
        transform.position = new Vector3(transform.position.x, yRobot + newY, transform.position.z);

        if (pathCompleted = true && newStepReached)
        {
            Debug.Log("pathCompleted = true && newStepReached");
            CheckStepAndMove();
        }
    }

    public void NotifyNewStep(int highestStepReached) 
    {
        this.highestStepReached = highestStepReached;
        newStepReached = true;
    }

    private void CheckStepAndMove()
    {
        Debug.Log("entraa??? " + highestStepReached);
        if (highestStepReached == -1)
        {
            positions = new Transform[] { pos1, pos2, pos3, posX };
        }
        else if (highestStepReached == 0)
        {
            positions = new Transform[] { pos1, posX };
            Debug.Log("entra in highstep 0");
        }
        else if (highestStepReached == 1)
        {
            positions = new Transform[] { pos5, posX };
        }
        else if (highestStepReached == 2)
        {
            positions = new Transform[] { pos6 };
        }
        else if (highestStepReached == 3)
        {
            positions = new Transform[] { pos5, posX };
        }
        else if (highestStepReached == 4)
        {
            positions = new Transform[] { pos5, posX };
        }
        else if (highestStepReached == 5)
        {
            positions = new Transform[] { pos7 };
        }
        Debug.Log("positions: " + string.Join(", ", positions.Select(p => p.name)));
        
        StartCoroutine(MoveBetweenPositions());
    }

    IEnumerator MoveBetweenPositions()
    {
        pathCompleted = false;
        newStepReached = false;
        currentIndex = 0;
        while (currentIndex != positions.Length && pathCompleted == false)
        {
            Debug.Log("currentIndex: " + currentIndex);
            Debug.Log(positions[currentIndex]);
            // Imposta la destinazione alla posizione corrente
            _navMeshAgent.SetDestination(positions[currentIndex].position);

            // Aspetta finché l'NPC non è arrivato a destinazione
            while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > 0.1f)
            {
                yield return null;
            }

            // Passa alla prossima posizione (ciclica)
            currentIndex++;

            if (currentIndex != positions.Length)
                // Attendi 2 secondi
                yield return new WaitForSeconds(2f);

            
        }
        Debug.Log("esce dal while");
        pathCompleted = true;
    }
}
