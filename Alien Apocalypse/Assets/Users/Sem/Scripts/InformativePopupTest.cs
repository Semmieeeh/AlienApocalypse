using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformativePopupTest : MonoBehaviour
{
    [SerializeField]
    InformativePopupManager manager;

    public InformativePopUpType type;

    public bool test;

    void Update()
    {
        if (test)
        {
            test = false;
            manager.AddPopup(type);
        }
    }
}
