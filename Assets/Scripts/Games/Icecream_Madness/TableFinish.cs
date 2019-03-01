using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableFinish : Table
{
    ParticleSystem confettiSystem;
    ParticleSystem crossesSystem;

	// Use this for initialization
	void Start ()
    {
        Initializing();
        ChangeTableSprite(FoodDicctionary.finishTable);
        confettiSystem = GameObject.FindGameObjectWithTag("Arrow").GetComponent<ParticleSystem>();
        crossesSystem = GameObject.FindGameObjectWithTag("Ground").GetComponent<ParticleSystem>();
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
                if (manager.CompareTrays(tempTray.GetMadeComposition()))
                {
                    confettiSystem.transform.position = transform.position;
                    confettiSystem.Play();
                }
                else
                {
                    crossesSystem.transform.position = transform.position;
                    crossesSystem.Play();
                }
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
