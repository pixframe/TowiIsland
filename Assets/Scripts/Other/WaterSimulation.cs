using UnityEngine;
using System.Collections;

public class WaterSimulation : MonoBehaviour {
	public float scale=0.05f;
	public float speed=1.0f;
	public float height=10.0f;
	public GameObject shore;
	float startY;
	// Use this for initialization
	void Start () {
		startY=transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position=new Vector3(transform.position.x,startY+Mathf.Sin(Time.time*speed)*height,transform.position.z);
		if (shore){
			if (Mathf.Sin (Time.time * speed) > 0.9) {
					Color tempColor = shore.GetComponent<Renderer>().materials [1].color;
					shore.GetComponent<Renderer>().materials [1].color = new Color (tempColor.r, tempColor.g, tempColor.b, 1.0f);
			}
		}
	}
}
