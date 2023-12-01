using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitIndicatorManager : MonoBehaviour
{
    public static HitIndicatorManager Instance;
    [SerializeField]
    Transform hitParent;

    [SerializeField]
    GameObject indicatorPrefab;


    public List<HitIndicator> hitIndicators { get; private set; } = new List<HitIndicator>();

    private void Awake ( )
    {
        Instance = this;
    }
    public void AddTarget(Transform target )
    {
        foreach ( var indicator in hitIndicators )
        {
            if(indicator.GetTarget() == target )
            {
                indicator.StartIndicator ( );
                return;
            }
        }

        var newIndicator = Instantiate (indicatorPrefab, hitParent).GetComponent<HitIndicator> ( );

        if ( newIndicator )
        {
            newIndicator.StartIndicator (target);
        }

    }
}
