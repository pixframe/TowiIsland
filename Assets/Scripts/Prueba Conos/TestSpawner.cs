using UnityEngine;
using System.Collections;

public class TestSpawner : MonoBehaviour {

	public string[] treasures;
	public int order;
	TestLogic mainLogic;
	TreasureChest chest;
	public GameObject spawnedObject;
	// Use this for initialization
	void Awake ()
	{
		mainLogic = GameObject.Find ("Main").GetComponent<TestLogic>();
		chest = GameObject.Find ("Treasure Chest").GetComponent<TreasureChest>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(spawnedObject==null){
			Spawn();
		}
	}
	
	public void Spawn ()
	{
		Destroy (spawnedObject);
		spawnedObject = chest.GetTreasure (treasures [0], transform);
		spawnedObject.GetComponent<Treasure>().order=order;
		Vector3 tempPos=spawnedObject.transform.position;
		spawnedObject.transform.position=new Vector3(tempPos.x,tempPos.y+spawnedObject.GetComponent<Treasure>().yOffset,tempPos.z);
	}
}
