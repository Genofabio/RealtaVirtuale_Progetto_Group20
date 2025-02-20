using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

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

    private int[] times;

    [SerializeField] private List<AudioClip> audioList;
    private List<AudioClip> tempAudioList;
    [SerializeField] private AudioSource audioSource;

    private bool newStepReached = false;
    private int highestStepReached = -1;

    private Transform[] positions;  // Array delle posizioni
    private int currentIndex = 0;   // Indice della posizione corrente

    private float yRobot;

    private bool justStarted = true;

    private Coroutine movementRoutine;

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
        float oscillationHeight = 0.05f; // Altezza massima dell'oscillazione
        float newY = Mathf.Sin(Time.time * oscillationSpeed) * oscillationHeight;

        // Applica la fluttuazione alla posizione dell'oggetto
        transform.position = new Vector3(transform.position.x, yRobot + newY, transform.position.z);

        if (newStepReached)
        {
            if (pathCompleted != true && movementRoutine != null)
            {
                StopCoroutine(movementRoutine);
            }
            justStarted = false;
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
        //Debug.Log("entraa??? " + highestStepReached);
        if(justStarted)
        {
            positions = new Transform[] { pos0, pos2, pos1, pos3, posX };
            times = new int[] { 12, 10, 13, 13, 11};
            tempAudioList = new List<AudioClip> { audioList[0], audioList[1], audioList[2], audioList[3], audioList[4] };
        }
        else if (highestStepReached == -1 && !justStarted) // Da fare: Mix sostanze
        {
            positions = new Transform[] { posX };
            times = new int[] { 12 };
            tempAudioList = new List<AudioClip> { audioList[4] };
        }
        else if (highestStepReached == 0) // Da fare: Aggiunta NaOH
        {
            positions = new Transform[] { pos1, posX };
            times = new int[] { 5, 5 };
            tempAudioList = new List<AudioClip> { audioList[5], null };
        }
        else if (highestStepReached == 1) // Da fare: Mix
        {
            positions = new Transform[] { pos5, posX };
            times = new int[] { 5, 5 };
            tempAudioList = new List<AudioClip> { audioList[6], null };
        }
        else if (highestStepReached == 2) // Da fare: frigo
        {
            positions = new Transform[] { pos6 };
            times = new int[] { 5 };
            tempAudioList = new List<AudioClip> { audioList[7] };
        }
        else if (highestStepReached == 3) // Da fare: Imbuto
        {
            positions = new Transform[] { pos5, posX };
            times = new int[] { 5, 5 };
            tempAudioList = new List<AudioClip> { audioList[8], null };
        }
        else if (highestStepReached == 4) // Da fare: Vetrino e forno
        {
            positions = new Transform[] { pos5, pos7 };
            times = new int[] { 5, 5 };
            tempAudioList = new List<AudioClip> { audioList[9], null };
        }
        else if (highestStepReached == 5)
        {
            positions = new Transform[] { pos0 };
            times = new int[] { 5 };
            tempAudioList = new List<AudioClip> { audioList[10] };
        }
        //Debug.Log("positions: " + string.Join(", ", positions.Select(p => p.name)));

        movementRoutine = StartCoroutine(MoveBetweenPositions());
    }

    IEnumerator MoveBetweenPositions()
    {
        pathCompleted = false;
        newStepReached = false;
        currentIndex = 0;
        while (currentIndex != positions.Length && pathCompleted == false)
        {
            //Debug.Log("currentIndex: " + currentIndex);
            Debug.Log(positions[currentIndex]);
            // Imposta la destinazione alla posizione corrente
            _navMeshAgent.SetDestination(positions[currentIndex].position);

            // Aspetta finché l'NPC non è arrivato a destinazione
            while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > 0.1f)
            {
                yield return null;
            }

            // Passa alla prossima posizione (ciclica)
            if (tempAudioList[currentIndex] != null)
            {
                StartCoroutine(PlayAudioAndWait(tempAudioList[currentIndex]));
            }
            currentIndex++;


            if (currentIndex != positions.Length)
                // Attendi 2 secondi
                yield return new WaitForSeconds(times[currentIndex - 1]);


        }
        Debug.Log("esce dal while");
        pathCompleted = true;
    }

    private IEnumerator PlayAudioAndWait(AudioClip clip)
    {
        if (clip != null)
        {

            // Attende finché l'audio è in riproduzione
            while (audioSource.isPlaying)
            {
                yield return null; // Aspetta un frame
            }

            audioSource.clip = clip;
            audioSource.Play();

            Debug.Log("Riproduzione audio terminata.");
            // Qui puoi aggiungere il codice che deve essere eseguito dopo la riproduzione
        }
    }
}
