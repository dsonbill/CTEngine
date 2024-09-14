
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class TouchOrbit : UnityEngine.MonoBehaviour
{
    public UnityEngine.Transform target; // The object to orbit around
    public float speed = 5f; // Rotation speed
    public float distance = 5f; // Distance from the target
    public float minDistance = 1f; // Minimum distance from the target
    public float maxDistance = 10f; // Maximum distance from the target
    public float yLimit = 80f; // Y-axis rotation limit
    public UnityEngine.Vector3 offset; // Offset from the target's position
    public UnityEngine.LayerMask floorLayer; // Layer mask for the floor

    private UnityEngine.Vector3 startTouchPosition;
    private UnityEngine.Vector3 endTouchPosition;
    private UnityEngine.Vector3 currentRotation;
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
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    startTouchPosition = touch.screenPosition;
                }
                else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
                {
                    endTouchPosition = touch.screenPosition;
                    HandleRotation();
                }
                else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
                {
                    // Reset touch positions
                    startTouchPosition = UnityEngine.Vector3.zero;
                    endTouchPosition = UnityEngine.Vector3.zero;
                }
            }
        }

        // Handle zoom
        if ( Touchscreen.current.touches.Count == 2)
        {
            HandleZoom();
        }

        // Apply rotation and zoom
        transform.eulerAngles = currentRotation;
        transform.position = target.position + offset + transform.forward * zoom;

        // Prevent going through the floor
        // Raycast downwards from the camera position
        UnityEngine.RaycastHit hit;
        if (UnityEngine.Physics.Raycast(transform.position, UnityEngine.Vector3.down, out hit, float.MaxValue, floorLayer))
        {
            // Adjust camera position to be above the floor
            transform.position = new UnityEngine.Vector3(transform.position.x, hit.point.y + 1f, transform.position.z);
        }
    }

    // Handle camera rotation
    private void HandleRotation()
    {
        // Calculate delta touch position
        UnityEngine.Vector3 delta = endTouchPosition - startTouchPosition;

        // Apply rotation
        currentRotation.x += delta.y * speed * UnityEngine.Time.deltaTime;
        currentRotation.y += delta.x * speed * UnityEngine.Time.deltaTime;

        // Clamp Y-axis rotation
        currentRotation.x = UnityEngine.Mathf.Clamp(currentRotation.x, -yLimit, yLimit);
    }

    // Handle camera zoom
    private void HandleZoom()
    {
    }
}
