using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableFinish : Table
{

	// Use this for initialization
	void Start ()
    {
        Initializing();
        ChangeTableSprite(FoodDicctionary.finishTable);
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public override void Initializing()
    {
        base.Initializing();
        spriteRenderer.color = Color.white;
    }

    override public void DoTheAction()
    {
        if (chef.IsHoldingSomething())
        {
            Tray tempTray = chef.GetHoldingTray();
            if (tempTray.HasAContainer())
            {
                chef.PutATray(trayPositioner);
                if (tempTray.IsWellMade())
                {
                    if (manager.CompareTrays(tempTray.GetMadeComposition()))
                    {
                        manager.GoodAnswer(transform.position);
                    }
                    else
                    {
                        manager.BadAnswer(transform.position);
                    }
                }
                else
                {
                    manager.BadAnswer(transform.position);
                }

                Destroy(tempTray.gameObject);
                hasSomethingOn = false;
            }
            else
            {
                Debug.Log("You shuold put a container");
            }
        }
    }
}
