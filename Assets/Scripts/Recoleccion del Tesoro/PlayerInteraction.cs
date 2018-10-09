using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
	
	Color originalColor;
	TreasureLogic mainNPC;
	//bool correctObject = false;
	public string currentObject = "";
	public string[] currentObjectNames;
	string currentObjectCategory = "";
	GameObject target;
	bool started;
	public List<TreasureLogic.Item> bag = new List<TreasureLogic.Item> ();
	public List<Texture> images = new List<Texture> ();
	public GameObject glow;
	Texture2D bgColor;
	TreasureChest chest;
	GUIStyle textBagStyle;
	float mapScale=1;

	// Use this for initialization
	void Start ()
	{
		chest = (TreasureChest)GameObject.Find ("Treasure Chest").GetComponent (typeof(TreasureChest));
		mainNPC = (TreasureLogic)GameObject.Find ("Main").GetComponent (typeof(TreasureLogic));
		bgColor = mainNPC.bgColor;
		textBagStyle = mainNPC.textBagStyle;
		glow = GameObject.Find ("Glow").gameObject;
		Debug.Log("Test");
		//GameObject.Find("PlayerCamera").GetComponent<ThirdPersonCamera>().standardPos=transform.Find ("CamPos");
		started = false;
		mapScale = (float)Screen.height / (float)768;
		textBagStyle.fontSize = (int)(textBagStyle.fontSize * mapScale);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown("space")) {
			Grab();
		}		
	}

	void Grab()
	{
		if(currentObject!="")
		{
			mainNPC.scoreEffects.DisplayNewObject(true,1,mainNPC.configuration.sound==1,null);
			started = true;
            bool objectExists = false;
			for (int i=0; i<bag.Count; i++) {
				if (bag [i].id == currentObject||bag [i].id == currentObjectCategory){
					bag [i].number++;
                    objectExists = true;
				}
			}
			if(!objectExists){
				bag.Add(new TreasureLogic.Item(currentObject,1,currentObjectNames,currentObjectCategory));
				images.Add(chest.GetTreasureImage(currentObject));
			}
			Destroy (target);
			glow.transform.position=new Vector3(-999,-999,-999);
			currentObject = "";
			//correctObject = false;
		}
	}
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Treasure") {
			target = other.gameObject;
			Treasure objectScript = (Treasure)other.GetComponent (typeof(Treasure));
			glow.transform.position=target.transform.position;
			//MeshRenderer objectRenderer=(MeshRenderer)other.GetComponent(typeof(MeshRenderer));
			//originalColor=objectRenderer.material.color;
			currentObject = objectScript.id;
			currentObjectCategory = objectScript.category;
			currentObjectNames=new string[]{objectScript.name,objectScript.namePlural};
			//if (objectScript.id==mainNPC.targetId){
			//	correctObject=true;
			//objectRenderer.material.color=new Color(0,100,0);
			/*}
			else{
				correctObject=false;
				objectRenderer.material.color=new Color(100,0,0);
			}*/
		} else if (other.tag == "NPC") {
			//if (started) {
				started=false;
				bool endCheck = false;
				if(mainNPC.itemList.Count!=bag.Count)
				{
					mainNPC.tryAgain=true;
				}else{
					for (int i=0; i<bag.Count; i++) {
						if (mainNPC.itemList[i].number-bag [i].number != 0)
							endCheck = true;
					}
					mainNPC.tryAgain = endCheck;
				}
				mainNPC.state = "SetComplete";
				if (!endCheck) {
					/*mainNPC.subLevel++;
					if (mainNPC.subLevel >= mainNPC.configuration.levels [mainNPC.level].subLevels.Length) {
						mainNPC.level++;
						mainNPC.subLevel = 0;
					}*/
				}
			//}
		}
		
	}

	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Treasure") {
			glow.transform.position=new Vector3(-900,-900-900);
			currentObject = "";
			//correctObject = false;
			//MeshRenderer objectRenderer=(MeshRenderer)other.GetComponent(typeof(MeshRenderer));
			//objectRenderer.material.color=originalColor;
		}
	}

	void OnGUI ()
	{
		int imgOffset=0;
		for (int i=bag.Count-1; i>=0; i--) {
			if(bag[i].number>0)
			{
				GUI.DrawTexture(new Rect(15*mapScale,115*mapScale+imgOffset*mapScale,110*mapScale,60*mapScale),bgColor);
				//GUI.Label (new Rect (10, 30 + 20 * i, 100, 20), bag [i].number.ToString () + "x" + bag [i].id);
				GUI.Label (new Rect (70*mapScale, 120*mapScale+imgOffset*mapScale, 50*mapScale, 50*mapScale), bag [i].number.ToString (),textBagStyle);
				GUI.DrawTexture(new Rect(20*mapScale,120*mapScale+imgOffset*mapScale,50*mapScale,50*mapScale),images[i]);
				imgOffset+=60;
			}
		}
	}
}
