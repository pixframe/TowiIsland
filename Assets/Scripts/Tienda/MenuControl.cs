using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Xml.Serialization;

public class MenuControl : MonoBehaviour 
{
	// File based asset catalogue access
	AssetManager assetManager;
	
	// Instance List Identifiers
	private const string CHR = "[chr]";
	private const string CLT = "[clt]";
	private const string ANI = "[ani]";
	
	// List box items
	private List<string> assetCharList  	  = new List<string>();
	private List<string> assetClothList 	  = new List<string>();
	private List<string> assetAnimList 	  	  = new List<string>();
	private List<string> instanceActiveList   = new List<string>();
	private List<string> instanceInActiveList = new List<string>();
	
	// List box selected items
	private string characterSelection           = "";
	private string clothingSelection            = "";
	private string animationSelection           = "";
	private string instanceActiveSelection      = "";
	private string instanceInActiveSelection    = "";
	private string instanceActive               = "";

	public string[] categories;
	public List<MenuCategory> categoriesList;
	Hashtable ownedItems;
	StoreMenuItem itemClicked=null;
	
	// Panel Sizes 
	int panelWidth = 0;
	int panelHeight = 0;

	GameObject rootObj;
	Transform categoryMenu;
	Transform mainMenu;
	public GameObject bigSlot;
	public GameObject smallSlot;
	bool hideCategories=false;
	bool showCategories=false;
	public Vector3 hidePosition;
	Vector3 originalPosition;
	AvatarSpawner avatar;
	SessionManager sessionMng;
	public float hideOffset=500f;
	List<Transform> activeItems;
	List<Transform> activeGroups;
	int activeGroup = 0;
	int lastItem=0;
	int firstItem=0;
	public bool itemsTransition=false;
	bool transitionForward=true;
	bool itemsTransitionH=false;
	bool itemsTransitionV=false;
	float transitionH=0;
	float transitionV=0;
	GameObject nextArrow;
	GameObject prevArrow;


	public Texture kiwiTexture;
	public GUIStyle kiwiScoreStyle;
	float mapScale=1;
	int kiwis=0;
	public bool showConfirmation=false;
	public GUIStyle confirmationStyle;
	public GUIStyle yesButtonStyle;
	public GUIStyle noButtonStyle;
	public GUIStyle readyButton;
	public Texture BgColor;
	public Texture2D es_yesTexture;
	public Texture2D es_yesTextureHover;
	public Texture2D en_yesTexture;
	public Texture2D en_yesTextureHover;
	public Texture loadingScreen;
	public Texture en_loadingScreen;
	public Texture loadingColor;
	bool fadeIn=false;
	bool fadeOut=false;
	float opacity=0;

	string tempTag="";
	string tempId="";
	int tempPrice=0;
	
	// Rotate Feature
	bool rotateSelected = false;

	LanguageLoader language;
	// Use this for initialization
	void Start () 
	{
		activeItems = new List<Transform> ();
		activeGroups = new List<Transform> ();
		sessionMng = GetComponent<SessionManager> ();
		string lang = sessionMng.activeUser.language;
		if(lang=="")
			lang="es";
		language = GetComponent<LanguageLoader>();
		language.LoadGameLanguage(lang);
		switch(lang)
		{
			case "es":

			break;
			case "en":
				loadingScreen=en_loadingScreen;
			break;
		}
		categoriesList = new List<MenuCategory> ();
		categoryMenu = transform.Find ("CategoryMenu");
		nextArrow=categoryMenu.Find("NextArrow").gameObject;
		prevArrow=categoryMenu.Find("PrevArrow").gameObject;
		mainMenu = transform.Find ("MainMenu");
		originalPosition = mainMenu.localPosition;
		int langIdx = 0;
		foreach (Transform item in mainMenu)
		{
			StoreMenuItem tempStoreItem=item.GetComponent<StoreMenuItem>();
			tempStoreItem.title=language.levelStrings[langIdx++];
		}
		switch(lang)
		{
			case "es":
				yesButtonStyle.normal.background=es_yesTexture;
				yesButtonStyle.hover.background=es_yesTextureHover;
			break;
			case "en":
				yesButtonStyle.normal.background=en_yesTexture;
				yesButtonStyle.hover.background=en_yesTextureHover;
			break;
		}
		mapScale = Screen.height / 768.0f;
		kiwiScoreStyle.fontSize = (int)((float)kiwiScoreStyle.fontSize * mapScale);
		confirmationStyle.fontSize = (int)((float)confirmationStyle.fontSize * mapScale);
		readyButton.fontSize = (int)((float)readyButton.fontSize * mapScale);
		kiwis = sessionMng.activeKid.kiwis;
		avatar = GameObject.Find ("Avatar").GetComponent<AvatarSpawner> ();
		ownedItems = new Hashtable ();
        LoadCatalogues();
        LoadOwnedItems ();
		// Load the catalogues
	}
	
