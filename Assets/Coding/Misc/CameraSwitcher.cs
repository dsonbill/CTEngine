using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera1; // First camera
    public Camera camera2; // Second camera
    public Button switchButton; // The button to switch cameras

    private bool usingCamera1 = true; // Flag to track the current camera

    void Start()
    {
        // Set the initial active camera
        SetCameraActive(usingCamera1);

        // Add a listener to the switch button
        switchButton.onClick.AddListener(SwitchCamera);
    }

    // Switch the active camera
    public void SwitchCamera()
    {
        // Toggle the camera flag
        usingCamera1 = !usingCamera1;

        // Set the active camera
        SetCameraActive(usingCamera1);
    }

    // Enable/Disable cameras based on the flag
    private void SetCameraActive(bool useCamera1)
    {
        if (useCamera1)
        {
            camera1.gameObject.SetActive(true);
            camera2.gameObject.SetActive(false);
        }
        else
        {
            camera1.gameObject.SetActive(false);
            camera2.gameObject.SetActive(true);
        }
    }
}
