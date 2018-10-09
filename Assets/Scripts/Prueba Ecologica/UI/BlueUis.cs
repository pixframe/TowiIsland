using UnityEngine;
using System.Collections;

public class BlueUis : MonoBehaviour
{
	GUITexture texture;
	float screenScale;
	// Use this for initialization
	void Start ()
	{
		texture = GetComponent<GUITexture>();

	}

	// Update is called once per frame
	void Update ()
	{
		screenScale = (float)Screen.height / (float)768;
		if(name == "backg")
		{
			transform.position = Vector3.zero;
			texture.pixelInset = new Rect(Screen.width * .5f  - 600 * screenScale, Screen.height * .5f - 380 * screenScale, 1240 * screenScale, 280 * screenScale);
		}
		else if(name == "ObjPlace1")
		{
			transform.position = Vector3.zero;
			texture.pixelInset = new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f, 250 * screenScale, 250 * screenScale);
		}
		else if(name == "ObjPlace2")
		{
			transform.position = Vector3.zero;
			texture.pixelInset = new Rect(Screen.width * .5f - 100 * screenScale , Screen.height * .5f, 250 * screenScale, 250 * screenScale);
		}
		else if(name == "ObjPlace3")
		{
			transform.position = Vector3.zero;
			texture.pixelInset = new Rect(Screen.width * .5f + 300 * screenScale , Screen.height * .5f, 250 * screenScale, 250 * screenScale);
		}
	}
}
