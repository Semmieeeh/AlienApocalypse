using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrackable : MonoBehaviour
{

    private void OnEnable ( )
    {
        MiniMap.Instance.AddTrackable (this);
    }

    private void OnDisable ( )
    {
        MiniMap.Instance.RemoveTrackable (this);

    }
    [SerializeField]
    bool useRotation;

    [SerializeField]
    bool pointWhenOutOfBounds;

    [SerializeField]
    Color color = Color.white;

    [SerializeField]
    Sprite uiImage;

    public Vector3 Position {
        get => transform.position;
    }

    public Vector3 EulerAngles
    {
        get => transform.localEulerAngles;
    }

    public Quaternion Rotation
    {
        get => transform.rotation;
    }

    public bool UseRotation => useRotation;

    public bool PointWhenOutOfBounds => pointWhenOutOfBounds;
    public Color Color => color;

    public Sprite UIImage => uiImage;


}
