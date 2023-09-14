using System.Collections;
using System.Net;
using UnityEngine;

public class SlidingAbility : MonoBehaviour
{
    public float slideScale = 0.5f;
    public float slideForce = 10f;
    public float slideDuration = 1f;

    private Vector3 originalScale;
    private Rigidbody playerRigidbody;
    private bool isSliding = false;
    Rigidbody rb;
    public Movement movement;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        if(movement == null)
        {
            movement = GetComponent<Movement>();
        }

        if(movement != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) && !isSliding)
            {
                StartSlide();
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl) && isSliding == true)
            {
                StopAllCoroutines();
                transform.localScale = originalScale;
                isSliding = false;
            }
        }
    }

    private void StartSlide()
    {
        transform.localScale *= slideScale;
        Vector3 slideDirection = transform.forward;
        playerRigidbody.AddForce(slideDirection * slideForce, ForceMode.VelocityChange);
        isSliding = true;
    }

    private IEnumerator RevertScaleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.localScale = originalScale;
        isSliding = false;
    }
}