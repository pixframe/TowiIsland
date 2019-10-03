using UnityEngine;
using System.Collections;
using DragonBones;

public class TableWaffleMaker : TableInstrument
{

    int? kindOfCooked = null;
    int waffleNumber = 2;

    const string NotReady = "BaileSinTerminar";
    const string Ready = "BaileVerde";
    const string Hurry = "BaileNaranja";
    const string Burn = "BaileRojo";
    const string Idle = "Idle";

    bool isOpen = true;

    // Use this for initialization
    void Start()
    {
        Initializing();
    }

    public override void Initializing()
    {
        base.Initializing();
        CreateAMachine(FoodDicctionary.waffleMachine);
        armature = machine.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        SetAudioClip("Beep");
    }

    public override void DoTheAction()
    {
        if (hasSomethingOn)
        {
            if (!workingMachine)
            {
                if (!isOpen)
                {
                    StopAllCoroutines();
                    StartCoroutine(GrabWaffleRoutine());
                }
                else
                {
                    GrabTheWaffle();
                }
            }
        }
        else
        {
            if (chef.IsHoldingSomething())
            {
                trayToReturn = chef.GetHoldingTray();
                if (trayToReturn.HasCookedIngredient() && kindOfCooked == null)
                {
                    kindOfCooked = trayToReturn.WhatKindOfCookedIngredientIs();
                    chef.PutATray(trayPositioner);
                    Destroy(trayOn);
                    StartCoroutine(CookRoutine($"Cerrar{FoodDicctionary.MadeIngridients.NameOfAnim((int)kindOfCooked)}"));
                }
            }
        }
    }

    IEnumerator GrabWaffleRoutine()
    {
        workingMachine = true;
        armature.animation.Play($"Abrir{FoodDicctionary.MadeIngridients.NameOfAnim((int)kindOfCooked)}", 1);
        while (armature.animation.isPlaying)
        {
            yield return null;
        }
        GrabTheWaffle();
    }

    void GrabTheWaffle()
    {
        isOpen = true;
        workingMachine = false;
        audioSource.Play();
        audioSource.loop = false;
        audioSource.pitch = 0.5f;
        if (chef.IsHoldingSomething())
        {
            Tray chefTray = chef.GetHoldingTray();
            if (chefTray.HasAContainer() && chefTray.KindOfConatiner() == waffleNumber && chefTray.CanSetACookMeal())
            {
                chef.GetHoldingTray().SetACookedMeal(waffleNumber, FoodDicctionary.MadeIngridients.ShapeOfCookedIngredient((int)kindOfCooked), (int)kindOfCooked);
                hasSomethingOn = false;
                kindOfCooked = null;
                armature.animation.Play(Idle, 1);
            }
        }
    }

    public IEnumerator CookRoutine(string animationName)
    {
        hasSomethingOn = true;
        workingMachine = true;
        isOpen = false;
        armature.animation.Play(animationName, 1);
        while (armature.animation.isPlaying)
        {
            yield return null;
        }

        armature.animation.Play(NotReady, 3);
        while (armature.animation.isPlaying)
        {
            yield return null;
        }

        workingMachine = false;
        armature.animation.Play(Ready, 3);
        audioSource.Play();
        while (armature.animation.isPlaying)
        {
            yield return null;
        }

        audioSource.Play();
        audioSource.loop = true;
        audioSource.pitch = 1.5f;
        armature.animation.Play(Hurry, 4);
        while (armature.animation.isPlaying)
        {
            yield return null;
        }

        audioSource.Play();
        audioSource.loop = false;
        audioSource.pitch = 0.5f;
        armature.animation.Play(Burn);
        kindOfCooked = 3;
    }
}
