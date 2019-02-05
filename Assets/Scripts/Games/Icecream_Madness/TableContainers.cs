using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableContainers : Table
{
    public int kindOfContainer = 0;
    string shape;

    // Use this for initialization
    void Start ()
    {
        Initializing();

        shape = FoodDicctionary.Containers.ShapeOfConatiner(kindOfContainer);

        ChangeTheColor("FFFFFF");

        CreateALogo(FoodDicctionary.Containers.ShapeOfConatiner(kindOfContainer));

        ChangeTableSprite(FoodDicctionary.Containers.ContainerTable);
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
        newTray.SetAContainer(kindOfContainer, shape);
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
