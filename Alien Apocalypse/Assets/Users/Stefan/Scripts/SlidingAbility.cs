using System.Collections;
using System.Net;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class SlidingAbility : MonoBehaviourPunCallbacks
{
    public float slideScale;
    public float slideForce;
    public float slideDuration;
    CapsuleCollider c;
    private float originalScale;
    private Rigidbody playerRigidbody;
    private bool isSliding = false;
    public Movement movement;
    public float centerOffset;
    Animator anim;
    public bool canSlide;
    public float slideCooldown;
    public float slideCooldownMax;

    private void Start()
    {

        playerRigidbody = GetComponent<Rigidbody>();
        c = GetComponent<CapsuleCollider>();
        originalScale = c.height;
        movement = GetComponent<Movement>();
        anim = movement.anim;
        
    }

    private void Update()
    {
        if(movement == null)
        {
            movement = GetComponent<Movement>();
        }
        slideCooldown -= Time.deltaTime;
        if(movement != null)
        {
            if (Input.GetKey(KeyCode.LeftControl) && !isSliding && slideCooldown <=0 && movement.grounded)
            {
                StartSlide();
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl) && isSliding == true)
            {
                float scale = originalScale;
                photonView.RPC("UpdateAnim", RpcTarget.All, scale, false, 0f);
                isSliding = false;
            }
        }
    }
    
    private void StartSlide()
    {
        float scale = c.height * slideScale;
        photonView.RPC("UpdateAnim", RpcTarget.All, scale, true, centerOffset);
        slideCooldown = slideCooldownMax;
        if (movement.input.magnitude > 0.5f)
        {
            Vector3 slideDirection = CalculateSlideDirection(movement.input);
            playerRigidbody.AddForce(slideDirection * slideForce, ForceMode.VelocityChange);
            StartCoroutine(nameof(Cancel));
        }
        isSliding = true;
        StartCoroutine(nameof(Cancel));
    }

    private Vector3 CalculateSlideDirection(Vector2 input)
    {
        
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        Vector3 slideDirection = (forward * input.y + right * input.x).normalized;

        return slideDirection;
    }

    IEnumerator Cancel()
    {
        yield return new WaitForSeconds(slideDuration);
        if (isSliding)
        {
            float scale = originalScale;
            photonView.RPC("UpdateAnim", RpcTarget.All, scale, false, 0f);
            isSliding = false;
        }
    }
    [PunRPC]
    void UpdateAnim(float scale,bool b,float centerHeight)
    {
        c.height = scale;
        c.center = new Vector3(0, centerHeight, 0);
        anim.SetBool("Sliding", b);
    }


    
}