using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLocator : MonoBehaviour
{
    public Transform icon;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (mainCamera == null)
            return;

        Vector3 cameraToIcon = icon.position - mainCamera.transform.position;
        float dotProduct = Vector3.Dot(mainCamera.transform.forward, cameraToIcon);

        if (dotProduct > 0)
        {
            icon.gameObject.SetActive(true);
            icon.transform.position = mainCamera.WorldToScreenPoint(icon.position);
        }
        else
        {
            icon.gameObject.SetActive(false);
        }
    }
}
