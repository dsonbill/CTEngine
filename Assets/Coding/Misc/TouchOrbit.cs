using UnityEngine;
using UnityEngine.InputSystem;

public class TouchOrbit : MonoBehaviour
{
    public Transform target; // The object to orbit around
    public float speed = 5f; // Rotation speed
    public float distance = 5f; // Distance from the target
    public float minDistance = 1f; // Minimum distance from the target
    public float maxDistance = 10f; // Maximum distance from the target
    public float yLimit = 80f; // Y-axis rotation limit
    public Vector3 offset; // Offset from the target's position
    public LayerMask floorLayer; // Layer mask for the floor

    private Vector2 touchStart;
    private Vector2 touchEnd;
    private Vector3 currentRotation;
    private float zoom;

    void Start()
    {
        // Set initial camera position
        transform.position = target.position + offset + transform.forward * distance;
        zoom = distance;
        currentRotation = transform.eulerAngles;
    }

    void Update()
    {
        // Handle touch input
        if (Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        {
            touchStart = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            touchEnd = Touchscreen.current.primaryTouch.position.ReadValue();
            HandleRotation();
        }
        else if (Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            // Reset touch positions
            touchStart = Vector2.zero;
            touchEnd = Vector2.zero;
        }

        // Handle zoom
        if (Touchscreen.current.touches.Count == 2 && Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            HandleZoom();
        }

        // Apply rotation and zoom
        transform.eulerAngles = currentRotation;
        transform.position = target.position + offset + transform.forward * zoom;

        // Prevent going through the floor
        // Raycast downwards from the camera position
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, float.MaxValue, floorLayer))
        {
            // Adjust camera position to be above the floor
            transform.position = new Vector3(transform.position.x, hit.point.y + 1f, transform.position.z);
        }
    }

    // Handle camera rotation
    private void HandleRotation()
    {
        // Calculate delta touch position
        Vector2 delta = touchEnd - touchStart;

        // Apply rotation
        currentRotation.x += delta.y * speed * Time.deltaTime;
        currentRotation.y += delta.x * speed * Time.deltaTime;

        // Clamp Y-axis rotation
        currentRotation.x = Mathf.Clamp(currentRotation.x, -yLimit, yLimit);

        // Reset touch start position
        touchStart = touchEnd;
    }

    // Handle camera zoom
    private void HandleZoom()
    {
        // Calculate distance between touch points
        Vector2 touch0 = Touchscreen.current.touches[0].position.ReadValue();
        Vector2 touch1 = Touchscreen.current.touches[1].position.ReadValue();
        float distanceBetweenTouches = Vector2.Distance(touch0, touch1);

        // Adjust zoom based on touch distance
        zoom = Mathf.Clamp(zoom - (distanceBetweenTouches - zoom) * speed * Time.deltaTime, minDistance, maxDistance);
    }
}
