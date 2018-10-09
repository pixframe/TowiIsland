using UnityEngine;
using System.Collections;

public class TextureAutoResize : MonoBehaviour {
	float scale=1;
	// Use this for initialization
	void Start () {
		scale = (float)Screen.height / (float)768;
		GetComponent<GUITexture>().pixelInset = new Rect (GetComponent<GUITexture>().pixelInset.x * scale, GetComponent<GUITexture>().pixelInset.y * scale,
		                                  GetComponent<GUITexture>().pixelInset.width * scale, GetComponent<GUITexture>().pixelInset.height * scale);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
