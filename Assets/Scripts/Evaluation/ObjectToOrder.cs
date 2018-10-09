using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToOrder : MonoBehaviour {

    //This is used to restart the original position of the object
	Vector3 initialPosition;
    Quaternion initialRotation;
	Collider coli;
    Rigidbody rigi;

    //This is used to know if a object has been placed in some place
	bool savedInAHolder;
    //This one is used to know in what position was place the object
	int numberOfHolder;

    int numberOfLayer;

    bool saveInPlace;

	// Use this for initialization
	void Start () 
	{
		initialPosition = this.transform.position;
        initialRotation = this.transform.rotation;
		coli = GetComponent<Collider>();
		savedInAHolder = false;
        rigi = GetComponent<Rigidbody>();
        numberOfLayer = this.gameObject.layer;
	}

    //Set the object in its original settings
    public void ResetToNormalPosition()
	{
		this.transform.position = initialPosition;
        this.transform.rotation = initialRotation;
        this.gameObject.layer = numberOfLayer;
        this.gameObject.SetActive(true);
		coli.enabled = true;
		savedInAHolder = false;
        saveInPlace = false;
        rigi.isKinematic = true;
        rigi.useGravity = false;
    }

    // this set when some object is saved in a holder and in what holder
	public void SaveInAHolder(int holderNumber){
		savedInAHolder = true;
		numberOfHolder = holderNumber;
	}

    //this will ask if the object has been put in a holder
	public bool IsSavedInAHolder(){
		return savedInAHolder;
	}

    //this will return the number of the holder to delete the data storage in there
	public int IsHoldedInNumber(){
		return numberOfHolder;
	}

    public void SaveInPlace() {
        saveInPlace = true;
    }

    public bool IsSaveInPlace() {
        return saveInPlace;
    }

    public void FlyAhed() {
        this.gameObject.layer = 0;
        rigi.velocity = new Vector3(5f, 0f, 0f);
        rigi.isKinematic = false;
        rigi.useGravity = true;
    }
}
