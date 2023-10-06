using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyHits : MonoBehaviour
{
    [Header("Hit Effects")]
    public Animator[] hitAnimators;
    public Image[] imageRects;

    [SerializeField]
    Color shotColor, headShotColor;
    public void EnemyHit()
    {
        foreach (Image image in imageRects)
        {
            image.color = shotColor;
        }
        foreach (Animator animator in hitAnimators)
        {
            animator.SetTrigger("Activate");
        }
    }

    public void EnemyKill()
    {
        foreach (Image image in imageRects)
        {
            image.color = headShotColor;
        }
        foreach (Animator animator in hitAnimators)
        {
            animator.SetTrigger("Activate");
        }
    }



}
