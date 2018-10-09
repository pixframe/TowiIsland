using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerGrabbing : MonoBehaviour {

    ParticleSystem glow;
    ParticleSystem bengal;
    GameObject objectToGrab;

    Vector3 objectPosition;

    bool grabTime = false;
    bool canGrab = false;

    bool needRetro;

    int retroIndex;

    List<GameObject> grabbedObjects = new List<GameObject>();

    TreasureHuntManager manager;

	// Use this for initialization
	void Start () {
        manager = FindObjectOfType<TreasureHuntManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (canGrab) {
            if (CrossPlatformInputManager.GetAxis("Jump") > 0) {
                GrabAndPackInTheBackpack();
                if (needRetro)
                {
                    if (retroIndex > 0)
                    {
                        manager.CheckForAnswer();
                        needRetro = false;
                    }
                }
            }
        }
	}

    void OnTriggerEnter(Collider target)
    {
        if (grabTime) {
            if (target.tag == "Treasure")
            {
                objectPosition = target.transform.position;
                SetGlowInPosition();
                objectToGrab = target.gameObject.transform.parent.gameObject;
                canGrab = true;
                bengal = target.transform.parent.transform.GetComponentInChildren<ParticleSystem>();
                bengal.Stop();
                bengal.gameObject.SetActive(false);
            }
        }

    }

    void OnTriggerExit(Collider target)
    {
        if (target.tag == "Treasure") {
            DesactivateGlow();
            objectToGrab = null;
            canGrab = false;
            bengal.gameObject.SetActive(true);
            bengal.Play();
        }
    }

    void SetGlowInPosition()
    {
        if (needRetro)
        {
            if (retroIndex < 1)
            {
                manager.CheckForAnswer();
                retroIndex++;
            }
        }
        glow.transform.position = objectPosition;
        glow.gameObject.SetActive(true);
        glow.Play();
    }

    void DesactivateGlow() {
        glow.Stop();
        glow.gameObject.SetActive(false);
    }

    void GrabObject() {
        grabbedObjects.Add(objectToGrab);
        objectToGrab.SetActive(false);
        DesactivateGlow();
        objectToGrab = null;
    }

    public void SetGrabTime(ParticleSystem particles) {
        grabTime = true;
        glow = particles;
    }

    void GrabAndPackInTheBackpack() {
        manager.PutObjectInBackpack(objectToGrab);
        canGrab = false;
    }

    public void AskForRetro()
    {
        needRetro = true;
    }
}
