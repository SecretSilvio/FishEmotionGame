using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class FishController : MonoBehaviour
{
    [Header("AI Settings")]
    public AIState currentState = AIState.Idle;

    public enum AIState
    {
        Idle,
        Roaming,
        Fleeing,
        Baited,
        Eating,
        Entering,
        Exiting
    }

    [Header("Roaming Settings")]
    [SerializeField] private float idleDuration = 2f; // Duration to stand still
    [SerializeField] private float moveRadius = 5f; // The radius the fish can move during its roam state
    public float rotationSpeed = 200f; // Speed at which the fish rotates

    [Header("Fleeing Settings")]
    [SerializeField] private float minFleeSpeed = 2f;
    [SerializeField] private float maxFleeSpeed = 10f;
    [SerializeField] private float fleeDistanceThreshold;
    [SerializeField] private float minFleeDistance = 2f;
    [SerializeField] private float maxFleeDistance = 15f;

    public bool CanBeBaited { get; set; }
    [Header("Baited Settings")]
    [SerializeField] private float baitedDuration = 2f;
    [SerializeField] private float baitedSpeed = 10f;
    [SerializeField] private float baitedDistance = 5f;

    [SerializeField] private float startingSpeed;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private FishSpawner fishSpawner;
    [SerializeField] private GameObject circle;
    [SerializeField] private MouseController mc;
    [SerializeField] private bool inScareMode;

    private float timer;
    private Vector3 roamDestination;
    private Vector3 fleeTarget;

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        startingSpeed = navMeshAgent.speed;
        timer = idleDuration;
        navMeshAgent.speed = 3.5f; // Adjust speed as needed

        mc = GameObject.Find("MouseManager").GetComponent<MouseController>();
        fleeDistanceThreshold = mc.radius;
        CanBeBaited = false;
        // set up button using textmeshpro
        circle = GameObject.Find("Circle");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        fishSpawner = GameObject.Find("Spawner").GetComponent<FishSpawner>();
    }

    void Update()
    {
        CheckForModeSwitch();
        CheckForPlayerClick();
        HandleState();
    }

    void HandleState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                HandleIdle();
                break;
            case AIState.Roaming:
                HandleRoaming();
                break;
            case AIState.Fleeing:
                HandleFleeing();
                break;
            case AIState.Baited:
                HandleBaited();
                break;
            case AIState.Eating:
                HandleEating();
                break;
            case AIState.Entering:
                HandleEntering();
                break;
            case AIState.Exiting:
                HandleExiting();
                break;    
        }
    }

    void HandleIdle()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Reset timer for idle
            timer = idleDuration + Random.Range(-2,2);
            FindRoamDestinationAndGo();
        }

        // Logic for standing still
        Debug.Log("AI is idle.");
    }

    void HandleRoaming()
    {
        navMeshAgent.speed = startingSpeed;
        SmoothRotateTowardsDestination();
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            currentState = AIState.Idle;
        }
       Debug.Log("AI is roaming.");
        
    }

    void HandleFleeing()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(fleeTarget); // Set the flee target position
            Debug.Log("AI is fleeing.");

            // Adjust speed based on remaining time
            float fleeSpeed = Mathf.Lerp(minFleeSpeed, maxFleeSpeed, timer / idleDuration);
            navMeshAgent.speed = fleeSpeed;
        }

        // Transition back to Idle after a certain time
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            currentState = AIState.Idle;
            timer = idleDuration;
            navMeshAgent.ResetPath(); // Stop moving
        }

        SmoothRotateTowardsDestination();
    }

    void HandleBaited()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(fleeTarget); // Set the flee target position
            Debug.Log("AI is baited.");

            // Adjust speed based on remaining time
        }

        // Transition to Eating after you reach bait target

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            currentState = AIState.Eating;
        }

        SmoothRotateTowardsDestination();
    }

    void HandleEating()
    {
        Debug.Log("AI is eating.");
        currentState = AIState.Idle;
    }

    void HandleEntering()
    {
        Debug.Log("AI is entering.");
    }

    void HandleExiting()
    {
        Debug.Log("AI is exiting.");
    }

    void CheckForModeSwitch()
    {
        if (mc.currentAction == MouseController.PlayerActions.Scare)
        {
            inScareMode = true;
        }
        else
        {
            inScareMode = false;
        }
    }

    void CheckForPlayerClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPosition.z = 0; // Ensure it's in 2D

            float distance = Vector3.Distance(transform.position, clickPosition);

            if (inScareMode)
            {
                if (distance <= fleeDistanceThreshold)
                {
                    float fleeDistance = Mathf.Lerp(maxFleeDistance, minFleeDistance, distance / fleeDistanceThreshold + 3);
                    fleeTarget = transform.position + (transform.position - clickPosition).normalized * (fleeDistance); // Calculate a flee position

                    // Set timer based on distance from click position
                    timer = Mathf.Clamp(2f - distance / 2, 0.5f, 2f); // Adjust as needed for timing

                    currentState = AIState.Fleeing; // Change to fleeing state
                }
            }
            else
            {
                if (distance <= baitedDistance && CanBeBaited == true)
                {
                    fleeTarget = clickPosition; // Set the bait position

                    // Set timer for baited duration
                    timer = baitedDuration;

                    currentState = AIState.Baited; // Change to baited state
                }

            }

        }
    }

    void FindRoamDestinationAndGo()
    {
        Vector2 randomDirection = new Vector2(this.transform.position.x, this.transform.position.y) + (Random.insideUnitCircle * moveRadius);
        roamDestination = new Vector3(randomDirection.x, randomDirection.y, 0);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(roamDestination, out hit, moveRadius, NavMesh.AllAreas))
        {
            currentState = AIState.Roaming;
            navMeshAgent.SetDestination(hit.position); // Move the fish to the random spot
        }
        else
        {
            // If the point is not valid, try again
            FindRoamDestinationAndGo();
        }
    }

    void SmoothRotateTowardsDestination()
    {
        // Get the direction to the destination
        Vector2 direction = (navMeshAgent.steeringTarget - transform.position).normalized;

        // Calculate the angle in degrees
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Get the current rotation (z-axis only)
        float currentAngle = transform.eulerAngles.z;

        // Smoothly rotate towards the target angle
        float smoothAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        // Apply the smooth rotation to the fish's transform
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, smoothAngle));
    }

    public void EnterCircle()
    {
        currentState = AIState.Entering;
    }

    public void ExitCircle()
    {
        currentState = AIState.Exiting;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Circle"))
        {
            EnterCircle();
            Debug.Log("Fish Entered Circle!");
        }
        if (other.gameObject.CompareTag("Kill"))
        {
            fishSpawner.fishDied();
            Destroy(gameObject);
            Debug.Log("Fish Killed!");
        }
    }
}
