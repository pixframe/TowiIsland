using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour {

    int whereToPutIt;

    float movementSpeed;

    Rigidbody rigi;
    Collider coli;
    MagicRiverManager manager;

    bool floating = false;
    bool isTarget = false;

    int typeOfTarget;
    int numberOfSpecial;
    int index;

    Quaternion rot;
    Vector3 pos;
    Vector3 posCurrent;
    Vector3[] posToGo;

    GameObject pointsToGo;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (floating)
        {
            Swim();
        }
    }

    public void SetTheDirection(int direction)
    {
        whereToPutIt = direction;
    }

    public void SetToSwim(float speed)
    {
        movementSpeed = speed;
        rigi = GetComponent<Rigidbody>();
        coli = GetComponent<Collider>();
        manager = FindObjectOfType<MagicRiverManager>();
        rigi.isKinematic = false;
        coli.isTrigger = false;
        floating = true;

        pointsToGo = GameObject.FindGameObjectWithTag("SObject");
        posToGo = new Vector3[pointsToGo.transform.childCount];
        for (int i = 0; i < posToGo.Length; i++)
        {
            posToGo[i] = pointsToGo.transform.GetChild(i).transform.position;
            posToGo[i].y = transform.position.y;
        }

        posCurrent = posToGo[index];
    }

    void Swim()
    {
        Vector3 vectorDir = posCurrent - transform.position;
        transform.Translate(Vector3.Normalize(vectorDir) * movementSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, posCurrent) < 0.3f)
        {
            index++;
            posCurrent = posToGo[index];
        }
    }

    void OnMouseDown()
    {
        floating = false;
        coli.isTrigger = true;
        rot = transform.rotation;
        pos = transform.position;
        manager.GrabAThing(gameObject);
    }

    public void ActAfterAnswer()
    {
        floating = true;
        coli.isTrigger = false;
        transform.rotation = rot;
        transform.position = pos;
    }

    public int GetTheData()
    {
        return whereToPutIt;
    }

    void OnTriggerEnter(Collider target)
    {
        if (target.tag == "Nest")
        {
            if (isTarget)
            {
                manager.SpecialMiss(numberOfSpecial);
            }
            else
            {
                manager.MissAnAnswer(this);
            }
            Destroy(this.gameObject);
        }
    }

    public void SetThisAsTarget(int targetType, int stimulNumber)
    {
        typeOfTarget = targetType;
        numberOfSpecial = stimulNumber;
        if (typeOfTarget > 1)
        {
            ApplyModifier();
        }
        isTarget = true;
    }

    public bool IsThisATarget()
    {
        return isTarget;
    }

    public int SpecialNumber()
    {
        return numberOfSpecial;
    }

    public int WhatTargetIs()
    {
        return typeOfTarget;
    }

    void ApplyModifier()
    {
        transform.GetChild(typeOfTarget - 2).gameObject.SetActive(true);
    }
}
