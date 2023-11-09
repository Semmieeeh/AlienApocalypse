using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPowerupManager : MonoBehaviour
{
    private static UIPowerupManager instance;

    public static UIPowerupManager Instance
    {
        get
        {
            if ( instance == null )
                instance = FindObjectOfType<UIPowerupManager> ( true);
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    List<UIPowerup> activeAbilities = new ( );

    public GameObject uiPrefab;

    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void OnPickupPowerup (FirearmAbility ability )
    {
        var powerup = activeAbilities.SingleOrDefault (ab => ab.currentAbility == ability);

        if ( powerup != null && powerup.currentAbility.Equals(ability))
        {
            powerup.Blink ( );
            return;
        }

        var obj = Instantiate (uiPrefab, parent).GetComponent<UIPowerup> ( );

        if ( obj )
        {
            obj.Initialize (ability);
        }

        activeAbilities.Add (obj);

    }
}
