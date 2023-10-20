using UnityEngine;

public class UITrackable : MonoBehaviour
{
    [SerializeField]
    private Transform m_target;

    [SerializeField]
    Vector3 m_targetPos;

    [SerializeField]
    protected float smoothTime;

    public Vector3 TargetPos
    {
        get
        {
            if ( useTransform && m_target != null )
            {
                return m_target.position;
            }

            return m_targetPos;
        }
    }

    bool useTransform;

    protected virtual void Update ( )
    {
        UpdateMarker ( );
    }
    private void UpdateMarker ( )
    {
        // Calculate the screen position of the world center
        Vector3 screenPosition = Camera.main.WorldToScreenPoint (TargetPos);

        // Check if the position is in front of the camera
        if ( Vector3.Dot (( TargetPos - Utilities.Camera.transform.position ).normalized, Utilities.Camera.transform.forward) > 0 )
        {
            // Set the UI element's anchored position to the screen position
            transform.position = screenPosition;
        }
        else
        {
            // If the position is behind the camera, set it outside the screen
            transform.position = new Vector3 (-1000, -1000, 0);
        }
    }

    public void SetTarget (Transform target )
    {
        useTransform = true;
        m_target = target;
    }

    public void SetTarget(Vector3 targetPos )
    {
        useTransform = false;
        m_targetPos = targetPos;
    }

}
