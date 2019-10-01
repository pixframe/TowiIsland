using UnityEngine;
using System.Collections;
using DragonBones;

public class TableBlender : TableInstrument
{
    int?[] ingredientsSet = new int?[2];
    int iceId = 0;
    int smoothieNumber = 1;
    string idleAnim = "Idle";
    string blendAnimation = "Licuar_";

    // Use this for initialization
    void Start()
    {
        Initializing();
    }

    public override void Initializing()
    {
        base.Initializing();
        Debug.Log($"{name} this is a blender machine table");
        CreateAMachine(FoodDicctionary.blenderMachine);
        armature = machine.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
        SetAudioClip("Blender");
    }

    override public void DoTheAction()
    {
        if (chef.IsHoldingSomething())
        {
            if (!hasSomethingOn)
            {
                Debug.Log("We enter here");
                trayToReturn = chef.GetHoldingTray();
                if (ingredientsSet[0] == null && trayToReturn.HasRawIngridient() && !trayToReturn.HasCookedIngredient())
                {
                    if (trayToReturn.WhatKindOfIngridientsIs() == iceId)
                    {
                        chef.PutATray(trayPositioner);
                        ingredientsSet[0] = 0;

                        if (ingredientsSet[1] != null)
                        {
                            armature.animation.Play(blendAnimation + FoodDicctionary.Toppings.AnimationOfChopper((int)ingredientsSet[1]), 1);
                            trayToReturn.HideAllImages();
                            trayToReturn.ChangeRawIngridientToTopping((int)ingredientsSet[1]);
                            StartCoroutine(BlendRoutine());
                        }
                        else
                        {
                            armature.animation.Play(PlayCorrectAnimation(), 1);
                            Destroy(trayToReturn.gameObject);
                            trayToReturn = null;
                            hasSomethingOn = false;
                        }
                    }
                }
                else if (ingredientsSet[1] == null && trayToReturn.HasATopping())
                {
                    if (trayToReturn.WhatKindOfTopping() > 3)
                    {
                        chef.PutATray(trayPositioner);

                        ingredientsSet[1] = trayToReturn.WhatKindOfTopping();

                        if (ingredientsSet[0] != null)
                        {
                            armature.animation.Play(blendAnimation + FoodDicctionary.Toppings.AnimationOfChopper((int)ingredientsSet[1]), 1);
                            trayToReturn.HideAllImages();
                            StartCoroutine(BlendRoutine());
                        }
                        else
                        {
                            armature.animation.Play(PlayCorrectAnimation(), 1);
                            Destroy(trayToReturn.gameObject);
                            trayToReturn = null;
                            hasSomethingOn = false;
                        }
                    }
                }
            }
            else
            {
                if (!workingMachine)
                {
                    var secondTray = chef.GetHoldingTray();
                    if (secondTray.HasAContainer())
                    {
                        if (secondTray.KindOfConatiner() == smoothieNumber)
                        {
                            if (trayToReturn.CanMergeTrays(secondTray))
                            {
                                Debug.Log("We merge a tray in icecream machine");
                                trayToReturn.MergeTrays(secondTray);
                                trayToReturn.SetSmoothieColor();
                                chef.GrabATray(trayToReturn.gameObject);
                                hasSomethingOn = false;
                                armature.animation.Play(idleAnim, 1);
                                ingredientsSet = new int?[2];
                            }
                            else
                            {
                                Debug.Log("cannot merge trays in icecream machine");
                            }
                        }
                    }

                }
            }


        }
    }

    IEnumerator BlendRoutine()
    {
        workingMachine = true;
        hasSomethingOn = true;
        audioSource.Play();

        while (armature.animation.isPlaying)
        {
            yield return null;
        }

        workingMachine = false;
        Debug.Log("smoothie is blend");
        trayToReturn.SetACookedMeal(smoothieNumber, FoodDicctionary.CookedMeal.ShapeOfCookedMeal(smoothieNumber));
    }

    public string PlayCorrectAnimation()
    {
        string animName = "";
        if (ingredientsSet[0] != null)
        {
            animName += "Hielos";
        }
        if (ingredientsSet[1] != null)
        {
            animName += FoodDicctionary.Toppings.AnimationOfChopper((int)ingredientsSet[1]);
        }
        Debug.Log($"the animation we play is {animName}");
        return animName;
    }
}
