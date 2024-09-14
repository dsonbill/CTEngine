using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

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

    private Vector3 startTouchPosition;
    private Vector3 endTouchPosition;
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
        if (EnhancedTouchSupport.enabled)
        {
            foreach (Touch touch in Touch.activeTouches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    startTouchPosition = touch.screenPosition;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    endTouchPosition = touch.screenPosition;
                    HandleRotation();
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    // Reset touch positions
                    startTouchPosition = Vector3.zero;
                    endTouchPosition = Vector3.zero;
                }
            }
        }

        // Handle zoom
        if (Touchscreen.current.primaryTouch.phase == TouchPhase.Moved && Touchscreen.current.touches.Count == 2)
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
        Vector3 delta = endTouchPosition - startTouchPosition;

        // Apply rotation
        currentRotation.x += delta.y * speed * Time.deltaTime;
        currentRotation.y += delta.x * speed * Time.deltaTime;

        // Clamp Y-axis rotation
        currentRotation.x = Mathf.Clamp(currentRotation.x, -yLimit, yLimit);
    }

    // Handle camera zoom
    private void HandleZoom()
    {
        // Calculate distance between touch points
        Vector2 touch0 = Touchscreen.current.touches[0].screenPosition;
        Vector2 touch1 = Touchscreen.current.touches[1].screenPosition;
        float distanceBetweenTouches = Vector2.Distance(touch0, touch1);

        // Adjust zoom based on touch distance
        zoom = Mathf.Clamp(zoom - (distanceBetweenTouches - zoom) * speed * Time.deltaTime, minDistance, maxDistance);
    }
}
