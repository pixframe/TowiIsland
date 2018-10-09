using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

//this components are require for this script to work
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class PlayerController : MonoBehaviour {

    Animator anim;
    CapsuleCollider capsule;
    TreasureHuntManager manager;

    AnimatorStateInfo stateInfo;

    float speed = 1.5f;
    float horizontal;
    float vertical;
    float animSpeed = 1.2f;
    float lookSmoother = 3f;
    float lookWeight;
    float jumpDelay = 0.3f;

    bool jumpTime = false;
    bool movementTime = false;

    bool moveUp;
    bool moveLateral;

    bool needRetro = false;

    int retroIndex = 0;

    static int stateIdle = Animator.StringToHash("Base Layer.Idle");
    static int stateLocomotion = Animator.StringToHash("Base Layer.Locomotion");
    static int stateJump = Animator.StringToHash("Base Layer.Jump");
    static int stateBack = Animator.StringToHash("Base Layer.WalkBack");

    // Use this for initialization
    void Start() {
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        manager = FindObjectOfType<TreasureHuntManager>();
        if (anim.layerCount == 2)
        {
            anim.SetLayerWeight(1, 1);
        }
        anim.speed = animSpeed;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (movementTime) {
            Walking();
            anim.SetLookAtWeight(lookWeight);
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            lookWeight = Mathf.Lerp(lookWeight, 0f, Time.deltaTime * lookSmoother);
            Jump();
            if (jumpTime)
            {
                DoTheJump();
            }
        }
    }

    void Walking() {
        horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        vertical = CrossPlatformInputManager.GetAxis("Vertical");

        if (needRetro)
        {
            switch (retroIndex)
            {
                case 0:
                    if (vertical > 0.2 || vertical < -0.2)
                    {
                        manager.CheckForAnswer();
                        needRetro = false;
                        retroIndex++;
                    }
                    break;
                case 1:
                    if (horizontal > 0.2 || horizontal < -0.2)
                    {
                        manager.CheckForAnswer();
                        needRetro = false;
                        retroIndex++;
                    }
                    break;
            }
        }
        float speedMovement = vertical * speed;
        anim.SetFloat("Speed", speedMovement);
        anim.SetFloat("Direction", horizontal);
        if (horizontal != 0 && vertical > -0.2f && vertical < 0.2)
        {
            anim.SetBool("Turn", true);
        }
        else
        {
            anim.SetBool("Turn", false);
        }
    }

    void Jump()
    {
        if (stateInfo.fullPathHash != stateJump && stateInfo.fullPathHash != stateBack)
        {
            if (!anim.GetBool("Jump"))
            {
                if (CrossPlatformInputManager.GetAxis("Jump") > 0)
                {
                    anim.SetBool("Jump", true);
                    jumpTime = true;
                }
            }
        }
        else if (stateInfo.fullPathHash == stateJump)
        {
            if (!anim.IsInTransition(0))
            {
                anim.SetBool("Jump", false);
            }
        }
    }

    void DoTheJump() {
        jumpDelay -= Time.deltaTime;
        if (jumpDelay <= 0)
        {
            transform.GetComponent<Rigidbody>().AddForce(Vector3.up * 7000);
            jumpDelay = 0.3f;
            jumpTime = false;
        }
    }

    void OnAnimatorMove()
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.fullPathHash == stateLocomotion || stateInfo.fullPathHash == stateJump)
        {
            transform.Translate(Vector3.forward * anim.GetFloat("Speed") * 7 * Time.deltaTime);
            transform.Rotate(Vector3.up * Time.deltaTime * 100 * anim.GetFloat("Direction") * anim.GetFloat("Speed"));
        }
        else if(stateInfo.fullPathHash == stateBack)
        {
            transform.Translate(Vector3.forward * anim.GetFloat("Speed") * 4 * Time.deltaTime);
            transform.Rotate(Vector3.up * Time.deltaTime * 100 * anim.GetFloat("Direction") * ((anim.GetFloat("Speed") < 0) ? -1 : 1) * anim.GetFloat("Speed"));
        }

        if (anim.GetBool("Turn"))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 100 * anim.GetFloat("Direction"));
        }
    }

    public void TimeToMove()
    {
        movementTime = true;
    }

    public void DontMove()
    {
        movementTime = false;
    }

    public void AskForRetro()
    {
        needRetro = true;
    }
}