	//  GUI Control
	
	// Update is called once per frame
	void Update () 
	{
		if(!rootObj)
		{
			rootObj = GameObject.Find("Character");
			rootObj.GetComponent<BotControlScript>().enabled=false;
			//CreateCharacter();
			//CreateClothing("pirata:pirata_atlas1");
		}

		if (itemsTransition) 
		{
			bool finished=false;
			if(!itemsTransitionV)
			{
				if(transitionForward)
				{
					for(int i=0;i<=activeGroup;i++)
					{
						activeGroups[i].localPosition=new Vector3(activeGroups[i].localPosition.x,activeGroups[i].localPosition.y+4*Time.deltaTime,activeGroups[i].localPosition.z);
					}
					if(activeGroups[activeGroup].localPosition.y>=3)
					{
						itemsTransitionV =true;
					}
				}else
				{
					for(int i=0;i<=activeGroup-1;i++)
					{
						activeGroups[i].localPosition=new Vector3(activeGroups[i].localPosition.x,activeGroups[i].localPosition.y-4*Time.deltaTime,activeGroups[i].localPosition.z);
					}
					if(activeGroups[activeGroup-1].localPosition.y<=0)
					{
						itemsTransitionV =true;
					}
				}
			}

			if(!itemsTransitionH)
			{
				if(transitionForward)
				{
					for(int i=activeGroup+1;i<activeGroups.Count;i++)
					{
						activeGroups[i].Translate(Vector3.left*3*Time.deltaTime,Space.Self);
					}
					if(activeGroups[activeGroup+1].localPosition.x<=0)
					{
						itemsTransitionH=true;
					}
				}else
				{
					for(int i=activeGroup;i<activeGroups.Count;i++)
					{
						activeGroups[i].Translate(Vector3.right*3*Time.deltaTime,Space.Self);
					}
					if(activeGroups[activeGroup].localPosition.x>=hideOffset)
					{
						itemsTransitionH=true;
					}
				}
			}
			itemsTransition=!(itemsTransitionH&&itemsTransitionV);
			if(!itemsTransition)
			{
				if(transitionForward)
					activeGroup++;
				else
					activeGroup--;
				if(activeGroup==0)
					prevArrow.SetActive(false);
				if(activeGroup==activeGroups.Count-1)
					nextArrow.SetActive(false);
				//firstItem=lastItem+1;
				//lastItem=Math.Min(firstItem+1,activeItems.Count-1);
			}
		}
		else{
			if(hideCategories)
			{
				mainMenu.localPosition=Vector3.Lerp(mainMenu.localPosition,hidePosition,Time.deltaTime*1.5f);
				categoryMenu.Translate(Vector3.left*3*Time.deltaTime,Space.Self);
				//categoryMenu.localPosition=Vector3.Lerp(categoryMenu.localPosition,new Vector3(0,0,0),Time.deltaTime*1.5f);
				//Debug.Log(Vector3.Distance(new Vector3(0,0,0),categoryMenu.localPosition));
				if(categoryMenu.localPosition.x<=0)
				{
					categoryMenu.localPosition=new Vector3(0,0,0);
					hideCategories=false;
					StoreMenuItem[] tempItems=categoryMenu.GetComponentsInChildren<StoreMenuItem>();
					for(int i=0;i<tempItems.Length;i++)
					{
						tempItems[i].ignore=false;
					}
				}
			}
			if(showCategories)
			{
				//mainMenu.localPosition=Vector3.Lerp(mainMenu.localPosition,hidePosition,Time.deltaTime*1.5f);
				mainMenu.Translate(Vector3.down*2*Time.deltaTime,Space.Self);
				categoryMenu.Translate(Vector3.right*4*Time.deltaTime,Space.Self);
				//categoryMenu.localPosition=Vector3.Lerp(categoryMenu.localPosition,new Vector3(0,0,0),Time.deltaTime*1.5f);
				//Debug.Log(Vector3.Distance(new Vector3(0,0,0),categoryMenu.localPosition));
				if(mainMenu.localPosition.y<=0)
				{

					StoreMenuItem[] tempItems=mainMenu.GetComponentsInChildren<StoreMenuItem>();
					for(int i=0;i<tempItems.Length;i++)
					{
						tempItems[i].ignore=false;
					}

					mainMenu.localPosition=new Vector3(0,0,0);
					categoryMenu.localPosition=new Vector3(4.5f,0,0);
					foreach(Transform child in categoryMenu)
					{
						if(child.name!="BackArrow"&&child.name!="CategoryTitle"&&child.name!="NextArrow"&&child.name!="PrevArrow")
						{
							Destroy(child.gameObject);
						}
					}
					showCategories=false;
				}
			}
		}
		/*// Rotate Button - rotate the character around its pivot point
		if(rotateSelected && instanceActive != "")
		{
			// Find the root asset object
			GameObject rootObj = GameObject.Find(instanceActive);
			
			// Only attach if its an AssetType
			if (rootObj.tag == AssetManager.ASSET_TAG && rootObj.active == true)
				rootObj.transform.Rotate(0.0f,1.0f * Time.deltaTime * 50,0.0f);
		}*/
		
	}

