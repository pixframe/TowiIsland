using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableIngredients : Table
{

    public int ingridientNumber;
    Color colorToPut;
    string shape = "Hexa";

    // Use this for initialization
    void Start ()
    {
        Initializing();
    }


    public override void Initializing()
    {
        base.Initializing();

        ChangeTheColor("8D8D8D");

        colorToPut = FoodDicctionary.RawIngridients.ColorOfRawIngridient(ingridientNumber);

        CreateAUpperSprite(colorToPut, shape);
    }

    // Update is called once per frame
    void Update () {
		
	}

    override public void DoTheAction()
    {
        base.DoTheAction();
    }

    public override void DoTheActionIfTheresNothingOn()
    {
        if (chef.IsHoldingSomething())
        {
            hasSomethingOn = true;
            chef.PutATray(trayPositioner);
        }
        else
        {
            Debug.Log("Grab The Ingrdient");
            Tray newTray = Instantiate(Resources.Load<GameObject>("IcecreamMadness/Prefabs/Tray"), chef.trayPositioner.transform).GetComponent<Tray>();
            newTray.SetARawIngridient(ingridientNumber, shape, colorToPut);
            chef.GrabATray(newTray.gameObject);
            Debug.Log("Grab The Ingrdient 2");
        }
    }

    public override void DoTheActionIfTheresSomethingOn()
    {
        base.DoTheActionIfTheresSomethingOn();
    }
}
