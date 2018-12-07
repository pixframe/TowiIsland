using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableContainers : Table
{
    public int kindOfContainer = 0;
    Color colorToPut;
    string shape;

    // Use this for initialization
    void Start ()
    {
        Initializing();

        colorToPut = FoodDicctionary.Containers.ColorOfContainer(kindOfContainer);
        shape = FoodDicctionary.Containers.ShapeOfConatiner(kindOfContainer);

        CreateAUpperSprite(colorToPut, shape);
    }

    public override void Initializing()
    {
        base.Initializing();

        ChangeTheColor("8D8D8D");
    }

    // Update is called once per frame
    void Update () {
		
	}

    override public void DoTheAction()
    {
        Tray newTray = Instantiate(Resources.Load<GameObject>("IcecreamMadness/Prefabs/Tray"), chef.trayPositioner.transform).GetComponent<Tray>();
        newTray.SetAContainer(kindOfContainer, shape, colorToPut);
        if (!chef.IsHoldingSomething())
        {
            chef.GrabATray(newTray.gameObject);
        }
        else
        {
            if (chef.GetHoldingTray().CanMergeTrays(newTray))
            {
                Debug.Log("Can merge trays");
                chef.GetHoldingTray().MergeTrays(newTray);
            }
            else
            {
                Destroy(newTray.gameObject);
            }
        }
    }
}
