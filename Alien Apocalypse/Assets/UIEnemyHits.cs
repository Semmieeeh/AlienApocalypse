using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEnemyHits : MonoBehaviour
{
    [Header("Hit Effects")]
    public Animator[] hitAnimators;

    public void EnemyHit()
    {
        foreach (Animator animator in hitAnimators)
        {
            animator.SetTrigger("Toggle");
        }
    }
}
