using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Camera mainCam;
    public GameObject recoil;
    public PhotonView pv;

    [Header("General")]
    [Space]
    public new string name;

    [TextArea(4, 1)]
    [Space]
    public string info;

    [Space]
    [Header("Position")]
    public Vector3 localPlacmentPos;

    public enum FirearmType
    {
        handgun,
        shotgun,
        assaultRifle
    }

    public Vector3 GetLocalPlacmentPos() => localPlacmentPos;

    public virtual void StartWeapon() { }

    public virtual void UpdateWeapon() { }

    public virtual void Shooting() { }

    public virtual void Meeling() { }


    public virtual IEnumerator Reloading()
    {
        yield return null;
    }

    public virtual void OnButtonUp() { }

    public virtual void Sway(Vector2 mouseInput) { }
}
