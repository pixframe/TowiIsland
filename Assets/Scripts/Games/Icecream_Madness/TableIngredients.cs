using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableIngredients : Table
{

    public int ingridientNumber;
    Color colorToPut;
    string tableShape;
    string ingridientShape;

    // Use this for initialization
    void Start ()
    {
        Initializing();
    }


    public override void Initializing()
    {
        base.Initializing();

        ChangeTheColor("FFFFFF");

        colorToPut = FoodDicctionary.RawIngridients.ColorOfRawIngridient(ingridientNumber);

        if (gameObject.name.Contains("D") || gameObject.name.Contains("U"))
        {
            tableShape = "Center/";
        }
        else
        {
            tableShape = "Lateral/";
        }

        tableShape += FoodDicctionary.RawIngridients.ShapeOfContainerTable(ingridientNumber);
        ingridientShape = FoodDicctionary.RawIngridients.ShapeOfRawIngridient(ingridientNumber);
        Debug.Log(ingridientShape);

        ChangeTableSprite(tableShape);

        CreateALogo(ingridientShape);
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
            newTray.SetARawIngridient(ingridientNumber, tableShape, colorToPut);
            chef.GrabATray(newTray.gameObject);
            Debug.Log("Grab The Ingrdient 2");
        }
    }

    public override void DoTheActionIfTheresSomethingOn()
    {
        base.DoTheActionIfTheresSomethingOn();
    }
}
