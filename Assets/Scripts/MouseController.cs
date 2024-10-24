using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MouseController : MonoBehaviour
{
    [Header("Ring Settings")]
    public float radius = 1.0f; // Radius of the ring
    public int segments = 36; // Number of segments in the ring
    public Color ringColor = Color.green; // Color of the ring

    [Header("Dotted Line Settings")]
    public GameObject fishPrefab;
    public int dotsCount = 20; // Number of dots in the line
    public float dotSpacing = 0.1f; // Spacing between dots
    private GameObject lastFish; // Track the last fish that was baited

    private LineRenderer lineRenderer;

    // Settings for toggling current player action
    public PlayerActions currentAction = PlayerActions.Scare;
    public enum PlayerActions
    {
        Bait,
        Scare,
    }

    

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.startColor = ringColor;
        lineRenderer.endColor = ringColor;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (currentAction == PlayerActions.Scare)
            {
                currentAction = PlayerActions.Bait;
            }
            else
            {
                currentAction = PlayerActions.Scare;
            }
        }

        if (currentAction == PlayerActions.Scare)
        {
            DrawRing();
        }
        else
        {
            DrawBait();
        }

    }

    void DrawRing()
    {
        lineRenderer.positionCount = segments + 1;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // Update ring positions
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0) + worldMousePos);
        }
    }

    void DrawBait()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Set Z to 0 for 2D

        // Find the nearest fish
        GameObject nearestFish = FindNearestFish(mousePos);

        if (nearestFish != null)
        {
            FishController fishController = nearestFish.GetComponent<FishController>();
            if (fishController != null)
            {
                // If a new fish is found, turn off CanBeBaited for the last fish
                if (lastFish != nearestFish)
                {
                    ToggleCanBeBaited(lastFish, false); // Turn off last fish
                    lastFish = nearestFish; // Update last fish
                }

                // Turn on CanBeBaited for the current nearest fish
                ToggleCanBeBaited(nearestFish, true);
            }

            // Draw the dotted line
            DrawDottedLine(mousePos, nearestFish.transform.position);
        }
        else
        {
            lineRenderer.positionCount = 0; // Hide line if no fish is found
        }
    }

    GameObject FindNearestFish(Vector3 mousePos)
    {
        GameObject[] fish = GameObject.FindGameObjectsWithTag("Fish"); // Assuming fish are tagged as "Fish"
        GameObject nearestFish = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject fishObject in fish)
        {
            float distance = Vector3.Distance(mousePos, fishObject.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestFish = fishObject;
            }
        }

        return nearestFish;
    }

    void DrawDottedLine(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        int dotsToDraw = Mathf.CeilToInt(distance / dotSpacing);

        lineRenderer.positionCount = dotsToDraw + 1; // +1 to include the end point

        for (int i = 0; i <= dotsToDraw; i++)
        {
            float t = (float)i / dotsToDraw;
            Vector3 point = start + direction * t * distance;
            lineRenderer.SetPosition(i, point);
        }
    }

    void ToggleCanBeBaited(GameObject fish, bool value)
    {
        if (fish != null)
        {
            FishController fishController = fish.GetComponent<FishController>();
            if (fishController != null)
            {
                fishController.CanBeBaited = value; // Set the CanBeBaited property
            }
        }
    }

}
