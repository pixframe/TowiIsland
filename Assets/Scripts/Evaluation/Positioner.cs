using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Positioner : MonoBehaviour {

    bool ocuppied;
    bool isAvailableInThisTry;
    string objectName;
    BoxCollider coli;
    Vector3 originalSize;
    int layerNum;

    public LayerMask lays;

	// Use this for initialization
	void Start () {
        objectName = this.transform.name;
        ocuppied = false;
        coli = GetComponent<BoxCollider>();
        originalSize = coli.size;
        layerNum = gameObject.layer;
	}

    //this will check if the collider is free to be ocuppied by an object
    public bool CanFillTheCollider(string data) {
        if (isAvailableInThisTry)
        {
            if (!IsOcuppied())
            {
                if (data == objectName)
                {
                    ocuppied = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //this will tell if the collider its full with an object
    public bool IsOcuppied() {
        return ocuppied;
    }

    //this will clear the container an let it ready for the next try
    public void EmptyTheContainer() {
        ocuppied = false;
    }

    //this will tell if an object its reapeted
    public bool IsRepeated(string data) {
        return data == objectName;
    }

    //this will make an score if the player for bad luck could not put in a correct spot the object
    public int ScoreDistance(Vector3 mousePos) {
        int sibiNum = 1000;
        //first try in a two time bigger collider
        coli.size *= 2;
        gameObject.layer = 17;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, lays)) {
            if (hit.transform.gameObject != null) {
                sibiNum = hit.transform.GetSiblingIndex();
            }
            if (sibiNum == transform.GetSiblingIndex()) {
                coli.enabled = false;
                return 2;
            }
        }

        //second try in a 4 times bigger collider
        coli.size *= 2;
        Ray ray1 = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit1;
        if (Physics.Raycast(ray1, out hit1, Mathf.Infinity, lays))
        {
            if (hit1.transform.gameObject != null)
            {
                sibiNum = hit1.transform.GetSiblingIndex();
            }
            if (sibiNum == transform.GetSiblingIndex())
            {
                coli.enabled = false;
                return 1;
            }
        }

        //if the two attemps failed
        coli.enabled = false;
        return 0;
    }

    //this will set the collider into original position
    public void RestoreTheCollider() {
        coli.enabled = true;
        coli.size = originalSize;
        gameObject.layer = layerNum;
    }

    //this will get the name of the collider
    public string GetTheName() {
        return objectName;
    }

    public void SetAvailable()
    {
        isAvailableInThisTry = true;
    }
}
