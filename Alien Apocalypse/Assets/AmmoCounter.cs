using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCounter : MonoBehaviour
{
    public static AmmoCounter Instance;


    [Header ("References")]
    [SerializeField]
    TextMeshProUGUI currentAmmoText;

    [SerializeField]
    TextMeshProUGUI maxAmmoText;

    [SerializeField]
    Image weaponImage;

    [SerializeField]
    Transform fireRateParent;

    [Header ("Other")]
    [SerializeField]
    float scaleSpeed = 0.1f;

    [SerializeField]
    Color fullClipColor, emptyClipColor;

    [SerializeField]
    float reloadPulseSpeed = 2;

    Vector3 ammoCountScaleVelocity;

    Color reloadingColor;

    bool reloading;

    private FirearmData weaponData;

    private Firearm actualWeapon;

    private void Awake ( )
    {
        Instance = this;
    }

    private void Update ( )
    {
        UpdateCounterSize ( );

        if(reloading)
            UpdateReloadingAnimation ( );
    }


    void OnShoot ( )
    {
        currentAmmoText.transform.localScale = Vector3.one * 1.2f;

        //TODO: Play UI Shoot effect

        UpdateAmmoData ( );
    }

    void UpdateAmmoData ( )
    {
        weaponImage.sprite = weaponData.weaponSprite;

        currentAmmoText.text = actualWeapon.currentAmmo.ToString ( );

        maxAmmoText.text = actualWeapon.maxAmmo.ToString ( );

        var c = Color.Lerp (emptyClipColor, fullClipColor, Mathf.InverseLerp (0, actualWeapon.maxAmmo, actualWeapon.currentAmmo));

        currentAmmoText.color = c;
    }

    void UpdateCounterSize ( )
    {
        var s = currentAmmoText.transform.localScale;

        s = Vector3.SmoothDamp (s, Vector3.one, ref ammoCountScaleVelocity, scaleSpeed);

        currentAmmoText.transform.localScale = s;
    }

    void UpdateReloadingAnimation ( )
    {
        float sin = (Mathf.Sin (Time.time * reloadPulseSpeed) + 1) / 2;

        Color c = new ( )
        {
            r = reloadingColor.r * 0.5f,
            g = reloadingColor.g * 0.5f,
            b = reloadingColor.b * 0.5f,
            a = 1,
        };

        currentAmmoText.color = Color.Lerp (reloadingColor, c, sin);
    }

    void OnReloadStart ( )
    {
        reloading = true;
        reloadingColor = currentAmmoText.color;
    }

    void OnReloadEnd ( )
    {
        reloading = false;

        UpdateAmmoData ( );
    }
    void RemoveWeaponDelegates ( )
    {
        if ( weaponData ) // Remove previous weapon delegate subscriptions
        {
            actualWeapon.events.onShooting.RemoveListener (OnShoot);

            actualWeapon.events.onStartReloading.RemoveListener (OnReloadStart);
            actualWeapon.events.onEndReloading.RemoveListener (OnReloadEnd);
        }


    }
    void ApplyWeaponDelegates ( )
    {
        actualWeapon.events.onShooting.AddListener (OnShoot);

        actualWeapon.events.onStartReloading.AddListener (OnReloadStart);
        actualWeapon.events.onEndReloading.AddListener (OnReloadEnd);

    }

    public void SetWeaponData(FirearmData weaponData, Firearm actualWeapon )
    {
        gameObject.SetActive (true);

        RemoveWeaponDelegates ( );

        this.weaponData = weaponData;

        this.actualWeapon = actualWeapon;

        //Apply new weapon delegate subscriptions

        ApplyWeaponDelegates ( );


        weaponImage.sprite = weaponData.weaponSprite;


        //Apply firerate graphics
        int fireRate = weaponData.fireType switch
        {
            FirearmData.Firetype.singleShot => 1,
            FirearmData.Firetype.projectile => 1,
            FirearmData.Firetype.shotgun => 1,

            FirearmData.Firetype.burst => 2,

            FirearmData.Firetype.automatic => 3,
            _ => 1
        };

        for ( int i = 0; i < fireRateParent.childCount; i++ )
        {
            fireRateParent.GetChild (i).gameObject.SetActive (i < fireRate);
        }

        UpdateAmmoData( );
    }

    public void SetNoWeapon ( )
    {
        RemoveWeaponDelegates ( );

        weaponData = null;
        actualWeapon = null;

        gameObject.SetActive (false);
    }
}
