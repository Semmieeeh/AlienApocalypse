using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLocator : MonoBehaviour
{
    public RectTransform uiElement;
    public Camera worldCamera;

    private Vector3 worldCenterPosition;

    private void Start()
    {
        // Initialize the world center position to (0, 0, 0)
        worldCenterPosition = Vector3.zero;
        worldCamera = Camera.main;
    }

    private void Update()
    {
        // Ensure that the UI element and world camera are assigned
        if (uiElement == null)
        {
            Debug.LogError("UI element or world camera not assigned.");
            return;
        }

        // Calculate the screen position of the world center
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldCenterPosition);

        // Check if the position is in front of the camera
        if (Vector3.Dot((worldCenterPosition - worldCamera.transform.position).normalized, worldCamera.transform.forward) > 0)
        {
            // Set the UI element's anchored position to the screen position
            uiElement.position = screenPosition;
        }
        else
        {
            // If the position is behind the camera, set it outside the screen
            uiElement.position = new Vector3(-1000, -1000, 0);
        }
    }
}
