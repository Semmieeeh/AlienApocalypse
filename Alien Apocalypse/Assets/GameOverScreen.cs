using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField]
    public Animator deathAnimator, spectatorAnimator;

    public void EnableGameOverScreen (bool active)
    {
        deathAnimator.SetBool ("Active", active);
    }

    public void EnableSpectatorScreen(bool active )
    {
        deathAnimator.gameObject.SetActive(active);
    }
}
