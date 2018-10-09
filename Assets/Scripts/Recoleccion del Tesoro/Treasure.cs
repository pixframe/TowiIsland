using UnityEngine;
using System.Collections;

public class Treasure : MonoBehaviour {
	public enum state {Empty,Occupied};
	public string id;
	public string category;
	public string name;
	public string namePlural;
	public Texture image;
	public int order;
	public state currentState;
	public float yOffset;
	// Use this for initialization
	void Start () {
		currentState=state.Occupied;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SwitchState(){
		currentState=state.Empty;
	}
}
