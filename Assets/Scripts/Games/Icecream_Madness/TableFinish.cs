using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableFinish : Table
{
    AudioSource audioSource;

    //Direction of sounds direction
    const string winSoundPath = "SFX/Birds/Win";
    const string loseSoundPath = "SFX/Birds/Fail";

	// Use this for initialization
	void Start ()
    {
        Initializing();
        ChangeTableSprite(FoodDicctionary.finishTable);

        //Obtaining the audio source
        if (!GetComponent<AudioSource>())
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }
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
                        PlayTheAudio(true);
                    }
                    else
                    {
                        manager.BadAnswer(transform.position);
                        PlayTheAudio(false);
                    }
                }
                else
                {
                    manager.BadAnswer(transform.position);
                    PlayTheAudio(false);
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

    void PlayTheAudio(bool isOrderCorrect)
    {
        AudioClip clip;

        if (isOrderCorrect)
        {
            clip = Resources.Load<AudioClip>(winSoundPath);
        }
        else
        {
            clip = Resources.Load<AudioClip>(loseSoundPath);
        }

        audioSource.clip = clip;

        audioSource.Play();
    }
}
