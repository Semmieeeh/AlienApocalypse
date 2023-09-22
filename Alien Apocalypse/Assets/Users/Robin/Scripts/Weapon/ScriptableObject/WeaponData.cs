using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : ScriptableObject
{
    [Header("General")]
    [Space]
    public GameObject prefab;
    [Space]
    public new string name;

    [TextArea(4, 1)]
    [Space]
    public string info;

    [Space]
    [Header("Position")]
    public Vector3 localPlacmentPos;
}
