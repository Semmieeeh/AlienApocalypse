using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class JetpackParticle : MonoBehaviour
{
    [SerializeField]
    VisualEffect[] effects;

    private bool m_Active;

    public bool Active
    {
        get
        {
            return m_Active;
        }
        set
        {
            if ( value == m_Active )
                return;
            m_Active = value;

            OnStateChanged ( );
        }
    }

    void OnStateChanged ( )
    {
        foreach ( var effect in effects )
        {
            effect.SetBool ("Active", Active);
        }
    }
}
