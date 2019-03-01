using UnityEngine;
using DragonBones;
using System.Collections;

public class TableIcreamMachine : TableInstrument
{
    const int numberOfIceCream = 0;

    // Use this for initialization
    void Start()
    {
        Initializing();
    }

    public override void Initializing()
    {
        base.Initializing();
        Debug.Log($"{name} this is a icecream machine table");
        CreateAMachine(FoodDicctionary.icecreamMachine);
        armature = machine.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
    }

    override public void DoTheAction()
    {
        if (chef.IsHoldingSomething())
        {
            if (!hasSomethingOn)
            {
                Debug.Log("We enter here");
                trayToReturn = chef.GetHoldingTray();
                if (trayToReturn.HasAContainer())
                {
                    if (trayToReturn.KindOfConatiner() == numberOfIceCream)
                    {
                        Debug.Log("Should play this");
                        chef.PutATray(trayPositioner);
                        armature.animation.Play("Serve", 1);
                        foreach (SpriteRenderer sp in trayToReturn.GetComponentsInChildren<SpriteRenderer>())
                        {
                            sp.enabled = false;
                        }
                        StartCoroutine(ServeIcecreamRoutine());
                        Debug.Log("This is end");
                        //holdTray.SetACookedMeal(numberOfIceCream, FoodDicctionary.CookedMeal.ShapeOfCookedMeal(numberOfIceCream), FoodDicctionary.CookedMeal.ColorOfCookedMeal(numberOfIceCream));
                    }
                }
                else
                {
                    Debug.Log("you need a container");
                }
            }
            else
            {
                if (!workingMachine)
                {
                    var secondTray = chef.GetHoldingTray();
                    if (trayToReturn.CanMergeTrays(secondTray))
                    {
                        Debug.Log("We merge a tray in icecream machine");
                        trayToReturn.MergeTrays(secondTray);
                        chef.GrabATray(trayToReturn.gameObject);
                        hasSomethingOn = false;
                        armature.animation.Play("Idle");
                    }
                    else
                    {
                        Debug.Log("cannot merge trays in icecream machine");
                    }
                }
            }
            

        }
        else
        {
            if (!workingMachine)
            {
                if (hasSomethingOn)
                {
                    chef.GrabATray(trayToReturn.gameObject);
                    hasSomethingOn = false;
                    armature.animation.Play("Idle");
                }
            }
        }
    }

    IEnumerator ServeIcecreamRoutine()
    {
        workingMachine = true;
        hasSomethingOn = true;
        while (armature.animation.isPlaying)
        {
            yield return null;
        }
        workingMachine = false;
        Debug.Log("icecream is served");
        trayToReturn.SetACookedMeal(numberOfIceCream, FoodDicctionary.CookedMeal.ShapeOfCookedMeal(numberOfIceCream));
    }

    public override void DoTheActionIfTheresSomethingOn()
    {
        base.DoTheActionIfTheresSomethingOn();
        armature.animation.Play("Idle");
    }
}