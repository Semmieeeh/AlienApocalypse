using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformativePopupManager : MonoBehaviour
{
    [SerializeField]
    InformativePopup popup;

    private Queue<InformativePopUpType> popups = new();

    private void Update()
    {
        if (!popup.Active && popups.Count > 0)
        {
            popup.Popup(popups.Dequeue());
        }
    }

    public void AddKillPopup()
    {
        AddPopup(InformativePopUpType.Kill);
    }

    public void AddRevivePopup()
    {
        AddPopup(InformativePopUpType.Revive);
    }

    public void AddOtherPopup()
    {
        AddPopup(InformativePopUpType.Other);
    }

    public void AddPopup(InformativePopUpType type)
    {
        popups.Enqueue(type);
    }
}
