using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FishController : MonoBehaviour
{
    public AIState currentState = AIState.Idle;

    public enum AIState
    {
        Idle,
        Roaming,
        Fleeing,
        Eating,
        Entering,
        Exiting
    }

    [SerializeField] private float idleDuration = 2f; // Duration to stand still
    [SerializeField] private float moveRadius = 5f; // The radius the fish can move during its roam state
    public float rotationSpeed = 200f; // Speed at which the fish rotates

    [SerializeField] private float minFleeSpeed = 2f;
    [SerializeField] private float maxFleeSpeed = 10f;
    [SerializeField] private float fleeDistanceThreshold = 10f;
    [SerializeField] private float minFleeDistance = 2f;
    [SerializeField] private float maxFleeDistance = 15f;
    [SerializeField] private float startingSpeed;

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
    }

    void Update()
    {
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
        }
    }

    void HandleIdle()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Reset timer for idle
            timer = idleDuration;
            FindRoamDestinationAndGo();
        }

        // Logic for standing still
        Debug.Log("AI is idle.");
    }

    void HandleRoaming()
    {
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

    void CheckForPlayerClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPosition.z = 0; // Ensure it's in 2D

            float distance = Vector3.Distance(transform.position, clickPosition);

            if (distance <= fleeDistanceThreshold)
            {
                float fleeDistance = Mathf.Lerp(maxFleeDistance, minFleeDistance, distance / fleeDistanceThreshold);
                fleeTarget = transform.position + (transform.position - clickPosition).normalized * (fleeDistance); // Calculate a flee position

                // Set timer based on distance from click position
                timer = Mathf.Clamp(2f - distance / 2, 0.5f, 2f); // Adjust as needed for timing

                currentState = AIState.Fleeing; // Change to fleeing state
            }
        }
    }

    void FindRoamDestinationAndGo()
    {
        Vector2 randomDirection = Random.insideUnitCircle * moveRadius;
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
}