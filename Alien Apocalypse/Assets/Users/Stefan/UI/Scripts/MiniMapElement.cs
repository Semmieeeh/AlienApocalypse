using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapElement : MonoBehaviour
{
    public Image image;

    public void Initialize ( MapTrackable trackable)
    {
        image.sprite = trackable.UIImage;
        image.color = trackable.Color;
    }

    public void SetPositionAndRotation(Vector3 position, Vector3 eulerAngles )
    {
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }

    public void Remove ( )
    {
        Destroy (gameObject);
    }
}
