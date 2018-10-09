/* ==================================================
 * Company   : Digitech Gamez
 *
 * Module    : GUIControlPanel.cs (Unity 3.5)
 * 
 * Desc      : Character Customisation Control Panel
 * 
 * Author    : gxmark
 * 
 * Date      : April 2012
 * 
 * Copyright : Royalty Free
 * ==================================================
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIControlPanel : MonoBehaviour 
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
	
	// Scroll Position for Control Panel
	Vector2 scrollCharacterPosition         = Vector2.zero;
	Vector2 scrollClothingPosition  		= Vector2.zero;
	Vector2 scrollAnimationPosition  		= Vector2.zero;
	Vector2 scrollActiveInstancePosition	= Vector2.zero;
	Vector2 scrollInActiveInstancePosition  = Vector2.zero;
	
	// Panel Sizes 
	int panelWidth = 0;
	int panelHeight = 0;
	
	// Rotate Feature
	bool rotateSelected = false;

	// Use this for initialization
	void Start () 
    {
		// Load the catalogues
		LoadCatalogues();
	}
	
	//  GUI Control
    void OnGUI()
    {
		// Panel Size Calculations
	   	panelWidth  = (int)((double)Screen.width/4.0f);
	   	panelHeight = Screen.height;
		
	   	// Make a group the size of the screen	
	   	GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
	
		GUI_Title();
		
		GUI_Box_Panels();
		
		GUI_Character_Control();
		
		GUI_Clothing_Control();
		
		GUI_Animation_Control();

		GUI_ActiveInstance_Control();
		
		GUI_InActiveInstance_Control();
		
       	GUI.EndGroup();
    }
	
	// Update is called once per frame
	void Update () 
    {
		// Rotate Button - rotate the character around its pivot point
		if(rotateSelected && instanceActive != "")
		{
			// Find the root asset object
		  	GameObject rootObj = GameObject.Find(instanceActive);
		
		  	// Only attach if its an AssetType
		  	if (rootObj.tag == AssetManager.ASSET_TAG && rootObj.active == true)
				rootObj.transform.Rotate(0.0f,1.0f * Time.deltaTime * 50,0.0f);
		}
	
	}
	

	// GUI Digitech Title
	void GUI_Title()
	{
	   	GUILayout.BeginArea(new Rect(Screen.width/2-150,5,300,50));
	   	GUILayout.Label("Digitech Gamez Control Panel - Version 1.1");
	   	GUILayout.EndArea();
	}
	
	// GUI Box Panels
	void GUI_Box_Panels()
	{
		// Create Catalogue Box Panels
	   	GUI.Box(new Rect(0,0,panelWidth,panelHeight/3), "Character Catalogue");
	   	GUI.Box(new Rect(0,panelHeight/3,panelWidth,panelHeight/3), "Clothing Catalogue");	
		GUI.Box(new Rect(0,(panelHeight/3)*2,panelWidth,(panelHeight/3)*2), "Animation Catalogue");
		
	   	// Create Instance Box Panels
	   	GUI.Box(new Rect(Screen.width-panelWidth,0,panelWidth,panelHeight/2), "Active Instance");	
	   	GUI.Box(new Rect(Screen.width-panelWidth,panelHeight/2,panelWidth,panelHeight/2), "InActive Instances");
	}
	
	// GUI Character Catalogue Control
	void GUI_Character_Control()
	{
	   	GUILayout.BeginArea(new Rect(0,30,panelWidth,50));
       	GUILayout.BeginHorizontal();
	   
		if ( GUILayout.Button("Create",GUILayout.Width(50)) )
		{
	       CreateCharacter();
		   instanceActiveList   = BuildInstanceActiveList(instanceActive);
		   instanceInActiveList.Add(instanceActive);
		}
		
		GUILayout.Space(10);
		if ( GUILayout.Button("Reload",GUILayout.Width(60)) )
		{
			// Load the catalogues
			LoadCatalogues();
		}
			
		GUILayout.EndHorizontal();
	    GUILayout.EndArea();
			
		GUILayout.BeginArea(new Rect(0,60,panelWidth,panelHeight/3));
		scrollCharacterPosition = GUILayout.BeginScrollView(scrollCharacterPosition,GUILayout.Width (panelWidth-5), GUILayout.Height ((int)((double)panelHeight/3.8)));
		characterSelection = (string)GUIListBox.SelectList(assetCharList, characterSelection, OnCheckboxItemGUI);
	    GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
	
	// GUI Clothing Catalogue Control
	void GUI_Clothing_Control()
	{
	   	GUILayout.BeginArea(new Rect(0,panelHeight/3+30,panelWidth,50));
       	GUILayout.BeginHorizontal();

	   	if ( GUILayout.Button("Create",GUILayout.Width(50)) )
       	{
			CreateClothing();
		    instanceActiveList   = BuildInstanceActiveList(instanceActive);
	   	}
		
	   	GUILayout.Space(10);
	   	GUILayout.Button("Delete",GUILayout.Width(50));
	   	GUILayout.EndHorizontal();
       	GUILayout.EndArea();
		
       	GUILayout.BeginArea(new Rect(0,panelHeight/3+60,panelWidth,panelHeight/3));
	   	scrollClothingPosition = GUILayout.BeginScrollView(scrollClothingPosition,GUILayout.Width (panelWidth-5), GUILayout.Height ((int)((double)panelHeight/3.8)));
	   	clothingSelection = (string)GUIListBox.SelectList(assetClothList, clothingSelection, OnCheckboxItemGUI);
	   	GUILayout.EndScrollView();
       	GUILayout.EndArea();
	}
	
	
	void GUI_Animation_Control()
	{
		GUILayout.BeginArea(new Rect(0,(panelHeight/3)*2+30, panelWidth,50));
	   	GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Add",GUILayout.Width(70)))
		{
			if (instanceActive != "")
	   		{
				// Find the character name of the object under root asset
				foreach(string str in instanceActiveList)
				{
					if(str.Contains(CHR))
					{
						// Find the root object
						GameObject characterObj = GameObject.Find(instanceActive+"/"+str.Substring(CHR.Length));
						
						if (characterObj != null)
						{
							AssetManager.AddAnimation(animationSelection, characterObj);
							instanceActiveList = BuildInstanceActiveList(instanceActive);
						}
						
						break;
					}
				}
			}
		}
		
		GUILayout.EndHorizontal();
       	GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect(0,(panelHeight/3)*2+60,panelWidth,panelHeight/3));
	   	scrollAnimationPosition = GUILayout.BeginScrollView(scrollAnimationPosition,GUILayout.Width (panelWidth-5), GUILayout.Height ((int)((double)panelHeight/3.8)));
       	animationSelection = (string)GUIListBox.SelectList(assetAnimList, animationSelection, OnCheckboxItemGUI);
	   	GUILayout.EndScrollView();
       	GUILayout.EndArea();
	}
	
	// GUI Active Instance Control
	void GUI_ActiveInstance_Control()
	{
	   	GUILayout.BeginArea(new Rect(Screen.width-panelWidth,30,panelWidth,50));
	   	GUILayout.BeginHorizontal();
	   
	   	if (GUILayout.Button("Delete",GUILayout.Width(50)))
		{
			// Delete the clothing item in the asset
			if (instanceActive != "" && instanceActiveSelection.Contains(CLT))
			{
				// Find the clothing object filtering out [clt]
				GameObject clothingObj = GameObject.Find(instanceActive+"/"+instanceActiveSelection.Substring(CLT.Length));
				
				// Remove if clothing object is found
				if (clothingObj != null)
				{
					GameObject rootObj = GameObject.Find(instanceActive);
					JointManager jointManager = rootObj.GetComponent("JointManager") as JointManager;
				    jointManager.DetachFromParent(clothingObj);
					Destroy(clothingObj);
				}
				
				// Remove from the instanceActiveList
				instanceActiveList.Remove(instanceActiveSelection);
				
				// Reset the selection back to the root asset
				instanceActiveSelection = instanceActive;
			}
			else
			// Delete the animation in the asset
			if (instanceActive != "" && instanceActiveSelection.Contains(ANI))
			{
				// Find the character object from instanceActiveList
				foreach(string str in instanceActiveList)
				{
					if (str.Contains(CHR))
					{
						// Find character object
						GameObject characterObj = GameObject.Find(instanceActive+"/"+str.Substring(CHR.Length));
						
						// Remove the animation clip
						AssetManager.RemoveAnimation(instanceActiveSelection.Substring(ANI.Length),characterObj);
				
						// Remove from the instanceActiveList
						instanceActiveList.Remove(instanceActiveSelection);
						
						// Reset the selection back to the root asset
						instanceActiveSelection = instanceActive;
						
						break;
					}
				}
			}
			else			// Delete the asset and all its children
			if (instanceActive != "")
	   		{
				instanceActiveList.Clear();
				instanceActiveSelection = "";
				instanceInActiveList.Remove(instanceActive);
				instanceInActiveSelection = "";
				RemoveAsset(instanceActive);
				instanceActive = "";
	   		}
			
		}
		
	   	GUILayout.Space(10);
	   	if (GUILayout.Button("Rotate",GUILayout.Width(50)))
	   	{
	      if (rotateSelected)
				rotateSelected = false;
		  else
				rotateSelected = true;
	   	}
		
		GUILayout.Space (10);
		
		if (GUILayout.Button ("Animate",GUILayout.Width (60)))
		{
			if (instanceActiveSelection.Contains (ANI))
			{
				GameObject rootObj = GameObject.Find (instanceActive);
				
				// Find the child character and clothing gameobjects
				Transform[] children = rootObj.GetComponentsInChildren<Transform>();
   				foreach (Transform child in children)
				{
   					if (child.tag == AssetManager.CHARACTER_TAG)
					{
						Animation animationObj = child.GetComponent(typeof(Animation)) as Animation;
				
						// If found play animation
						if (animationObj != null)
						{
			         		animationObj.PlayQueued(instanceActiveSelection.Substring(ANI.Length),QueueMode.PlayNow);
					 		animationObj.PlayQueued("tpose",QueueMode.CompleteOthers);
						}
					
						break;
					}
				}
			}
		}

		GUILayout.EndHorizontal();
       	GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect(Screen.width-panelWidth,60,panelWidth,panelHeight/2));
	   	scrollActiveInstancePosition = GUILayout.BeginScrollView(scrollActiveInstancePosition,GUILayout.Width (panelWidth-5), GUILayout.Height ((int)((double)panelHeight/2.8)));
       	instanceActiveSelection = (string)GUIListBox.SelectList(instanceActiveList, instanceActiveSelection, OnCheckboxItemGUI);
	   	GUILayout.EndScrollView();
       	GUILayout.EndArea();
	}
	
	
	// GUI InActive Instance Control
	void GUI_InActiveInstance_Control()
	{
	   	GUILayout.BeginArea(new Rect(Screen.width-panelWidth,panelHeight/2+20,panelWidth,panelHeight/2));
	   	scrollInActiveInstancePosition = GUILayout.BeginScrollView(scrollInActiveInstancePosition,GUILayout.Width (panelWidth-5), GUILayout.Height ((int)((double)panelHeight/2.8)));
       	instanceInActiveSelection = (string)GUIListBox.SelectList(instanceInActiveList, instanceInActiveSelection, OnCheckboxItemGUI);
	   	GUILayout.EndScrollView();
       	GUILayout.EndArea();
		
	   	// Check for new active asset
	   	if (instanceActive != instanceInActiveSelection)
	   	{
			GameObject obj = GameObject.Find(instanceInActiveSelection);
	    	// Only change the instanceActive if the instance selected is a root asset
			if (obj != null)
			if ( obj.tag == AssetManager.ASSET_TAG )
			{
				HideAsset(instanceActive);
				instanceActive = obj.name;
				ShowAsset(instanceActive);
				instanceActiveList = BuildInstanceActiveList(instanceActive);
			}
		}
	}
	
	private void LoadCatalogues()
	{
	    // Load the asset catalogue into the game
       	AssetManager.LoadCatalogue();
		
		// Ensure all lists are empty
		assetCharList.Clear();
		assetClothList.Clear();
		assetAnimList.Clear();
		
		// load the character assets
		string[] characterList = AssetManager.getCharacterCatalogue();
		foreach (string item in characterList)
        	assetCharList.Add(item);

        // load the clothing assets
		string[] clothingList = AssetManager.getClothingCatalogue();
		foreach (string item in clothingList)
        	assetClothList.Add(item);
		
		// load the animation assets
		string[] animationList = AssetManager.getAnimationCatalogue();
		foreach (string item in animationList)
        	assetAnimList.Add(item);
	
	}
	
	
	// Remove asset
	private void RemoveAsset(string assetID)
	{
		// Find the game object
		GameObject obj = GameObject.Find(assetID);
		// Destroy if found
		if (obj != null)
			Destroy(obj);
	}
	
	// Show asset
	private void ShowAsset (string assetID)
	{
		if (assetID != "")
		{
			GameObject obj = GameObject.Find(assetID);
			obj.SetActiveRecursively(true);
		}
	}
	
	// Hide asset
	private void HideAsset (string assetID)
	{
		if (assetID != "")
		{
			GameObject obj = GameObject.Find(assetID);
			obj.SetActiveRecursively(false);
			obj.active = true;
		}
	}
	
	// Create toggle box
    private bool OnCheckboxItemGUI(object item, bool selected, ICollection list)
    {
		bool result;
		string name = item.ToString();
		int indentLength = 0;
		
		// Indent the toggle option if its a character or clothing item
		if (name.Contains(CHR) || name.Contains(CLT))
			indentLength = 20;
		
		if (name.Contains(ANI))
			indentLength = 40;

		
		GUILayout.BeginHorizontal();
		GUILayout.Space(indentLength);
		result = GUILayout.Toggle(selected, item.ToString());
		GUILayout.EndHorizontal();
		
		return result;
    }
	
	
	// Create a character
	private void CreateCharacter()
	{
		  // Create the character from the resource
	   	  string[] meshtex = characterSelection.Split(':');
		  Object characterObj = AssetManager.CreateCharacter(meshtex[0]);
		
		  // Create root scene gameobject for the character
		  GameObject rootObj = new GameObject(AssetManager.CreateGUID());
		  rootObj.tag = AssetManager.ASSET_TAG;
		
		  // Add the JointManager Component to the root scene gameobject
		  rootObj.AddComponent<JointManager>();
		       
          // Instantiate character into the scene
		  GameObject NewObject = (GameObject)Instantiate((Object)characterObj, rootObj.transform.position, rootObj.transform.rotation);
		  NewObject.tag = AssetManager.CHARACTER_TAG;
			
          // Parent the character object to the root scene gameobject
          NewObject.transform.parent = rootObj.transform;
		
		  // Apply the texture maps to the materials
		  AssetManager.ApplyMaterialTextures(NewObject,meshtex[1]);
		
		  // Initialise the JointManager
		  JointManager jointManager = rootObj.GetComponent("JointManager") as JointManager;
		  jointManager.Initialize(rootObj);
		
		  // Hide the currently active asset
		  HideAsset (instanceActive);
		
		  // Make the currently created character active
		  instanceActive = rootObj.name;
		
		  print("Character Asset = " + characterSelection + " Instance ID = " + rootObj.name);		
	}
	
	// Create clothing item
	private void CreateClothing()
	{
		if(instanceActive != "")
		{
		  // Find the root asset object
		  GameObject rootObj = GameObject.Find(instanceActive);
		
		  // Only attach if its an AssetType
		  if (rootObj.tag == AssetManager.ASSET_TAG)
		  {
		  	string[] meshtex = clothingSelection.Split(':');
				
			// Find the clothing object
			GameObject clothingObj = GameObject.Find(rootObj.name+"/"+meshtex[0]+"(Clone)");
				
			// Only instantiate the clothing mesh if its not already attached to the asset
			if(clothingObj == null)
			{
				// Create clothing mesh object using the ClothingID
				Object obj = AssetManager.CreateClothing(meshtex[0]);	
				
				// Instantiate the clothing mesh into the scene
				clothingObj = (GameObject)Instantiate(obj);
				clothingObj.tag = AssetManager.CLOTHING_TAG;
				clothingObj.transform.parent = rootObj.transform;
				clothingObj.transform.rotation = rootObj.transform.rotation;
					
				// Attach the clothing object joints to the character object joints
				JointManager jointManager = rootObj.GetComponent("JointManager") as JointManager;
				jointManager.AttachToParent(clothingObj);
			}

			// Setup the clothing material with the texture maps
			AssetManager.ApplyMaterialTextures(clothingObj,meshtex[1]);
			
		  	print("Clothing Asset = " + clothingSelection + " Instance ID = " + rootObj.name);		
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
}


