using UnityEngine;

[ExecuteAlways]
public class TopDownCamera : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 1f;

    [Header("Camera Settings")]
    public float height = 20f; // How high above the grid
    public bool orthographic = true;
    public float padding = 2f; // Extra space around grid

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        // Center camera above grid
        float centerX = (gridWidth * cellSize) / 2f - cellSize / 2f;
        float centerZ = (gridHeight * cellSize) / 2f - cellSize / 2f;
        transform.position = new Vector3(centerX, height, centerZ);

        // Point straight down
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Set orthographic and size
        cam.orthographic = orthographic;
        if (orthographic)
        {
            cam.orthographicSize = Mathf.Max(gridWidth, gridHeight) / 2f + padding;
        }
    }
}
