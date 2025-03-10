using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MovementControllerAdvanced : MonoBehaviour, IMovementController
{
    [Header("Positions")]
    public Transform player;
    public Transform pos0;
    public Transform pos1;
    public Transform pos2;
    public Transform pos3;
    public Transform posX;
    public Transform pos5;
    public Transform pos6;
    public Transform pos7;
    private NavMeshAgent _navMeshAgent;

    [Header("Audio")]
    [SerializeField] private List<AudioClip> audioList;
    [SerializeField] private AudioSource audioSource;
    private List<AudioClip> tempAudioList;

    [Header("Images")]
    [SerializeField] private Image displayImage;
    [SerializeField] private List<Sprite> images;
    private List<Sprite> tempImageList;
    [SerializeField] private GameObject endScreen;

    private int[] times;

    private bool justStarted = true;

    private bool newStepReached = false;
    private int highestStepReached = -1;

    private Transform[] positions;  // Array delle posizioni
    private int currentIndex = 0;   // Indice della posizione corrente
    private bool pathCompleted = false;
    private Coroutine movementRoutine;

    private bool isPaused = false;
    private Coroutine moveCoroutine;

    private float yRobot;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        yRobot = transform.position.y;

        endScreen.SetActive(false);

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
        float oscillationSpeed = 1f;  // Velocit� dell'oscillazione
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
        if (justStarted)
        {
            positions = new Transform[] { pos0, posX };
            times = new int[] { 12, 11 };
            tempAudioList = new List<AudioClip> { audioList[0], audioList[4] };
            tempImageList = new List<Sprite> { images[0], images[1] };
        }
        else if (highestStepReached == -1 && !justStarted) // Da fare: Mix sostanze
        {
            positions = new Transform[] { posX };
            times = new int[] { 12 };
            tempAudioList = new List<AudioClip> { audioList[4] };
            tempImageList = new List<Sprite> { images[1] };
            displayImage.sprite = tempImageList[0];
        }
        else if (highestStepReached == 0) // Da fare: Aggiunta NaOH
        {
            positions = new Transform[] { posX };
            times = new int[] { 12 };
            tempAudioList = new List<AudioClip> { audioList[5]};
            tempImageList = new List<Sprite> { images[2]};
            displayImage.sprite = tempImageList[0];
        }
        else if (highestStepReached == 1) // Da fare: Mix
        {
            positions = new Transform[] { posX };
            times = new int[] { 13 };
            tempAudioList = new List<AudioClip> { audioList[6]};
            tempImageList = new List<Sprite> { images[3] };
            displayImage.sprite = tempImageList[0];
        }
        else if (highestStepReached == 2) // Da fare: frigo
        {
            positions = new Transform[] { posX };
            times = new int[] { 8 };
            tempAudioList = new List<AudioClip> { audioList[7] };
            tempImageList = new List<Sprite> { images[4] };
            displayImage.sprite = tempImageList[0];
        }
        else if (highestStepReached == 3) // Da fare: Imbuto
        {
            positions = new Transform[] { posX };
            times = new int[] { 7 };
            tempAudioList = new List<AudioClip> { audioList[8]};
            tempImageList = new List<Sprite> { images[5]};
            displayImage.sprite = tempImageList[0];
        }
        else if (highestStepReached == 4) // Da fare: Vetrino e forno
        {
            positions = new Transform[] { posX };
            times = new int[] { 9 };
            tempAudioList = new List<AudioClip> { audioList[9]};
            tempImageList = new List<Sprite> { images[6]};
            displayImage.sprite = tempImageList[0];
        }
        else if (highestStepReached == 5)
        {
            positions = new Transform[] { pos0 };
            times = new int[] { 13 };
            tempAudioList = new List<AudioClip> { audioList[10] };
            tempImageList = new List<Sprite> { null };
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

            // Imposta la destinazione alla posizione corrente
            _navMeshAgent.SetDestination(positions[currentIndex].position);

            // Aspetta finch� l'NPC non � arrivato a destinazione
            while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > 0.1f)
            {
                while (isPaused)
                    yield return null;

                yield return null;
            }

            Debug.Log("Arrivato qui, current index: " + currentIndex);
            Debug.Log("tempAudioList: " + (tempAudioList[currentIndex] == null));


            // Passa alla prossima posizione (ciclica)
            if (tempAudioList[currentIndex] != null)
            {
                Debug.Log("Audio partito");
                StartCoroutine(PlayAudioAndWait(tempAudioList[currentIndex]));
            }

            if (tempImageList[currentIndex] != null)
            {
                displayImage.sprite = tempImageList[currentIndex];
            }

            currentIndex++;

            if (currentIndex != positions.Length)
            {
                float waitTime = times[currentIndex - 1];

                for (float t = 0; t < waitTime; t += Time.deltaTime)
                {
                    if (!isPaused) // Se non � in pausa, continua ad aspettare
                        yield return null;
                    else // Se � in pausa, aspetta finch� la pausa non viene tolta
                        while (isPaused)
                            yield return null;
                }
            }

        }

        if (highestStepReached == 5)
        {
            endScreen.SetActive(true);
        }

        Debug.Log("esce dal while");
        pathCompleted = true;
    }

    private IEnumerator PlayAudioAndWait(AudioClip clip)
    {
        if (clip != null)
        {
            while (audioSource.isPlaying)
            {
                while (isPaused) // Aggiunto controllo pausa
                    yield return null;

                yield return null; // Aspetta un frame
            }

            audioSource.clip = clip;
            audioSource.Play();

            while (audioSource.isPlaying)
            {
                if (isPaused) // Mette in pausa l'audio
                {
                    audioSource.Pause();
                    while (isPaused)
                        yield return null;
                    audioSource.UnPause(); // Riprende l'audio
                }

                yield return null;
            }

            Debug.Log("Riproduzione audio terminata.");
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused; // Inverti lo stato della pausa

        if (isPaused)
        {
            // Metti in pausa l'audio e il movimento
            if (audioSource.isPlaying)
                audioSource.Pause();

            _navMeshAgent.isStopped = true;
        }
        else
        {
            // Riprendi l'audio e il movimento
            if (audioSource.clip != null)
                audioSource.UnPause();

            _navMeshAgent.isStopped = false;
        }
    }
}

