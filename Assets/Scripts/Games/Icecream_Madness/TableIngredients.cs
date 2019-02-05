using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableIngredients : Table
{

    public int ingridientNumber;
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

        ingridientShape = FoodDicctionary.RawIngridients.ShapeOfRawIngridient(ingridientNumber);

        ChangeTableSprite(FoodDicctionary.RawIngridients.ShapeOfContainerTable(ingridientNumber));

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
            newTray.SetARawIngridient(ingridientNumber, ingridientShape);
            chef.GrabATray(newTray.gameObject);
            Debug.Log("Grab The Ingrdient 2");
        }
    }

    public override void DoTheActionIfTheresSomethingOn()
    {
        base.DoTheActionIfTheresSomethingOn();
    }
}
