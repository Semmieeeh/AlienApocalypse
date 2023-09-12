using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("General")]
    [Space]
    public new string name;

    [TextArea(4,1)]
    [Space]
    public string info;

    public virtual void Shooting() { }

    public virtual void Meeling() { }

    public virtual void Reloading() { }
}
