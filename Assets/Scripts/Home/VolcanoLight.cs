using UnityEngine;
using System.Collections;

public class VolcanoLight : MonoBehaviour {
	Light volcano;
	float sin=0f;
	// Use this for initialization
	void Start () {
		volcano = GetComponent<Light> ();
	}
	
	// Update is called once per frame
	void Update () {
		sin += Time.deltaTime;
		sin %= Mathf.PI;
		volcano.intensity = 5 + (3 * Mathf.Sin (sin));
	}
}
