using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableInstrument : Table
{

    public int retunerKindOfFood;
    protected List<int> ingredientsToInput = new List<int>();
    protected int numberOfIngridients;

    [System.NonSerialized]
    public bool thingsAreMade;
    public bool workingMachine;
    public bool thingGoodMade;

    protected Color colorToReturn;

    protected Tray trayToReturn;

    // Use this for initialization
    void Start ()
    {
        Initializing();
	}

    public override void Initializing()
    {
        base.Initializing();
    }


    // Update is called once per frame
    void Update ()
    {
		
	}
}
