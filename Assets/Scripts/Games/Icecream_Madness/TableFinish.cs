using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableFinish : Table
{
	// Use this for initialization
	void Start ()
    {
        Initializing();
        ChangeTableSprite("Talavera");
	}

    // Update is called once per frame
    void Update ()
    {
		
	}

    public override void Initializing()
    {
        base.Initializing();
        spriteRenderer.color = Color.white;
        Debug.Log("This are finish tables");
    }

    override public void DoTheAction()
    {
        if (chef.IsHoldingSomething())
        {
            Tray tempTray = chef.GetHoldingTray();
            if (tempTray.HasAContainer())
            {
                chef.PutATray(trayPositioner);
                manager.CompareTrays(tempTray.GetMadeComposition());
            }
            else
            {
                Debug.Log("You shuold put a container");
            }
            Destroy(tempTray.gameObject);
            hasSomethingOn = false;
        }
    }
}
