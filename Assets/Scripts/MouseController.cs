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

    private LineRenderer lineRenderer;

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
        DrawRing();
        
    }

    void DrawRing()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane; // Set the distance from the camera
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
}
