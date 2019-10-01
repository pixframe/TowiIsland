using UnityEngine;
using DragonBones;
using System.Collections;

public class TableChopper : TableInstrument
{

    // Use this for initialization
    void Start()
    {
        Initializing();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Initializing()
    {
        base.Initializing();

        CreateAMachine(FoodDicctionary.chopperMachine);
        armature = machine.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        SetAudioClip("Chopper");
    }

    override public void DoTheAction()
    {
        if (thingsAreMade)
        {
            if (thingGoodMade)
            {
                if (!chef.IsHoldingSomething())
                {
                    thingsAreMade = false;
                    thingGoodMade = false;
                    chef.GrabATray(trayToReturn.gameObject);
                }
                else
                {
                    if (chef.GetHoldingTray().CanMergeTrays(trayToReturn))
                    {
                        thingsAreMade = false;
                        thingGoodMade = false;
                        chef.GetHoldingTray().MergeTrays(trayToReturn);
                    }
                }
                ingredientsToInput.Clear();
            }

        }
        else
        {
            if (!workingMachine)
            {
                if (chef.IsHoldingSomething())
                {
                    Tray holdTray = chef.GetHoldingTray();
                    if (holdTray.HasRawIngridient())
                    {
                        if (holdTray.WhatKindOfIngridientsIs() > 3)
                        {
                            trayToReturn = holdTray;
                            holdTray.HideAllImages();
                            chef.PutATray(trayPositioner);
                            StartTheChopper(holdTray.GetIngridient());
                        }
                    }
                }
            }
        }
    }

    void StartTheChopper(int numberIn)
    {
        ingredientsToInput.Add(numberIn);
        if (ingredientsToInput.Count >= numberOfIngridients)
        {
            if (ingredientsToInput[0] > 3)
            {
                thingGoodMade = true;
                StartCoroutine(ChoppedTheToppings());
            }
        }
    }

    IEnumerator ChoppedTheToppings()
    {
        workingMachine = true;
        armature.armature.animation.Play(FoodDicctionary.Toppings.AnimationOfChopper(ingredientsToInput[0]),1);
        audioSource.Play();
        while (armature.armature.animation.isPlaying)
        {
            yield return null;
        }
        thingsAreMade = true;
        workingMachine = false;
        trayToReturn.TransformIngridientToTopping();
        armature.armature.animation.Play("Idle");
    }
}
