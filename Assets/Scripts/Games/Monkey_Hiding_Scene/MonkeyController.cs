using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyController : MonoBehaviour {

    Vector3 initialPosition;
    Vector3 newPosition;
    Vector3 firstPos;
    Vector3 secondPos;
    Vector3 direct;
    Vector3 direct1;

    float time = 0.3f;
    float yUpPosition = 2f;
    float yDownPosition;
    float movingSpeed;
    float switchingSpeed = 2f;
    float levitatingSpeed = 3f;

    bool inPosition = false;
    bool walking = false;
    bool floating = false;
    bool descending = false;
    bool holding = false;
    bool switchingPlaces = false;
    bool isSelectable = false;

    string holdObject;

    int direction = 0;
    int switchInstriction = 0;
    int numberOfStimulus = -1;

    Animator anim;
    MonkeyHidingManager manager;
    GameObject platform;
    Collider coli;

	// Use this for initialization
	void Awake ()
    {
        initialPosition = transform.position;
        anim = transform.GetChild(0).GetComponent<Animator>();
        manager = FindObjectOfType<MonkeyHidingManager>();
        platform = transform.GetChild(1).gameObject;
        yDownPosition = transform.position.y;
        coli = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!isSelectable)
        {
            if (walking)
            {
                if (direction == 0)
                {
                    WalkToThePosition();
                    StopMonkey();
                }
                else
                {
                    WalkOutOfThePosition();
                    StopMonkey();
                }

            }
            else if (floating)
            {
                MonkeyLevitating();
            }
            else if (switchingPlaces)
            {
                SwitchPlace();
            }
            else if (descending)
            {
                DescendMonkey();
            }
        }
	}

    //this will set the position where the monkey should go
    public void SetPosition(Vector3 pos)
    {
        newPosition = pos;
    }

    //this will set the direction of the walk 
    public void WalkTo(int directionToGo)
    {
        walking = true;
        RotateTheMonkey();
        movingSpeed = 10;
        direction = directionToGo;
    }

    //this will make the monkey coming into the scene
    void WalkToThePosition()
    {

        transform.Translate((newPosition - transform.position).normalized * Time.deltaTime * movingSpeed);

    }

    //this will make going out the scene
    void WalkOutOfThePosition()
    {
        transform.Translate((initialPosition - transform.position).normalized * Time.deltaTime * movingSpeed);
    }

    //this will rotate the monkey
    void RotateTheMonkey()
    {
        anim.SetFloat("Speed", 10f);
        if (gameObject.transform.GetSiblingIndex() == 1 || gameObject.transform.GetSiblingIndex() == 3)
        {
            transform.GetChild(0).transform.Rotate(new Vector3(0, 90, 0));
        }
        else
        {
            transform.GetChild(0).transform.Rotate(new Vector3(0, 270, 0));
        }
    }

    //this will stop the monkey to avoid going somewhere else
    void StopMonkey()
    {
        if (Vector3.Distance(gameObject.transform.position, newPosition) < 0.2f) {
            walking = false;
            RotateTheMonkey();
            MonkeyStill();
            if (direction == 0)
            {
                manager.MonkeyInPlace();
            }
            else {
                manager.MonkeysReadyToDanceAgain();
            }

        }
    }

    //this make the monkey stay still
    void MonkeyStill()
    {
        anim.SetFloat("Speed", 0);
    }

    //this will prepare the monekey to float
    public void MonkeyFloat()
    {
        floating = true;
        inPosition = false;
        platform.SetActive(true);
    }

    //this will lift the monkey above the floor to play the hidding game
    void MonkeyLevitating()
    {
        if (transform.position.y < yUpPosition)
        {
            transform.Translate(Vector3.up * Time.deltaTime * levitatingSpeed);
        }
        else
        {
            Vector3 newPos = transform.position;
            newPos.y = yUpPosition;
            transform.position = newPos;
            floating = false;
            manager.MonkeysFlying();
        }
    }

    //this will make a monkey to hold something
    public Vector3 HoldAnObject(string objectName, int stimulusNum)
    {
        holding = true;
        holdObject = objectName;
        numberOfStimulus = stimulusNum;
        return transform.GetChild(0).transform.GetChild(4).transform.position;
    }

    //this will tell if the monkey is holding something
    public bool IsHoldingSomething()
    {
        return holding;
    }

    //this will tel what object is holding a monkey if its holding something
    public string WhatIsHolding()
    {
        return holdObject;
    }

    //this will prepare monkeys to switch ppositions
    public void MoveToNextPopsition(int Side, Vector3 pos, float time)
    {
        direction = Side;
        newPosition = pos;
        switchingPlaces = true;
        switchInstriction = 0;
        if (direction == 0)
        {
            firstPos = transform.position + Vector3.forward;
            secondPos = newPosition + Vector3.forward;
            direct = Vector3.forward;
        }
        else {
            firstPos = transform.position + Vector3.back;
            secondPos = newPosition + Vector3.back;
            direct = Vector3.back;
        }
        if (firstPos.x < secondPos.x)
        {
            direct1 = Vector3.right;
        }
        else
        {
            direct1 = Vector3.left;
        }
        float distance = Vector3.Distance(transform.position, firstPos) + Vector3.Distance(firstPos, secondPos) + Vector3.Distance(secondPos, newPosition);
        switchingSpeed = distance / time;
    }

    //This will controle how monkeys switch places
    void SwitchPlace()
    {
        switch (switchInstriction)
        {
            case 0:
                transform.Translate(direct * switchingSpeed * Time.deltaTime);
                if (direction == 0)
                {
                    if (transform.position.z >= firstPos.z)
                    {
                        transform.position = firstPos;
                        switchInstriction = 1;
                        direct = Vector3.back;
                    }
                }
                else
                {
                    if (transform.position.z <= firstPos.z)
                    {
                        transform.position = firstPos;
                        switchInstriction = 1;
                        direct = Vector3.forward;
                    }
                }

                break;
            case 1:
                transform.Translate(direct1 * switchingSpeed * Time.deltaTime);
                if (direct1 == Vector3.right)
                {
                    if (transform.position.x >= secondPos.x)
                    {
                        transform.position = secondPos;
                        switchInstriction = 2;
                    }
                }
                else
                {
                    if (transform.position.x <= secondPos.x)
                    {
                        transform.position = secondPos;
                        switchInstriction = 2;
                    }
                }

                break;
            case 2:
                transform.Translate(direct * switchingSpeed * Time.deltaTime);
                if (direction == 0)
                {
                    if (transform.position.z <= newPosition.z)
                    {
                        transform.position = newPosition;
                        switchInstriction = 0;
                        switchingPlaces = false;
                        manager.NextMovement();
                    }
                }
                else
                {
                    if (transform.position.z >= newPosition.z)
                    {
                        transform.position = newPosition;
                        switchInstriction = 0;
                        switchingPlaces = false;
                        manager.NextMovement();
                    }
                }
                break;
        }
    }

    public void SelectableNow()
    {
        isSelectable = true;
    }

    public void DownTheMonkey()
    {
        descending = true;
        isSelectable = false;
    }

    void DescendMonkey()
    {
        if (transform.position.y > yDownPosition)
        {
            transform.Translate(Vector3.down * Time.deltaTime * levitatingSpeed);
        }
        else
        {
            Vector3 newPos = transform.position;
            newPos.y = yDownPosition;
            transform.position = newPos;
            descending = false;
            newPosition = initialPosition;
            WalkTo(1);
            platform.SetActive(false);
        }
    }


    public void MonkeyReset()
    {
        inPosition = false;
        walking = false;
        floating = false;
        descending = false;
        holding = false;
        switchingPlaces = false;
        isSelectable = false;
        holdObject = null;
        numberOfStimulus = -1;
        coli.enabled = true;
    }

    public void DisableTheMonkey()
    {
        coli.enabled = false;
    }

    void OnMouseDown()
    {
        if (isSelectable) {
            if (IsHoldingSomething())
            {
                manager.CorrectAnswer(WhatIsHolding());
                GameObject objectToPresent = manager.ObjectToShow(numberOfStimulus);
                objectToPresent.SetActive(true);
                objectToPresent.transform.position = transform.GetChild(0).transform.GetChild(4).transform.position;
                holding = false;
                holdObject = null;
                coli.enabled = false;
            }
            else
            {
                manager.BadAnswer();
                GameObject objectToPresent = manager.BadAnswerShow();
                objectToPresent.SetActive(true);
                objectToPresent.transform.position = transform.GetChild(0).transform.GetChild(4).transform.position;
            }
        }
    }
}