	void OnGUI()
	{
		GUI.DrawTexture(new Rect(50*mapScale,20*mapScale,90*mapScale,90*mapScale),kiwiTexture);
		GUI.Label(new Rect(150*mapScale,30*mapScale,90*mapScale,90*mapScale),kiwis.ToString(),kiwiScoreStyle);

		if(showConfirmation)
		{	
			float offset=-200;
			GUI.DrawTexture(new Rect(Screen.width*0.5f-400*mapScale,Screen.height*0.5f+offset*mapScale,800*mapScale,150*mapScale),BgColor);
			if(kiwis>=tempPrice)
			{
				confirmationStyle.normal.textColor=new Color(0.5f,0.5f,0.5f);
				GUI.Label(new Rect(Screen.width*0.5f-348*mapScale,Screen.height*0.5f+2*mapScale+offset*mapScale,700*mapScale,150*mapScale),language.levelStrings[8]+" <color=#599b0b>"+tempPrice+" KIWIS</color>?",confirmationStyle);
				confirmationStyle.normal.textColor=new Color(1,1,1);
				GUI.Label(new Rect(Screen.width*0.5f-350*mapScale,Screen.height*0.5f+offset*mapScale,700*mapScale,150*mapScale),language.levelStrings[8]+" <color=#80e010>"+tempPrice+" KIWIS</color>?",confirmationStyle);
				if (GUI.Button (new Rect (Screen.width * 0.5f+255*mapScale, Screen.height * 0.5f+130*mapScale+offset*mapScale, 70 * mapScale, 70 * mapScale), "", noButtonStyle)) {
					showConfirmation=false;
					itemClicked=null;
				}
				if (GUI.Button (new Rect (Screen.width * 0.5f + 330 * mapScale, Screen.height * 0.5f+130*mapScale+offset*mapScale, 70 * mapScale, 70 * mapScale), "", yesButtonStyle)) {
					CreateClothing(tempId,tempTag);
					kiwis-=tempPrice;
					sessionMng.activeKid.kiwis=kiwis;
					sessionMng.SaveSession();
					showConfirmation=false;
					tempId="";
					tempTag="";
					tempPrice=0;
					itemClicked.owned=true;
					itemClicked.HideKiwi();
					itemClicked=null;
				}
			}else
			{
				confirmationStyle.normal.textColor=new Color(0.5f,0.5f,0.5f);
				GUI.Label(new Rect(Screen.width*0.5f-348*mapScale,Screen.height*0.5f+2*mapScale+offset*mapScale,700*mapScale,150*mapScale),language.levelStrings[9],confirmationStyle);
				confirmationStyle.normal.textColor=new Color(1,1,1);
				GUI.Label(new Rect(Screen.width*0.5f-350*mapScale,Screen.height*0.5f+offset*mapScale,700*mapScale,150*mapScale),language.levelStrings[9],confirmationStyle);
				if (GUI.Button (new Rect (Screen.width * 0.5f + 330 * mapScale, Screen.height * 0.5f+130*mapScale+offset*mapScale, 70 * mapScale, 70 * mapScale), "", yesButtonStyle)) {
					showConfirmation=false;
					itemClicked=null;
				}
			}
		}
		if (GUI.Button (new Rect (0, Screen.height-80*mapScale, 190*mapScale,80*mapScale), language.levelStrings[7],readyButton)) {
			fadeIn=true;
		}

		if(fadeIn){
			opacity+=1*Time.deltaTime;
			if(opacity>=1){
				opacity=1;
                
				//Application.LoadLevel("GameMenus");
                //SceneManager.LoadScene("GameMenus");
				//fadeIn=false;
			}
			GUI.color=new Color(1,1,1,opacity);
		}else if (fadeOut){
			opacity-=1*Time.deltaTime;
			if(opacity<=0){
				opacity=0;
				fadeOut=false;
			}
			GUI.color=new Color(1,1,1,opacity);
		}
		if(fadeIn)
		{
			GUI.DrawTexture(new Rect (0, 0, Screen.width, Screen.height), loadingColor);
			GUI.DrawTexture(new Rect(Screen.width/2-Screen.height/2,0,Screen.height,Screen.height),loadingScreen);
		}
		GUI.color=new Color(1,1,1,1);
	}

