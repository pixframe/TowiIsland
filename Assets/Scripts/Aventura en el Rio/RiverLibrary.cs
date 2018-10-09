using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiverLibrary : MonoBehaviour {
	
	public GameObject spawnZone;
	public RiverItem[] items;
	
	List<string> keys=new List<string>();
	// Use this for initialization
	void Awake () {
		for(int i=0;i<items.Length;i++)
		{
			keys.Add(items[i].key);
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public GameObject Spawn(string key,bool reverse,bool neutral,bool bubble,bool forceForest,bool forceBeach,int speed){
		for (int i=0; i<keys.Count; i++) {
			if(keys[i]==key){
				GameObject temp=(GameObject)GameObject.Instantiate(items[i].rObject, new Vector3(spawnZone.transform.position.x+Random.Range(-1,1),spawnZone.transform.position.y-2,spawnZone.transform.position.z),items[i].rObject.transform.rotation);
				RiverObject tempSettings=temp.GetComponent<RiverObject>();
				tempSettings.finalY=spawnZone.transform.position.y+tempSettings.yOffset;
				//temp.transform.position=new Vector3(transform.position.x,spawnZone.transform.position.y-2,transform.position.z);
				tempSettings.reverse=reverse;
				tempSettings.riverSpeed=speed*3;
				if(neutral)
					tempSettings.zone="";
				if(bubble){
					temp.transform.Find("Bubble").gameObject.SetActive(true);
				}
				if(forceForest){
					temp.transform.Find("BubbleBlue").gameObject.SetActive(true);
					tempSettings.zone="Forest";
				}
				if(forceBeach){
					temp.transform.Find("BubbleRed").gameObject.SetActive(true);
					tempSettings.zone="Beach";
				}
				return temp;
			}
		}
		return null;
	}

	public void UpdateNames(string[] newNames,int start, int end, string lang)
	{
		for(int i=0;i<items.Length;i++)
		{
			if(start+i<=end)
			{
				items[i].name=newNames[start+i];
				if(lang=="en")
				{
					items[i].articleD="the";
					items[i].articleI="the";
				}
			}
		}
	}
	
	public string[] GetNames(string key){
		for (int i=0; i<keys.Count; i++) {
			if (keys [i] == key) {
				return new string[]{items[i].name,items[i].articleD,items[i].articleI};
			}
		}
		return null;
	}
	public int GetSound(string key){
		for (int i=0; i<keys.Count; i++) {
			if (keys [i] == key) {
				return items[i].soundIdx;
			}
		}
		return -1;
	}
	public Texture GetImage(string key){
		for (int i=0; i<keys.Count; i++) {
			if (keys [i] == key) {
				return items[i].img;
			}
		}
		return null;
	}
	[System.Serializable]
	public class RiverItem{
		public string key;
		public GameObject rObject;
		public string name;
		public string articleD;
		public string articleI;
		public int soundIdx;
		public Texture img;
	}
}
