using UnityEngine;
using System.Collections;

public class AvatarSpawner : MonoBehaviour {
    
	public GameObject[]avatars;
	GameObject avatarPos;
	public string key;
	[HideInInspector]
	public string[]clothesPieces;
	SessionManager sessionMng;
	GameObject avatarRef;
	SkinnedMeshRenderer[]tempRenderer;
	Transform shoesRef;
	int hideShoes=0;

	// Use this for initialization
	void Awake() {
        sessionMng = FindObjectOfType<SessionManager>();
		avatarPos=GameObject.Find("Avatar");
        //string avatarName = "Jaguar";
        string avatarName = sessionMng.activeKid.avatar;
        if (avatarName == "")
        {
            avatarName = "koala";
        }
		for (int i=0; i<avatars.Length; i++) {
			if(avatars[i].name==avatarName){
			 	avatarRef=(GameObject)GameObject.Instantiate(avatars[i]);
				avatarRef.transform.position=transform.position;
				avatarRef.transform.rotation=transform.rotation;
				avatarRef.name="Character";
				switch(key){
					case "Treasure":
						avatarRef.AddComponent<PlayerInteraction>();
						avatarRef.GetComponent<BotControlScript>().enabled=false;
					break;
					case "Prueba":
						avatarRef.GetComponent<Rigidbody>().isKinematic = true;
						avatarRef.transform.parent = Camera.main.transform.Find("Avatar").transform;
						avatarRef.transform.position = avatarRef.transform.parent.transform.position;
						avatarRef.transform.Rotate(0, -25f, 0);
                        avatarRef.GetComponent<BotControlScript>().enabled = false;
					break;
				}
				LoadClothes(avatarRef);
				break;
			}
		}
		if(tempRenderer!=null&&hideShoes>=1)
		{
			for(int idx=0;idx<tempRenderer.Length;idx++)
			{
				tempRenderer[idx].enabled=false;
			}
		}
	}

	void LoadClothes(GameObject rootObj)
	{
		rootObj.tag = AssetManager.CHARACTER_TAG;
		// Add the JointManager Component to the root scene gameobject
		rootObj.AddComponent<JointManager>();
		
		// Initialise the JointManager
		JointManager jointManager = rootObj.GetComponent("JointManager") as JointManager;
		jointManager.Initialize(rootObj);
		//sessionMng.activeKid.avatarClothes = "";
		//sessionMng.SaveSession ();

		foreach(Transform child in rootObj.transform)
		{
			if(child.tag==AssetManager.CLOTHING_SHOES_TAG)
			{
				shoesRef=child;
				tempRenderer= child.GetComponentsInChildren<SkinnedMeshRenderer>();
			}
		}

		string clothes = sessionMng.activeKid.avatarClothes;
		if(clothes!="")
		{
			clothesPieces=clothes.Split('|');
			for(int i=0;i<clothesPieces.Length;i++)
			{
				if(clothesPieces[i]!="")
				{
					hideShoes+=AddClothing(clothesPieces[i],rootObj);
				}
			}
		}
	}




	public int AddClothing(string clothingID, GameObject rootObj)
	{
		int isShoe = 0;
		if(rootObj)
		{
			
			// Only attach if its an AssetType
			if (rootObj.tag == AssetManager.CHARACTER_TAG)
			{
				string[] meshtex = clothingID.Split('/');
				foreach(Transform child in rootObj.transform)
				{
					if(child.tag==AssetManager.CLOTHING_COSTUME_TAG||child.tag==meshtex[1]||
					   ((meshtex[1]==AssetManager.CLOTHING_COSTUME_TAG&&child.tag!=AssetManager.CLOTHING_SHOES_TAG)&&(child.tag==AssetManager.CLOTHING_BOTTOM_TAG||child.tag==AssetManager.CLOTHING_SHOES_TAG
					                                          ||child.tag==AssetManager.CLOTHING_HAT_TAG||child.tag==AssetManager.CLOTHING_TOP_TAG)))
					{
						if(child.tag==AssetManager.CLOTHING_SHOES_TAG)
						{
							child.tag=AssetManager.CLOTHING_SHOES_HIDDEN_TAG;
							isShoe = 1;
						}else
						{
							Destroy(child.gameObject);
						}
					}
				}
				
				// Find the clothing object
				GameObject clothingObj = GameObject.Find(rootObj.name+"/"+meshtex[0]+"(Clone)");
				
				// Only instantiate the clothing mesh if its not already attached to the asset
				if(clothingObj == null)
				{
					// Create clothing mesh object using the ClothingID
					Object obj = AssetManager.CreateClothing(meshtex[0].Split(':')[0]);	
					
					// Instantiate the clothing mesh into the scene
					clothingObj = (GameObject)Instantiate(obj);
					clothingObj.tag = meshtex[1];
					clothingObj.transform.parent = rootObj.transform;
					clothingObj.transform.rotation = rootObj.transform.rotation;
					
					// Attach the clothing object joints to the character object joints
					JointManager jointManager = rootObj.GetComponent("JointManager") as JointManager;
					jointManager.AttachToParent(clothingObj);
				}
				
				// Setup the clothing material with the texture maps
				AssetManager.ApplyMaterialTextures(clothingObj,meshtex[0].Split(':')[1]);
			}
		}
		return isShoe;
	}

	public void ShowOriginalShoes()
	{
		shoesRef.tag=AssetManager.CLOTHING_SHOES_TAG;
		if(tempRenderer!=null)
		{
			for(int i=0;i<tempRenderer.Length;i++)
			{
				tempRenderer[i].enabled=true;
			}
		}
	}
	public void HideOriginalShoes()
	{
		shoesRef.tag=AssetManager.CLOTHING_SHOES_HIDDEN_TAG;
		if(tempRenderer!=null)
		{
			for(int i=0;i<tempRenderer.Length;i++)
			{
				tempRenderer[i].enabled=false;
			}
		}
	}
}