	public void ShowConfirmation(string clothingID,string tag,int price,StoreMenuItem item)
	{
		itemClicked = item;
		showConfirmation = true;
		tempId = clothingID;
		tempTag = tag;
		tempPrice = price;
	}

	void LoadOwnedItems()
	{
		string data = sessionMng.activeKid.ownedItems;
		//If not blank then load it
		if(!string.IsNullOrEmpty(data))
		{
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

            /*XmlSerializer serializer = new XmlSerializer(typeof(Hashtable));
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(data));
            ownedItems = (Hashtable)serializer.Deserialize(stream);
            */
			//Binary formatter for loading back
			BinaryFormatter b = new BinaryFormatter();
			//Create a memory stream with the data
			MemoryStream m = new MemoryStream(Convert.FromBase64String(data));
			//Load back the scoress
			ownedItems = (Hashtable)b.Deserialize(m);
		}
	}

	void SaveOwnedItems()
	{
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");


        /*XmlSerializer serializer = new XmlSerializer(typeof(Hashtable));
        MemoryStream stream = new MemoryStream();
        serializer.Serialize(stream, ownedItems);
        sessionMng.activeKid.ownedItems = Convert.ToBase64String(stream.GetBuffer());
        */
		BinaryFormatter b = new BinaryFormatter();
		//Create an in memory stream
		MemoryStream m = new MemoryStream();
		//Save the scores
		b.Serialize(m, ownedItems);
		//Add it to player prefs
		sessionMng.activeKid.ownedItems=Convert.ToBase64String(m.GetBuffer());
	}

