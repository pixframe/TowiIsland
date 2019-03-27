using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonBones;

public class TableMixer : TableInstrument
{
    List<int> ingredientsSaved = new List<int>();

    int kindOfDough;

    const string Idle = "Idle";
    const string BadMix = "Batido_Malo";
    const string GoodMix = "Batido_Bueno";
    const string IncompleteMix = "Batido_Incompleto";

    // Use this for initialization
    void Start()
    {
        Initializing();
    }

    public override void Initializing()
    {
        base.Initializing();
        CreateAMachine(FoodDicctionary.mixerMachine);
        armature = machine.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void DoTheAction()
    {
        if (chef.IsHoldingSomething())
        {
            trayToReturn = chef.GetHoldingTray();
            if (!hasSomethingOn)
            { 
                if (trayToReturn.HasRawIngridient() && trayToReturn.GetIngridient() < 4)
                {
                    if (ingredientsSaved.Count < 3)
                    {
                        ingredientsSaved.Add(trayToReturn.GetIngridient());
                        chef.PutATray(trayPositioner);
                        Destroy(trayToReturn.gameObject);
                        trayToReturn = null;
                        hasSomethingOn = false;
                        armature.animation.Play(PrintTheAnim(), 1);
                    }
                    else
                    {
                        StartCoroutine(StartTheMix(BadMix));
                    }
                }
            }
        }
        else
        {
            if (hasSomethingOn)
            {
                if (!workingMachine)
                {
                    GrabTheDough(kindOfDough);
                }
            }
            else
            {
                if (ingredientsSaved.Count == 2)
                {
                    if (ingredientsSaved[0] == ingredientsSaved[1])
                    {
                        GrabTheDough(2);
                    }
                    else
                    {
                        StartCoroutine(StartTheMix(IncompleteMix));
                    }
                }
                else if (ingredientsSaved.Count == 3)
                {
                    if (!HasARepeatedIngredient())
                    {
                        StartCoroutine(StartTheMix(GoodMix));
                    }
                    else
                    {
                        StartCoroutine(StartTheMix(BadMix));
                    }
                }
            }

        }
    }


    void GrabTheDough(int typeOfDough)
    {
        Tray newTray = Instantiate(Resources.Load<GameObject>(FoodDicctionary.trayPrefab), chef.trayPositioner.transform).GetComponent<Tray>();
        newTray.SetACookedIngredient(typeOfDough);
        chef.GrabATray(newTray.gameObject);
        hasSomethingOn = false;
        ingredientsSaved.Clear();
        armature.animation.Play(Idle, 1);
    }

    bool HasARepeatedIngredient()
    {
        for (int i = 0; i < ingredientsSaved.Count; i++)
        {
            for (int j = i + 1; j < ingredientsSaved.Count; i++)
            {
                if (ingredientsSaved[i] == ingredientsSaved[j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    string PrintTheAnim()
    {
        string stringToReturn = "";

        ingredientsSaved.Sort();

        for (int i = 0; i < ingredientsSaved.Count; i++)
        {
            if (i > 0)
            {
                stringToReturn += "_";
            }
            stringToReturn += FoodDicctionary.RawIngridients.AnimNames(ingredientsSaved[i]);
        }

        return stringToReturn;
    }

    IEnumerator StartTheMix(string animationName)
    {
        workingMachine = true;
        hasSomethingOn = true;

        armature.animation.Play(animationName, 1);

        switch (animationName)
        {
            case GoodMix:
                kindOfDough = 0;
                break;
            case IncompleteMix:
                kindOfDough = 1;
                break;
            case BadMix:
                kindOfDough = 2;
                break;
        }

        while (armature.animation.isPlaying)
        {
            yield return null;
        }


        workingMachine = false;
        if (kindOfDough != 1)
        {

        }
    }
}
