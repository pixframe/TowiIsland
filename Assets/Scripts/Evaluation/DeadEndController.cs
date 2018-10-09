using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEndController : MonoBehaviour {

    BoxCollider2D activator;
    BoxCollider2D desactivator;
    BoxCollider2D coli;

	// Use this for initialization
	void Start ()
    {
        coli = GetComponent<BoxCollider2D>();
        activator = transform.GetChild(1).GetComponent<BoxCollider2D>();
        desactivator = transform.GetChild(0).GetComponent<BoxCollider2D>();
        activator.gameObject.SetActive(false);
        desactivator.gameObject.SetActive(true);
        coli.enabled = true;
	}

    public void DesactivateTheColi()
    {
        coli.enabled = false;
        activator.gameObject.SetActive(true);
        desactivator.gameObject.SetActive(false);
    }

    public void ActivateTheColi()
    {
        coli.enabled = true;
        activator.gameObject.SetActive(false);
        desactivator.gameObject.SetActive(false);
    }

    public void HitTheColi()
    {
        coli.enabled = false;
        activator.gameObject.SetActive(true);
        desactivator.gameObject.SetActive(false);
    }
}
