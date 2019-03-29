using UnityEngine;
using System.Collections;
using DragonBones;

public class TableTrash : Table
{
    string idleAnim = "Idle";
    string trowAnim = "Chop";

    public UnityArmatureComponent armature;

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
        armature = machine.transform.GetChild(0).GetComponent<UnityArmatureComponent>();
    }

    public override void DoTheAction()
    {
        if (chef.IsHoldingSomething())
        {
            chef.PutATray(trayPositioner);
            armature.animation.Play(trowAnim, 1);
            manager.TrashAnswer(transform.position);
            Destroy(trayOn);
        }
    }
}
