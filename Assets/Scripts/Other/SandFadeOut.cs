using UnityEngine;
using System.Collections;

public class SandFadeOut : MonoBehaviour
{
	Color tempColor;
	public float fadeSpeed = 0.01f;
	// Use this for initialization
	void Start ()
	{
	 
	}
	
	// Update is called once per frame
	void Update ()
	{
		tempColor = GetComponent<Renderer>().materials [1].color;
		GetComponent<Renderer>().materials [1].color = new Color (tempColor.r, tempColor.g, tempColor.b, tempColor.a - fadeSpeed < 0 ? 0 : tempColor.a - fadeSpeed);
	}
}
