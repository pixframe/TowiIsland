    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableSimple : Table {

	// Use this for initialization
	void Start ()
    {
        Initializing();
	}

    public override void Initializing()
    {
        base.Initializing();
        ChangeTableSprite(FoodDicctionary.normalTable);
        ChangeTableColor();
    }

    // Update is called once per frame
    void Update () {
		
	}

    override public void DoTheAction()
    {
        base.DoTheAction();
    }
}
