using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomCharacterSpawner : MonoBehaviour {
	public GameObject[]avatars;
	public float spawnTime;
	public int avatarCount;
	float remainingTime;

	List<Clothes> clothingTop;
	List<Clothes> clothingBottom;
	List<Clothes>clothingCostume;
	List<Clothes> clothingHat;
	List<Clothes> clothingShoes;
	List<Clothes> clothingAccessory;
	List<Clothes> clothingClocks;
	List<BotControlScript> controls;
	int counter=0;
	
	void Start()
	{
		clothingTop=new List<Clothes>();
		clothingBottom=new List<Clothes>();
		clothingCostume=new List<Clothes>();
		clothingHat=new List<Clothes>();
		clothingShoes=new List<Clothes>();
		clothingAccessory=new List<Clothes>();
		clothingClocks=new List<Clothes>();
		controls = new List<BotControlScript> ();
		remainingTime = spawnTime;
		LoadCatalogue ();
		for(int i=0;i<avatarCount;i++)
		{
			GameObject avatarRef;
			avatarRef=(GameObject)GameObject.Instantiate(avatars[Random.Range(0,avatars.Length)]);
			avatarRef.AddComponent<KillBox>();
			int option = Random.Range(0,2);
			if(option==0)
			{
				avatarRef.transform.position=new Vector3(-10,2.5f,Random.Range(0f,20f));
				avatarRef.transform.rotation=Quaternion.Euler(new Vector3(0,130,0));
			}else
			{
				avatarRef.transform.position=new Vector3(10,2.5f,Random.Range(0f,20f));
				avatarRef.transform.rotation=Quaternion.Euler(new Vector3(0,230,0));
			}
			BotControlScript controlRef = avatarRef.GetComponent<BotControlScript>();
			controlRef.overrideMovement=true;
			controls.Add(controlRef);
			LoadRandomClothes(avatarRef);
		}
		controls[counter++].v=Random.Range(0.25f, 0.5f);
	}
	void Update()
	{
		remainingTime -= Time.deltaTime;
		if(remainingTime<=0&&counter<controls.Count)
		{
			remainingTime=spawnTime;
			controls[counter++].v=Random.Range(0.25f, 0.5f);
		}
	}

	void LoadCatalogue()
	{
		AssetManager.LoadCatalogue();
		string[] clothingList = AssetManager.getClothingCatalogue();
		foreach (string item in clothingList)
		{
			string[] tempItem=item.Split('/');
			if(tempItem[0]!="")
			{
				string categId=tempItem[1].Split(':')[1];
				switch(categId)
				{
					case AssetManager.CLOTHING_TOP_TAG:
						clothingTop.Add(new Clothes(tempItem[0].Split(':')[0].Split('_')[1],tempItem[0].Split(':')[1],tempItem[0].Split(':')[0].Split('_')[0],categId));
					break;
					case AssetManager.CLOTHING_BOTTOM_TAG:
						clothingBottom.Add(new Clothes(tempItem[0].Split(':')[0].Split('_')[1],tempItem[0].Split(':')[1],tempItem[0].Split(':')[0].Split('_')[0],categId));
					break;
					case AssetManager.CLOTHING_COSTUME_TAG:
						clothingCostume.Add(new Clothes(tempItem[0].Split(':')[0].Split('_')[1],tempItem[0].Split(':')[1],tempItem[0].Split(':')[0].Split('_')[0],categId));
					break;
					case AssetManager.CLOTHING_HAT_TAG:
						clothingHat.Add(new Clothes(tempItem[0].Split(':')[0].Split('_')[1],tempItem[0].Split(':')[1],tempItem[0].Split(':')[0].Split('_')[0],categId));
					break;
					case AssetManager.CLOTHING_SHOES_TAG:
						clothingShoes.Add(new Clothes(tempItem[0].Split(':')[0].Split('_')[1],tempItem[0].Split(':')[1],tempItem[0].Split(':')[0].Split('_')[0],categId));
					break;
					case AssetManager.CLOTHING_ACCESSORY_TAG:
						clothingAccessory.Add(new Clothes(tempItem[0].Split(':')[0].Split('_')[1],tempItem[0].Split(':')[1],tempItem[0].Split(':')[0].Split('_')[0],categId));
					break;
					case AssetManager.CLOTHING_CLOCKS_TAG:
						clothingClocks.Add(new Clothes(tempItem[0].Split(':')[0].Split('_')[1],tempItem[0].Split(':')[1],tempItem[0].Split(':')[0].Split('_')[0],categId));
					break;
				}
			}
		}
	}

	void LoadRandomClothes(GameObject rootObj)
	{
		rootObj.tag = AssetManager.CHARACTER_TAG;
		// Add the JointManager Component to the root scene gameobject
		rootObj.AddComponent<JointManager>();
		
		// Initialise the JointManager
		JointManager jointManager = rootObj.GetComponent("JointManager") as JointManager;
		jointManager.Initialize(rootObj);

		int costume = Random.Range (0, 5);
		if (costume == 0) 
		{
			if(clothingCostume.Count>0)
			{
				int random=Random.Range(0,clothingCostume.Count);
				AddClothing(clothingCostume[random],rootObj,jointManager);
			}
		}else
		{
			if(clothingAccessory.Count>0)
			{
				int random=Random.Range(-1,clothingAccessory.Count);
				if(random!=-1)
					AddClothing(clothingAccessory[random],rootObj,jointManager);
			}
			/*if(clothingTop.Count>0)
			{
				int random=Random.Range(-1,clothingTop.Count);
				if(random!=-1)
					AddClothing(clothingTop[random],rootObj,jointManager);
			}*/
			if(clothingBottom.Count>0)
			{
				int random=Random.Range(-1,clothingBottom.Count);
				if(random!=-1)
					AddClothing(clothingBottom[random],rootObj,jointManager);
			}
			if(clothingHat.Count>0)
			{
				int random=Random.Range(-1,clothingHat.Count);
				if(random!=-1)
					AddClothing(clothingHat[random],rootObj,jointManager);
			}
			if(clothingClocks.Count>0)
			{
				int random=Random.Range(-1,clothingClocks.Count);
				if(random!=-1)
					AddClothing(clothingClocks[random],rootObj,jointManager);
			}
			if(clothingShoes.Count>0)
			{
				int random=Random.Range(-1,clothingShoes.Count);
				if(random!=-1)
				{
					Destroy(rootObj.transform.Find("Accesorios").gameObject);
					AddClothing(clothingShoes[random],rootObj,jointManager);
				}
			}
		}
		//AddClothing(clothingAccessory[Random.Range(0,clothingAccessory.Count)],rootObj);
		/*string clothes = sessionMng.activeKid.avatarClothes;
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
		}*/
	}

	public void AddClothing(Clothes piece, GameObject rootObj, JointManager jointManager)
	{
		if(rootObj)
		{
			if (rootObj.tag == AssetManager.CHARACTER_TAG)
			{	
				// Create clothing mesh object using the ClothingID
				Object obj = AssetManager.CreateClothing(piece.category+"_"+piece.name);	
				
				// Instantiate the clothing mesh into the scene
				GameObject clothingObj = (GameObject)Instantiate(obj);
				clothingObj.tag = piece.tag;
				clothingObj.transform.parent = rootObj.transform;
				clothingObj.transform.rotation = rootObj.transform.rotation;
				
				// Attach the clothing object joints to the character object joints
				jointManager.AttachToParent(clothingObj);

				// Setup the clothing material with the texture maps
				AssetManager.ApplyMaterialTextures(clothingObj,piece.material);
			}
		}
	}
	public class Clothes
	{
		public string name;
		public string material;
		public string category;
		public string tag;
		public Clothes(string name,string material,string category,string tag)
		{
			this.name=name;
			this.material=material;
			this.category=category;
			this.tag=tag;
		}
	}
}
