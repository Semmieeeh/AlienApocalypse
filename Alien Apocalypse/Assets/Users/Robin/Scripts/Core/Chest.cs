using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class Chest : MonoBehaviourPunCallbacks, IInteractable
{
    /*
     //////* 0 => CrosshairEffects,
       //////1 => Fov,
       //////2 => HorizontalSens,
       //////3 => VerticalSens,
       //////4 => ScreenResIndex,
       //////5 => FpsIndex,
       //////6 => QualityIndex,
       //////7 => Fullscreen,
       //////8 => VSync,
       //////9 => MainAudioStrength,
       //////10 => SoundsStrength,
       //////11 => MusicStrength,
       //////12 => UIStrength,
       //////13 => crosshairIndex,
       //////14 => crosshairSize,
       //////_ => throw new 
     * 
     * 
     * 
     * 
     */
    public ChestManager chestManager;

    [Space]
    [Header("Spawn Chances")]
    public float minFirearmChance;
    public float maxFirearmChance;
    public float minFirearmAbilityChance;
    public float maxFirearmAbilityChance;
    public float minBombChance;
    public float maxBombChance;

    [Header("Prefabs")]
    public GameObject[] firearmDatas;
    public GameObject[] firearmAbilities;

    public GameObject chestBeacon;
    public Transform goToPoint;
    public float force;
    public bool opened = false;

    [Header("Bomb")]
    public float bombDamage;
    public GameObject bomb;
    public GameObject explosion;
    public float bombRadius;
    public LayerMask playerMask;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip countDownClip;
    public AudioClip explosionClip;

    public void Interact(WeaponInputHandler handler)
    {
        if(!opened)
        {
            photonView.RPC("OpenChest", RpcTarget.All);
        }
    }

    [PunRPC]
    void OpenChest()
    {
        chestBeacon.SetActive(false);
        chestManager.New();

        GetComponent<Animator>().SetTrigger("Open");
        float value = Random.value;

        if(value >= minFirearmChance / 100 && value <= maxFirearmChance / 100)
        {
            int n = Random.Range(0, firearmDatas.Length - 1);

            for(int i = 0; i < firearmDatas.Length; i++)
            {
                if(n == i)
                {
                    GameObject gun = PhotonNetwork.Instantiate(firearmDatas[i].name, goToPoint.position, Quaternion.identity);

                    Rigidbody rb = gun.GetComponent<Rigidbody>();
                    rb.AddForce(goToPoint.forward * force, ForceMode.Impulse);
                    Vector3 torque = new Vector3(Random.Range(0, 0), Random.Range(0, 0), Random.Range(-2f, 2f));
                    rb.AddTorque(torque * 5);

                    opened = true;

                    return;
                }
            }
        }
        else if(value >= minFirearmAbilityChance / 100 && value <= maxFirearmAbilityChance / 100)
        {
            int n = Random.Range(0, firearmAbilities.Length - 1);

            for(int i = 0; i < firearmAbilities.Length; i++)
            {
                if(n == i)
                {
                    GameObject holder = PhotonNetwork.Instantiate(firearmAbilities[n].name, goToPoint.position, Quaternion.identity);

                    Rigidbody rb = holder.GetComponent<Rigidbody>();
                    rb.AddForce(goToPoint.forward * force, ForceMode.Impulse);
                    Vector3 torque = new Vector3(Random.Range(0, 0), Random.Range(0, 0), Random.Range(-2f, 2f));
                    rb.AddTorque(torque * 5);

                    opened = true;

                    return;
                    
                }
            }
        }
        else if(value >= minBombChance / 100 && value <= maxBombChance / 100)
        {
            StartCoroutine(Bomb());

            opened = true;

            return;
        }
    }

    public Collider[] testCol; 

    IEnumerator Bomb()
    {
        GameObject currentBomb = PhotonNetwork.Instantiate(bomb.name, goToPoint.position, Quaternion.identity);

        Rigidbody rb = currentBomb.GetComponent<Rigidbody>();
        rb.AddForce(goToPoint.forward * force, ForceMode.Impulse);
        Vector3 torque = new Vector3(Random.Range(0, 0), Random.Range(0, 0), Random.Range(-2f, 2f));
        rb.AddTorque(torque * 5);
        
        if(countDownClip != null)
        {
            audioSource.clip = countDownClip;
            audioSource.Play();

            yield return new WaitForSeconds(audioSource.clip.length);

            audioSource.clip = explosionClip;
            audioSource.Play();
        }
        else
        {
            yield return new WaitForSeconds(2);
        }

        GameObject currentExplosion = PhotonNetwork.Instantiate(explosion.name, currentBomb.transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(currentBomb);

        Collider[] collider = Physics.OverlapSphere(currentBomb.transform.position, bombRadius, playerMask);
        testCol = collider;


        foreach(Collider col in collider)
        {
            if(col.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(bombDamage / 2);
            }
        }

        yield return new WaitForSeconds(2);

        PhotonNetwork.Destroy(currentExplosion);
    }
}
