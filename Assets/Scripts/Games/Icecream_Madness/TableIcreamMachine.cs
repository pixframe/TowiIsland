using UnityEngine;
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
        CreateAUpperSprite("Icecream/Machine");
    }

    override public void DoTheAction()
    {
        if (chef.IsHoldingSomething())
        {
            Tray holdTray = chef.GetHoldingTray();
            trayToReturn = holdTray;
            if (holdTray.HasAContainer())
            {
                if (holdTray.KindOfConatiner() == numberOfIceCream)
                {
                    holdTray.SetACookedMeal(numberOfIceCream, FoodDicctionary.CookedMeal.ShapeOfCookedMeal(numberOfIceCream), FoodDicctionary.CookedMeal.ColorOfCookedMeal(numberOfIceCream));
                }
            }
            else
            {
                Debug.Log("you need a container");
            }
        }
        else
        {
            Debug.Log("Need a conatainer");
        }
    }
}