using UnityEngine;

public class ScaleFixer : MonoBehaviour
{
    //Get desired scale multiplier
    public float ScaleMultiplier = 1f;



    void Start()
    {


        // Get the RectTransform component
        RectTransform rectTransform = GetComponent<RectTransform>();

        // Check if the RectTransform is assigned
        if (rectTransform != null)
        {
            // Get the current scale
            Vector3 currentScale = rectTransform.localScale;

            // Set the new scale
            currentScale = Vector3.one * ScaleMultiplier;
            rectTransform.localScale = currentScale;
        }
        else
        {
            //Log the error if the RectTransform is not assigned
            Debug.LogError("RectTransform not assigned!");
        }
    }
}