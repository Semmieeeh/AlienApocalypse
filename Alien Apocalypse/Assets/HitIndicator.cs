using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

public class HitIndicator : MonoBehaviour
{
    [SerializeField]
    [Range (0, 1)]
    float progress;

    [SerializeField]
    Animator animator;

    [SerializeField]
    RawImage targetGraphic;

    [SerializeField]
    Transform target;

    public float timeToDestroy;
    float lifeTime;

    bool inited;

    public bool IsDead => lifeTime >= timeToDestroy;

    public void StartIndicator ( ) => StartIndicator (target);
    public void StartIndicator ( Transform target )
    {
        if ( !inited )
        {
            inited = true;
            //targetGraphic.material = new (targetGraphic.material);
        }

        this.target = target;

        animator.SetTrigger ("Active");

        lifeTime = 0;
    }

    private void Update ( )
    {
        lifeTime += Time.deltaTime;

        if ( lifeTime >= timeToDestroy )
        {
            Delete ( );
            return;
        }

        if ( target == null )
            return;


        var tRot = target.rotation;

        var direction = Utilities.Camera.transform.position - target.position;

        tRot = Quaternion.LookRotation (-direction);
        tRot.z = -tRot.y;
        tRot.x = 0;
        tRot.y = 0;

        Vector3 northDirection = new (0,0, Utilities.Camera.transform.eulerAngles.y );

        transform.localRotation = tRot * Quaternion.Euler ( northDirection);
    }

    void Delete ( )
    {
        Destroy (gameObject);
        HitIndicatorManager.Instance.hitIndicators.Remove ( this );
    }

    public Transform GetTarget ( ) => target;
}