	private void LoadCatalogues()
	{
		// Load the asset catalogue into the game
		AssetManager.LoadCatalogue();
		
		// Ensure all lists are empty
		assetClothList.Clear();
		categoriesList.Clear ();

		for(int i=0;i<categories.Length;i++)
		{
			categoriesList.Add(new MenuCategory(categories[i]));
		}

		// load the clothing assets
		string[] clothingList = AssetManager.getClothingCatalogue();
		foreach (string item in clothingList)
		{
			string[] tempItem=item.Split('/');
			string categId=tempItem[0].Split('_')[0];
			for(int i=0;i<categoriesList.Count;i++)
			{
				if(categoriesList[i].name==categId)
				{
					assetClothList.Add(tempItem[0]);
					categoriesList[i].items.Add(new MenuItem(tempItem[0],tempItem[1].Split(':')[0],tempItem[1].Split(':')[1],AssetManager.CreatePreview(tempItem[0]) as Sprite,int.Parse(tempItem[2])));
					break;
				}
			}
		}
	}

	public void LoadCategoryItems(string title, string categoryId, StoreMenuItem.SlotSize size)
	{
		activeGroup = 0;
		activeItems.Clear ();
		activeGroups.Clear ();
		lastItem = 0;
		firstItem=0;
		prevArrow.SetActive (false);
		nextArrow.SetActive (false);
		StoreMenuItem[] tempItems=mainMenu.GetComponentsInChildren<StoreMenuItem>();
		for(int i=0;i<tempItems.Length;i++)
		{
			tempItems[i].DeactivateObject();
		}

		hideCategories = true;
		float offset=0;
		float yOffset=0.4f;
		int row = 0;
		categoryMenu.localPosition = new Vector3 (4.5f, 0, 0);
		categoryMenu.Find("CategoryTitle").GetComponent<TextMesh>().text=title;
		int hideCounter = 0;
		float tempHideOffset = 0;

		for(int i=0;i< categoriesList.Count;i++)
		{
			if(categoriesList[i].name==categoryId)
			{
				GameObject itemsGroup=new GameObject();
				itemsGroup.transform.parent=categoryMenu;
				itemsGroup.transform.localPosition=new Vector3(0,0,0);
				itemsGroup.transform.localRotation=new Quaternion(0,0,0,0);
				activeGroups.Add(itemsGroup.transform);
				if(size==StoreMenuItem.SlotSize.Big)
				{
					GameObject temp= GameObject.Instantiate(bigSlot) as GameObject;
					activeItems.Add(temp.transform);
					lastItem=activeItems.Count-1;
					temp.transform.parent=itemsGroup.transform;
					temp.transform.localPosition=new Vector3(offset,0,0);
					temp.transform.localRotation=new Quaternion(0,0,0,0);
					StoreMenuItem tempMenuItem=temp.GetComponent<StoreMenuItem>();
					tempMenuItem.type=StoreMenuItem.MenuType.Empty;
					tempMenuItem.tag=categoriesList[i].items[0].tag;
					tempMenuItem.HideKiwi();
					offset+=tempMenuItem.xOffset;
					temp.transform.Find("Preview").GetComponent<SpriteRenderer>().sprite=Resources.Load<Sprite>("textures/RemoveIcon");
					
				}else
				{
					row++;
					GameObject temp= GameObject.Instantiate(smallSlot) as GameObject;
					activeItems.Add(temp.transform);
					lastItem=activeItems.Count-1;
					temp.transform.parent=itemsGroup.transform;
					temp.transform.localPosition=new Vector3(offset,yOffset,0);
					temp.transform.localRotation=new Quaternion(0,0,0,0);
					StoreMenuItem tempMenuItem=temp.GetComponent<StoreMenuItem>();
					tempMenuItem.type=StoreMenuItem.MenuType.Empty;
					tempMenuItem.tag=categoriesList[i].items[0].tag;
					tempMenuItem.HideKiwi();
					yOffset-=tempMenuItem.yOffset;
					temp.transform.Find("Preview").GetComponent<SpriteRenderer>().sprite=Resources.Load<Sprite>("textures/RemoveIcon");
				}
				hideCounter++;
	
				for(int idx=0;idx<categoriesList[i].items.Count;idx++)
				{
					if(size==StoreMenuItem.SlotSize.Big)
					{
						GameObject temp= GameObject.Instantiate(bigSlot) as GameObject;
						activeItems.Add(temp.transform);
						if(hideCounter%2==0)
						{
							nextArrow.SetActive(true);
							offset=0;
							tempHideOffset+=hideOffset;
							itemsGroup=new GameObject();
							itemsGroup.transform.parent=categoryMenu;
							itemsGroup.transform.localPosition=new Vector3(tempHideOffset,0,0);
							itemsGroup.transform.localRotation=new Quaternion(0,0,0,0);
							activeGroups.Add(itemsGroup.transform);
						}
						temp.transform.parent=itemsGroup.transform;
						//if(hideCounter>=2)
						//{
							temp.transform.localPosition=new Vector3(offset,0,0);
						//}else
						//{
						//	temp.transform.localPosition=new Vector3(offset,0,0);
						//	lastItem=activeItems.Count-1;
						//}
						temp.transform.localRotation=new Quaternion(0,0,0,0);
						StoreMenuItem tempMenuItem=temp.GetComponent<StoreMenuItem>();
						tempMenuItem.title=categoriesList[i].items[idx].description;
						tempMenuItem.menuId=categoriesList[i].items[idx].name;
						tempMenuItem.tag=categoriesList[i].items[idx].tag;
						tempMenuItem.price=categoriesList[i].items[idx].price;
						if(ownedItems.ContainsKey(categoriesList[i].items[idx].name.Split(':')[0]))
						{
							tempMenuItem.HideKiwi();
							tempMenuItem.owned=true;
						}
						offset+=tempMenuItem.xOffset;
						//temp.transform.FindChild("Title").GetComponent<TextMesh>().text=categoriesList[i].items[idx].description;
						temp.transform.Find("Preview").GetComponent<SpriteRenderer>().sprite=categoriesList[i].items[idx].img;
						hideCounter++;
					}else
					{
						row++;
						GameObject temp= GameObject.Instantiate(smallSlot) as GameObject;
						activeItems.Add(temp.transform);
						if(hideCounter%9==0)
						{
							nextArrow.SetActive(true);
							offset=0;
							tempHideOffset+=hideOffset;
							itemsGroup=new GameObject();
							itemsGroup.transform.parent=categoryMenu;
							itemsGroup.transform.localPosition=new Vector3(tempHideOffset,0,0);
							itemsGroup.transform.localRotation=new Quaternion(0,0,0,0);
							activeGroups.Add(itemsGroup.transform);
						}
						temp.transform.parent=itemsGroup.transform;
						//if(hideCounter>=9)
						//{
							temp.transform.localPosition=new Vector3(offset,yOffset,0);
						//}else
						//{
						//	temp.transform.localPosition=new Vector3(offset,yOffset,0);
						//	lastItem=activeItems.Count-1;
						//}
						temp.transform.localRotation=new Quaternion(0,0,0,0);
						StoreMenuItem tempMenuItem=temp.GetComponent<StoreMenuItem>();
						tempMenuItem.title=categoriesList[i].items[idx].description;
						tempMenuItem.menuId=categoriesList[i].items[idx].name;
						tempMenuItem.tag=categoriesList[i].items[idx].tag;
						tempMenuItem.price=categoriesList[i].items[idx].price;
						if(ownedItems.ContainsKey(categoriesList[i].items[idx].name.Split(':')[0]))
						{
							tempMenuItem.HideKiwi();
							tempMenuItem.owned=true;
						}
						yOffset-=tempMenuItem.yOffset;
						if(row==3)
						{
							row=0;
							offset+=tempMenuItem.xOffset;
							yOffset=0.4f;
						}
						//temp.transform.FindChild("Title").GetComponent<TextMesh>().text=categoriesList[i].items[idx].description;
						temp.transform.Find("Preview").GetComponent<SpriteRenderer>().sprite=categoriesList[i].items[idx].img;
						hideCounter++;
					}
				}
			}
		}
	}

