using UnityEngine;
using System.Collections;

public class SpotlightControl : MonoBehaviour {

	public Vector3 target;
	public bool turnOff=false;
	public bool turnOn=false;
	Vector3 originalPos;
	bool justOn=true;
	Transform cylinder;
	Light light;
	Color tintColor;
	// Use this for initialization
	void Start () {
		target = transform.position;
		originalPos = target;
		cylinder = transform.Find ("Cylinder");
		light = GetComponent<Light> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (turnOff) {
			light.intensity -= 0.2f;
			tintColor=cylinder.GetComponent<Renderer>().material.GetColor("_TintColor");
			cylinder.GetComponent<Renderer>().material.SetColor("_TintColor",new Color(tintColor.r,tintColor.g,tintColor.b,tintColor.a-0.005f));
			if (light.intensity <= 0) {
					light.intensity = 0;
					cylinder.gameObject.SetActive (false);
					turnOff = false;
			}
		} else {
			if(turnOn){
				if(justOn){
					cylinder.gameObject.SetActive (true);
					Vector3 tempPos=transform.position;
					transform.position = new Vector3(-7,tempPos.y,tempPos.z);
					target=originalPos;
					justOn=false;
				}
				light.intensity += 0.2f;
				tintColor=cylinder.GetComponent<Renderer>().material.GetColor("_TintColor");
				cylinder.GetComponent<Renderer>().material.SetColor("_TintColor",new Color(tintColor.r,tintColor.g,tintColor.b,tintColor.a+0.005f));
				if (light.intensity >= 3) {
					light.intensity = 3;
					cylinder.GetComponent<Renderer>().material.SetColor("_TintColor",new Color(tintColor.r,tintColor.g,tintColor.b,0.024f));
					turnOn = false;
					justOn=true;
				}
			}
			transform.position = Vector3.Lerp (transform.position, target,0.2f);
		}
	}
}
