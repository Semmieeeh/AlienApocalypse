using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    public float bobbingSpeed = 0.3f;
    public float bobbingAmount = 0.25f;
    public GameObject player;
    public float horizontal;
    public float vertical;
    public float speed;
    private float timer = 0.0f;
    private float midpoint = 0.0f;

    private Vector3 originalPosition;
    public float baseBobbingSpeed;

    private void Start()
    {
        originalPosition = transform.localPosition;        
    }

    private void Update()
    {
        if (player != null)
        {
            if(player.GetComponent<Movement>() != null)
            {
                if(player.GetComponent<Movement>().rb != null)
                {
                    bobbingSpeed = baseBobbingSpeed * player.GetComponent<Movement>().rb.velocity.magnitude * baseBobbingSpeed;
                }
            }
        }
        else
        {
            bobbingSpeed = 0.035f;
        }

        float waveslice = 0.0f;

        horizontal = player.GetComponent<Movement>().input.x;
        vertical = player.GetComponent<Movement>().input.y;

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            float wavesliceSpeed = bobbingSpeed * Time.deltaTime;
            timer += wavesliceSpeed;
            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }
        }

        if (timer != 0 && player.GetComponent<Movement>().grounded == true)
        {
            waveslice = Mathf.Sin(timer);
            float translateChange = waveslice * bobbingAmount;

            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange *= totalAxes;

            Vector3 localPosition = originalPosition;
            localPosition.y = midpoint + translateChange;
            transform.localPosition = localPosition;
        }
        else
        {
            transform.localPosition = originalPosition;
        }
    }
}
