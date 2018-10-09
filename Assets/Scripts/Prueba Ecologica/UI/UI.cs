using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour
{
	public GameObject advanceBut;
	Animator anim;
	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator>();
		advanceBut = transform.Find("UICamera").transform.Find("Advance").gameObject;
		advanceBut.SetActive(false);
	}

	// Update is called once per frame
	void Update ()
	{

	}
	public void CamRot()
	{
		anim.SetBool("Rot", false);
	}
}
