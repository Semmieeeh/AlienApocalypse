using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public ChestManager chestManager;

    [Space]
    [Header("Spawn Chances")]
    public float firearmChance;
    public float abilityPointChance;
    public float bombChance;

    [Header("Firearms")]
    public GameObject[] firearms;


    //[Space]
    //[Header("Spawn Chances")]
    //public float minFirearmChance;
    //public float maxFirearmChance;
    //public float minFirearmAbilityChance;
    //public float maxFirearmAbilityChance;
    //public float minBombChance;
    //public float maxBombChance;

    //[Header("Prefabs")]
    //public GameObject[] firearmDatas;
    //public GameObject[] firearmAbilities;

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
            OpenChest();

            chestBeacon.SetActive(false);
            GetComponent<Animator>().SetTrigger("Open");
            chestManager.New();

            opened = true;
        }
    }

    void OpenChest()
    {
        float random = Random.Range(0, 101);

        if(random < abilityPointChance)
        {
            SkillTree.AddAbilityPoint(0.25f);
        }
        else if(random < abilityPointChance + bombChance)
        {
            StartCoroutine(Bomb());

            return;
        }
        else if(random <= firearmChance)
        {
            int n = Random.Range(0, firearms.Length - 1);

            for(int i = 0; i < firearms.Length; i++)
            {
                if(n == i)
                {
                    GameObject gun = Instantiate(firearms[i], goToPoint.position, Quaternion.identity);

                    Rigidbody rb = gun.GetComponent<Rigidbody>();
                    rb.AddForce(goToPoint.forward * force, ForceMode.Impulse);
                    Vector3 torque = new Vector3(Random.Range(0, 0), Random.Range(0, 0), Random.Range(-2f, 2f));
                    rb.AddTorque(torque * 5);

                    return;
                }
            }
        }
    }

    public Collider[] testCol; 

    IEnumerator Bomb()
    {
        GameObject currentBomb = Instantiate(bomb, goToPoint.position, Quaternion.identity);

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

        GameObject currentExplosion = Instantiate(explosion, currentBomb.transform.position, Quaternion.identity);
        Destroy(currentBomb);

        Collider[] collider = Physics.OverlapSphere(currentBomb.transform.position, bombRadius, playerMask);
        testCol = collider;


        foreach(Collider col in collider)
        {
            if(col.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(bombDamage / 2);
                Vector3 exp = playerHealth.gameObject.transform.position - transform.position;
                playerHealth.gameObject.GetComponent<Rigidbody>().AddForce(exp * 15, ForceMode.Impulse);
            }
        }

        yield return new WaitForSeconds(2);

        Destroy(currentExplosion);
    }
}