	void NextItems()
	{
		prevArrow.SetActive (true);
		itemsTransition = true;
		itemsTransitionH = false;
		itemsTransitionV = false;
		transitionForward = true;
	}
	void PrevItems()
	{
		nextArrow.SetActive (true);
		itemsTransition = true;
		itemsTransitionH = false;
		itemsTransitionV = false;
		transitionForward = false;
	}

	void Back()
	{
		Debug.Log("Back");
		if(!hideCategories)
		{
			StoreMenuItem[] tempItems=categoryMenu.GetComponentsInChildren<StoreMenuItem>();
			for(int i=0;i<tempItems.Length;i++)
			{
				tempItems[i].ignore=true;
			}
			showCategories=true;
		}
	}

	// Create a character
	private void CreateCharacter()
	{
		rootObj.tag = AssetManager.CHARACTER_TAG;
		// Add the JointManager Component to the root scene gameobject
		rootObj.AddComponent<JointManager>();

		// Initialise the JointManager
		JointManager jointManager = rootObj.GetComponent("JointManager") as JointManager;
		jointManager.Initialize(rootObj);
	}
	
	// Create clothing item
	public void CreateClothing(string clothingID,string tag)
	{
		bool hideShoes = false;
		if(rootObj)
		{

			// Only attach if its an AssetType
			if (rootObj.tag == AssetManager.CHARACTER_TAG)
			{

				foreach(Transform child in rootObj.transform)
				{
					if(child.tag!="Untagged")
					{
						if(child.tag==AssetManager.CLOTHING_SHOES_HIDDEN_TAG)
							hideShoes=true;
						if((child.tag==AssetManager.CLOTHING_COSTUME_TAG&&tag!=AssetManager.CLOTHING_SHOES_TAG)||child.tag==tag||
						   (tag==AssetManager.CLOTHING_COSTUME_TAG&&child.tag!=AssetManager.CLOTHING_SHOES_TAG))
						{
							if(child.tag==AssetManager.CLOTHING_SHOES_TAG)
								hideShoes=true;
							if(child.name!="Accesorios")
								Destroy(child.gameObject);
						}
					}
				}
				string[] meshtex = clothingID.Split(':');
				if(!ownedItems.ContainsKey(meshtex[0]))
					ownedItems.Add(meshtex[0],true);
				SaveOwnedItems();
				// Find the clothing object
				GameObject clothingObj = GameObject.Find(rootObj.name+"/"+meshtex[0]+"(Clone)");
				
				// Only instantiate the clothing mesh if its not already attached to the asset
				if(clothingObj == null)
				{
					// Create clothing mesh object using the ClothingID
					GameObject obj =(GameObject)AssetManager.CreateClothing(meshtex[0]);	
					
					// Instantiate the clothing mesh into the scene
					clothingObj = (GameObject)Instantiate(obj);
					clothingObj.tag = tag;
					clothingObj.transform.parent = rootObj.transform;
					clothingObj.transform.rotation = rootObj.transform.rotation;
					
					// Attach the clothing object joints to the character object joints
					JointManager jointManager = rootObj.GetComponent("JointManager") as JointManager;
					jointManager.AttachToParent(clothingObj);

					string avatarClothes=sessionMng.activeKid.avatarClothes;
					//if(tag==AssetManager.CLOTHING_COSTUME_TAG)
					//{
					//	avatarClothes="";
					//}
					string[] clothesP=avatarClothes.Split('|');
					List<string> clothesPieces=new List<string>(clothesP);
					if(avatarClothes=="")
					{
						sessionMng.activeKid.avatarClothes=meshtex[0]+":"+meshtex[1]+"/"+tag;
					}else
					{
						bool found=false;
						for(int i=0;i<clothesPieces.Count;i++)
						{
							if(tag==AssetManager.CLOTHING_COSTUME_TAG&&!clothesPieces[i].Contains(AssetManager.CLOTHING_SHOES_TAG))
							{
								clothesPieces.RemoveAt(i--);
							}else
							if(clothesPieces[i].Contains(tag)||clothesPieces[i].Contains(AssetManager.CLOTHING_COSTUME_TAG))
							{
								clothesPieces[i]=meshtex[0]+":"+meshtex[1]+"/"+tag;
								found=true;
							}
						}
						avatarClothes="";
						for(int i=0;i<clothesPieces.Count;i++)
						{
							if(i==0)
							{
								avatarClothes+=clothesPieces[i];
							}else
							{
								avatarClothes+='|'+clothesPieces[i];
							}
						}
						sessionMng.activeKid.avatarClothes=avatarClothes;
						if(!found)
						{
							if(clothesPieces.Count>0)
								sessionMng.activeKid.avatarClothes=avatarClothes+'|'+meshtex[0]+":"+meshtex[1]+"/"+tag;
							else
								sessionMng.activeKid.avatarClothes=meshtex[0]+":"+meshtex[1]+"/"+tag;
						}
					}
					sessionMng.SaveSession();
				}
				
				// Setup the clothing material with the texture maps
				AssetManager.ApplyMaterialTextures(clothingObj,meshtex[1]);
				
				print("Clothing Asset = " + clothingID + " Instance ID = " + rootObj.name);		
			}
			if(hideShoes)
				avatar.HideOriginalShoes();
		}
	}

