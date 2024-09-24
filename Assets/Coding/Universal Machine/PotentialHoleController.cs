using UnityEngine;

public class PotentialHoleController : MonoBehaviour
{
    public GameObject PotentialHole;
    public Material potentialHoleMaterial; // The material assigned to the PotentialHole object
    public float fluctuationSpeed = 2f; // The speed of the fluctuation
    public float amplitude = 1f; // The maximum value of the fluctuation

    private float currentTime; // Internal time variable

    void Start()
    {
        potentialHoleMaterial = PotentialHole.GetComponent<Renderer>().material;
    }

    void Update()
    {
        currentTime += Time.deltaTime * fluctuationSpeed;
        float distortionFactor = Mathf.Sin(currentTime) * amplitude;

        // Apply the distortion factor to the shader's "_Depth" property
        potentialHoleMaterial.SetFloat("_Distortion", distortionFactor);
    }
}