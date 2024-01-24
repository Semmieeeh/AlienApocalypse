using UnityEngine;
using UnityEngine.UI;   
public class UITrackable : MonoBehaviour
{
    [Header ("Tracking setttings")]
    [SerializeField]
    private Transform m_target;

    [SerializeField]
    Vector3 m_targetPos;

    [SerializeField]
    protected float smoothTime;

    public float alphaDst = 100;

    bool alphaActive;

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

    bool useTransform = true;

    protected virtual void Update ( )
    {
        UpdateMarker ( );

        CheckTransparency ( );
    }
    private void UpdateMarker ( )
    {
        // Check if the position is in front of the camera

        if ( Utilities.Camera != null && Vector3.Dot (( TargetPos - Utilities.Camera.transform.position ).normalized, Utilities.Camera.transform.forward) > 0 )
        {
            // Set the UI element's anchored position to the screen position
            transform.position = Utilities.Camera.WorldToScreenPoint(TargetPos);
        }
        else
        {
            // If the position is behind the camera, set it outside the screen
            transform.position = new Vector3 (-1000, -1000, 0);
        }
    }

    protected void CheckTransparency ( )
    {
        Vector2 centre = new (Screen.width / 2, Screen.height / 2);

        Vector2 pos = transform.position;

        float dst = Vector2.Distance (centre, pos);

        if ( dst <= alphaDst)
        {
            if ( alphaActive == true )
                return;

                SetAlpha (0.3f);
            alphaActive = true;
        }
        else if ( alphaActive == true )
        {
            SetAlpha (1);
            alphaActive = false;
        }

        void SetAlpha ( float alpha )
        {
            var graphics = transform.GetComponentsInHierarchy<Graphic> ( );

            foreach ( var g in graphics )
            {
                var c = g.color;
                c.a = alpha;
                g.color = c;
            }
        }
    }

    public void SetTarget ( Transform target )
    {
        useTransform = true;
        m_target = target;
    }

    public void SetTarget ( Vector3 targetPos )
    {
        useTransform = false;
        m_targetPos = targetPos;
    }

}