	public void DeleteClothing(string tag)
	{
		if(rootObj)
		{
			// Only attach if its an AssetType
			if (rootObj.tag == AssetManager.CHARACTER_TAG)
			{
				
				foreach(Transform child in rootObj.transform)
				{
					if(child.tag==tag)
					{
						if(child.name!="Accesorios")
							Destroy(child.gameObject);
						if(tag==AssetManager.CLOTHING_SHOES_TAG)
							avatar.ShowOriginalShoes();
					}
				}

				string avatarClothes=sessionMng.activeKid.avatarClothes;
				//if(tag==AssetManager.CLOTHING_COSTUME_TAG)
				//{
				//	avatarClothes="";
				//}
				string[] clothesP=avatarClothes.Split('|');
				List<string> clothesPieces=new List<string>(clothesP);
				bool found=false;
				for(int i=0;i<clothesPieces.Count;i++)
				{
					if(clothesPieces[i].Contains(tag))
					{
						clothesPieces.RemoveAt(i--);
					}
				}
				avatarClothes="";
				for(int i=0;i<clothesPieces.Count;i++)
				{
					if(i==0)
					{
						avatarClothes+=clothesPieces[i];
					}else
					{
						avatarClothes+='|'+clothesPieces[i];
					}
				}
				sessionMng.activeKid.avatarClothes=avatarClothes;
				sessionMng.SaveSession();
			}	
		}
	}
	
