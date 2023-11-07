using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InformativePopupManager : MonoBehaviour
{
    [SerializeField]
    InformativePopup popup;

    private readonly Queue<InformativePopupData> popups = new ( );

    private void Update ( )
    {
        if ( !popup.Active && popups.Count > 0 )
        {
            popup.Popup (popups.Dequeue ( ));
        }
    }

    public void AddKillPopup ( )
    {
        AddPopup (InformativePopUpType.Kill);
    }

    public void AddRevivePopup ( )
    {
        AddPopup (InformativePopUpType.Revive);
    }

    public void AddOtherPopup ( )
    {
        AddPopup (InformativePopUpType.Other);
    }

    public void AddPopup ( InformativePopUpType type )
    {
        Debug.Log ($"Added kill popup of type {type}");

        var samePopup = popups.SingleOrDefault (p => p.type == type);

        if ( samePopup != null )
        {
            samePopup.amount++;
        }
        else
        {
            popups.Enqueue (new InformativePopupData
            {
                amount = 1,
                type = type,
            });
    
        }
    }
}

