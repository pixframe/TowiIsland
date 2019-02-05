using UnityEngine;
using System.Collections;

public class TableChopper : TableInstrument
{

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
        spriteRenderer.color = Color.blue;
        numberOfIngridients = 1;
    }

    override public void DoTheAction()
    {
        if (thingsAreMade)
        {
            if (thingGoodMade)
            {
                if (!chef.IsHoldingSomething())
                {
                    thingsAreMade = false;
                    thingGoodMade = false;
                    chef.GrabATray(trayToReturn.gameObject);
                }
                else
                {
                    if (chef.GetHoldingTray().CanMergeTrays(trayToReturn))
                    {
                        thingsAreMade = false;
                        thingGoodMade = false;
                        chef.GetHoldingTray().MergeTrays(trayToReturn);
                    }
                }
            }

        }
        else
        {
            if (!workingMachine)
            {
                if (chef.IsHoldingSomething())
                {
                    Tray holdTray = chef.GetHoldingTray();
                    trayToReturn = holdTray;
                    if (holdTray.HasRawIngridient())
                    {
                        StartTheChopper(holdTray.GetIngridient());
                    }
                    chef.PutATray(trayPositioner);
                }
            }
        }
    }

    void StartTheChopper(int numberIn)
    {
        ingredientsToInput.Add(numberIn);
        if (ingredientsToInput.Count >= numberOfIngridients)
        {
            if (ingredientsToInput[0] > 3)
            {
                thingGoodMade = true;
                StartCoroutine(ChoppedTheToppings(3.0f));
            }
        }
    }

    IEnumerator ChoppedTheToppings(float timeToComplete)
    {
        workingMachine = true;
        yield return new WaitForSeconds(timeToComplete);
        thingsAreMade = true;
        workingMachine = false;
        trayToReturn.TransformIngridientToTopping();
        Debug.Log("Done");
    }
}
