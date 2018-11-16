using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarToSelect : MonoBehaviour {

    AvatarSelectionController controller;
    Animator anim;

    bool getClose = false;
    bool getFar = false;
    bool inOriginalPos = true;
    bool isMoving;

    float speed;
    float lookWeight;
    float animSpeed = 1.5f;
    float originalX;
    float originalZ;

    Vector3 originalAngles;
    Vector3 backAngle;
    Vector3 lookFowardAngle = new Vector3(0f, 180f, 0f);
    Vector3 lookBackwardAngle = new Vector3(0f, 0f, 0f);

    enum PositionAboutCenter { Left, Right, Center};
    PositionAboutCenter posCen;

    Transform posToGo;

    private AnimatorStateInfo currentBaseState;

    // Use this for initialization
    void Start()
    {
        controller = FindObjectOfType<AvatarSelectionController>();
        anim = GetComponent<Animator>();
        originalX = transform.position.x;
        originalZ = transform.position.z;
        if (originalX == 0)
        {
            posCen = PositionAboutCenter.Center;
        }
        else if (originalX < 0)
        {
            posCen = PositionAboutCenter.Left;
        }
        else
        {
            posCen = PositionAboutCenter.Right;
        }
        originalAngles = transform.eulerAngles;
        backAngle = transform.eulerAngles;
        backAngle.y += 180f;
        posToGo = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            anim.SetFloat("Speed", speed);                          // set our animator's float parameter 'Speed' equal to the vertical input axis				
            anim.speed = animSpeed;                                 // set the speed of our animator to the public variable 'animSpeed'
            anim.SetLookAtWeight(lookWeight);                       // set the Look At Weight - amount to use look at IK vs using the head's animation
            currentBaseState = anim.GetCurrentAnimatorStateInfo(0); // set our currentState variable to the current state of the Base Layer (0) of animation
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (getFar)
        {
            controller.MoveSpotLight(transform.position);
            if (transform.position.z < originalZ)
            {
                speed = 1;
                switch (posCen)
                {
                    case PositionAboutCenter.Left:
                        if (transform.position.x <= originalX)
                        {
                            transform.eulerAngles = lookBackwardAngle;
                        }
                        break;
                    case PositionAboutCenter.Right:
                        if (transform.position.x >= originalX)
                        {
                            transform.eulerAngles = lookBackwardAngle;
                        }
                        break;
                }
            }
            else
            {
                getFar = false;
                speed = 0;
                controller.StopMoving();
                controller.AskForTheCharacter();
                inOriginalPos = true;
                isMoving = false;
                transform.eulerAngles = originalAngles;
                anim.SetFloat("Speed", 0);
            }
        }
        if (getClose)
        {
            controller.MoveSpotLight(transform.position);
            if (transform.position.z > posToGo.position.z)
            {
                speed = 1;
                switch (posCen)
                {
                    case PositionAboutCenter.Left:
                        if (transform.position.x >= 0)
                        {
                            transform.eulerAngles = lookFowardAngle;
                        }
                        break;
                    case PositionAboutCenter.Right:
                        if (transform.position.x <= 0)
                        {
                            transform.eulerAngles = lookFowardAngle;
                        }
                        break;
                }
            }
            else
            {
                getClose = false;
                speed = 0;
                controller.StopMoving();
                controller.AskIfItsAllRight();
                anim.SetFloat("Speed", 0);
            }

        }
    }

    private void OnMouseDown()
    {
        if (controller.CanSelect())
        {
            if (!isMoving)
            {
                if(inOriginalPos)
                {
                    getClose = true;
                    isMoving = true;
                    controller.ClearText();
                    controller.SomeOneIsMoving();
                    controller.SomeOneIsSelected(this);
                    inOriginalPos = false;
                }
            }
        }
        //transform.rotation = Quaternion.LookRotation(transform.position, posToGo.position);
    }

    public void GoBack()
    {
        transform.eulerAngles = backAngle;
        getFar = true;
        isMoving = true;
        controller.ClearText();
        controller.SomeOneIsMoving();
    }

    private void OnMouseOver()
    {
        if (controller.CanSelect())
        {
            controller.MoveSpotLight(transform.position);
        }
    }
}
