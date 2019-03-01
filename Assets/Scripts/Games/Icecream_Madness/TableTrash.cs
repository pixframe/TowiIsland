using UnityEngine;
using System.Collections;

public class TableTrash : Table
{
    string idleAnim = "Idle";
    string trowAnim = "Chop";


    // Use this for initialization
    void Start()
    {
        Initializing();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Initializing()
    {
        base.Initializing();
        ChangeTableSprite(FoodDicctionary.trashTable);
        CreateAMachine(FoodDicctionary.trashMachine);
    }

    public override void DoTheAction()
    {
        if (chef.IsHoldingSomething())
        {
            chef.PutATray(trayPositioner);
        }
    }
}
