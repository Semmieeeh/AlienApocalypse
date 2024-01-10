using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Camera mainCam;
    public GameObject recoilObject;

    public Vector3 GetLocalPlacmentPos() => Vector3.zero;

    public virtual void StartWeapon() { }

    public virtual void UpdateWeapon() { }

    public virtual void Shooting() { }

    public virtual void Meeling() { }

    public virtual IEnumerator Reloading()
    {
        yield return null;
    }

    public virtual void OnButtonUp() { }

    public virtual Vector3 Sway(Vector3 pos) => Vector3.zero;
}