	// Build active asset list from scene hierarchy
	private List<string> BuildInstanceActiveList(string activeSelection)
	{
		List<string> instanceList = new List<string>();
		
		// Find actively selected asset
		GameObject rootObj = GameObject.Find(activeSelection);
		
		if (rootObj != null)
		{
			instanceList.Add(rootObj.name);
			
			// Find the child character and clothing gameobjects
			Transform[] children = rootObj.GetComponentsInChildren<Transform>();
			foreach (Transform child in children)
			{
				if (child.tag == AssetManager.CHARACTER_TAG)
				{
					instanceList.Add(CHR+child.name);
					
					// Add animation clips from animation component
					Animation animObj = child.GetComponent(typeof(Animation)) as Animation;
					
					if (animObj != null)
					{
						foreach (AnimationState state in animObj) 
							if (state.name != "tpose")
								instanceList.Add(ANI+state.name);
					}
				}
				
				if (child.tag == AssetManager.CLOTHING_TAG)
					instanceList.Add(CLT+child.name);
			}
		}
		
		return instanceList;
	}

	[System.Serializable]
	public class MenuCategory
	{
		public string name;
		public List<MenuItem> items;
		public MenuCategory(string name)
		{
			this.name=name;
			items=new List<MenuItem>();
		}
	}
	[System.Serializable]
	public class MenuItem
	{
		public string name;
		public string description;
		public Sprite img;
		public string tag;
		public int price;
		public MenuItem(string name,string description, string tag, Sprite image,int price)
		{
			this.name=name;
			this.description=description;
			this.tag=tag;
			this.price=price;
			img=image;
		}
	}
}


