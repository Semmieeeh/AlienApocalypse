using UnityEngine;

public class GrappleRope : MonoBehaviour
{
    private Spring spring;
    private LineRenderer lr;
    private Vector3 currentGrapplePosition;
    public Grappling grapplingGun;
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve affectCurve;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.SetTarget(0);
    }

    public float time;
    float cooldown = 0.1f;
    bool executed;
    bool cleared;
    private void Update()
    {
        
        time += Time.deltaTime;
        if (grapplingGun.isGrappling)
        {
            Vector3 grapplePoint = grapplingGun.GetGrapplePoint();
            DrawRope(grapplePoint);
            if (time >= cooldown)
            {
                DrawRope(grapplePoint);
            }
            executed = true;
            cleared = false;
        }
        else
        {
            ClearRope();
            cleared = true;
            executed = false;
        }



    }

    public void DrawRope(Vector3 grapplePoint)
    {
        // Handle rope drawing based on grapplePoint
        UpdateRope(grapplePoint);
    }


    // Implement the UpdateRope and ClearRope methods to update the LineRenderer appropriately
    private void UpdateRope(Vector3 grapplePoint)
    {
        if (lr == null)
        {
            return;
        }
        if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var gunTipPosition = grapplingGun.gunTip.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

        for (var i = 0; i < quality + 1; i++)
        {
            var delta = i / (float)quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                         affectCurve.Evaluate(delta);

            if (i < lr.positionCount)
            {
                lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
            }
            else
            {
                Debug.LogWarning("Attempted to set LineRenderer position outside of bounds.");
            }
        }
    }

    private void ClearRope()
    {
        currentGrapplePosition = grapplingGun.gunTip.position;
        if (spring == null || lr == null)
        {
            return;
        }
        if (spring != null)
        {
            spring.Reset();
        }
        if (lr.positionCount > 0)
        {
            lr.positionCount = 0;
        }
    }
}