/* ==================================================
 * Company   : Digitech Gamez
 *
 * Module    : AssetManager.cs (Unity 3.5)
 * 
 * Desc      : Loads the catalogue of assets from a file
 * 
 * Author    : gxmark
 * 
 * Date      : April 2012
 * 
 * Copyright : Royalty Free
 * ==================================================
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AssetManager : MonoBehaviour 
{
	public const string ASSET_TAG     = "AssetType";
	public const string CHARACTER_TAG = "CharacterType";
	public const string CLOTHING_TAG  = "ClothingType";
	public const string CLOTHING_TOP_TAG  = "ClothingTopType";
	public const string CLOTHING_BOTTOM_TAG  = "ClothingBottomType";
	public const string CLOTHING_SHOES_TAG  = "ClothingShoesType";
	public const string CLOTHING_SHOES_HIDDEN_TAG  = "ClothingShoesTypeHidden";
	public const string CLOTHING_HAT_TAG  = "ClothingHatType";
	public const string CLOTHING_COSTUME_TAG  = "ClothingCostumeType";
	public const string CLOTHING_ACCESSORY_TAG = "ClothingAccessoryType";
	public const string CLOTHING_CLOCKS_TAG = "ClothingClocksType";
	
	static private String[] characterCatalogue;
	static private String[] clothingCatalogue;
	static private String[] animationCatalogue;
		
	// Material Structure
	private struct materialitem
	{
		public string name;
		public string shader;
		public string diffuse;
		public string bump;
	}
	
	static private Dictionary<string,materialitem> materialMap = new Dictionary<string,materialitem>();
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// Create a GUID
	static public string CreateGUID()
	{
		return Guid.NewGuid().ToString();
	}

    static public void LoadCatalogue()
    {
		TextAsset textObj = null;
		
		textObj = (TextAsset)Resources.Load("character_catalogue", typeof(TextAsset));
		characterCatalogue = textObj.text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);
		
		textObj = (TextAsset)Resources.Load("clothing_catalogue", typeof(TextAsset));
		clothingCatalogue = textObj.text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);
		
		textObj = (TextAsset)Resources.Load("animation_catalogue", typeof(TextAsset));
		animationCatalogue = textObj.text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);

		//TextAsset test =(TextAsset)"Wawa";

    }
	
	static public String[] getCharacterCatalogue()
	{
		return characterCatalogue;
	}
	
	static public String[] getClothingCatalogue()
	{
		return clothingCatalogue;
	}
	
	static public String[] getAnimationCatalogue()
	{
		return animationCatalogue;
	}
	
	
	// Create the character from the fbx resource
	static public UnityEngine.Object CreateCharacter(string characterID)
	{
		return (Resources.Load("meshes/characters/"+characterID+"/"+characterID));
	}
	
	// Create the clothing from the fbx resource
	static public UnityEngine.Object CreateClothing(string clothingID)
	{
		return (Resources.Load("meshes/clothing/"+clothingID+"/"+clothingID.Split('_')[1]));
	}

	static public Sprite CreatePreview(string previewID)
	{
		return (Resources.Load<Sprite>("meshes/clothing/previews/"+previewID.Split('_')[0]+"/"+previewID.Split(':')[1]));
	}
	
	
	// Apply the texture maps to the materials of the mesh object
	static public void ApplyMaterialTextures(GameObject meshObj,string texturesetID)
	{
		// Resolve the path to the asset based on mesh type
		string path = null;
		if (meshObj.tag == AssetManager.CHARACTER_TAG)
			path = "textures/characters/";
		else
			path = "textures/clothing/";
		
		// Clear the materials map
		materialMap.Clear();
		
		// Load the texture maps
		object[] textures = Resources.LoadAll(path+texturesetID,typeof(Texture2D));
		
		// Load material note (material=?;shader=?;bump=?difuse=?;trans=?)
		object[] material_note = Resources.LoadAll(path+texturesetID,typeof(TextAsset));
		
		// Only process texture maps if material.txt is defined
		if (material_note.Length > 0)
		{
			// Parse into material definitions
			string[] material_defs = ((TextAsset)material_note[0]).text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);
			
			// Parse material definition
			foreach (string matdef in material_defs)
			{
				// Instantiate material item and initialise it
				materialitem matitemObj = new materialitem();
				matitemObj.name    = "";
				matitemObj.shader  = "";
				matitemObj.diffuse = "";
				matitemObj.bump    = "";
			
				
				string[] material_def = matdef.Split(';');
				
				foreach (string matitem in material_def)
				{
					string[] matparts = matitem.Split('=');
					
					switch(matparts[0])
					{
					case "name" 		:  matitemObj.name = matparts[1];   break;
					case "shader"   	:  matitemObj.shader = matparts[1]; break;
					case "bump"     	:  matitemObj.bump = matparts[1]; break;
					case "diffuse"		:  matitemObj.diffuse = matparts[1]; break;
					}
				}
				
				// Add the material information to the materials map
				materialMap.Add(matitemObj.name,matitemObj);
			}
			
			
			        
			// Find the materials for the clothing object
			Renderer[] renderers = meshObj.transform.GetComponentsInChildren<Renderer>() as Renderer[];
			// For each material renderer  
			foreach (Renderer r in renderers)
			{
				// Assign the correct texture to the correct texture type, this will be
				// determined from the material.txt file. "_BumpMap"
				foreach (Material matitem in r.materials)
				{
					// Seperate the (instance) appended text
					string[] matname = matitem.name.Split(' '); 
					
					// Check to see if material is in materialMap
					materialitem value = new materialitem();
					if (materialMap.TryGetValue(matname[0], out value) == false)
					{
						// Set up material in materialMap
						value.name   = matname[0];
						value.shader = "Diffuse";
						materialMap.Add(matname[0],value);
						print ("Can't find the material = " + matname[0] + " Setting to default diffuse shader");
					}
						
						
					matitem.shader = Shader.Find(materialMap[matname[0]].shader);

					
					// Iterate through textures assigning to correct texture type
					foreach (Texture2D texture2D in textures)
					{
						// Assign the textures to the correct texture types [diffuse, bump, etc]
						if (materialMap[matname[0]].diffuse == texture2D.name)
							matitem.SetTexture("_MainTex",texture2D);
						
						if (materialMap[matname[0]].bump == texture2D.name)
							// Convert to a normal map
							matitem.SetTexture("_BumpMap",texture2D);
					}
				}
			}
		}
		else
		{
			print (texturesetID + " has no material note");
		}
	}
	
	// Not used but will convert at runtime from bump to normal map !
	static private Texture2D ConvertNormalTexture(Texture2D texturemap)
	{
		Texture2D normalTexture = new Texture2D(texturemap.width,texturemap.height,TextureFormat.ARGB32,false);
		
		Color theColour = new Color();
		for (int x=0; x<texturemap.width; x++)
		{
			for (int y=0; y<texturemap.height; y++)
			{
			   theColour.r = texturemap.GetPixel(x,y).g;
			   theColour.g = theColour.r;
			   theColour.b = theColour.r;
			   theColour.a = texturemap.GetPixel(x,y).r;
			   normalTexture.SetPixel(x,y, theColour);
			}
		}
	
		normalTexture.Apply();
		
		return normalTexture;
	}
	
	
	// Add the clip to the animation component
	static public void AddAnimation(string animationID, GameObject characterObj)
	{
		// Find the animation component
		Animation animationObj = characterObj.GetComponent(typeof(Animation)) as Animation;

		if (animationObj != null)
		{
			// Add TPose clip if the no clips exist
			if (animationObj.GetClipCount() == 0)
				animationObj.AddClip(Resources.Load("animations/"+"tpose") as AnimationClip, "tpose");	
			// Add the clip to the animation object
 			animationObj.AddClip(Resources.Load("animations/"+animationID) as AnimationClip, animationID);
			animationObj.playAutomatically = false;
		}
	}
	
	// Remove the clip from the animation component 
	static public void RemoveAnimation(string animationID, GameObject characterObj)
	{
		// Find the animation component
		Animation animationObj = characterObj.GetComponent(typeof(Animation)) as Animation;

		if (animationObj != null)
		{
			// Remove TPose clip if the no clips exist
			if (animationObj.GetClipCount() == 2)
				animationObj.RemoveClip("tpose");
			
			animationObj.RemoveClip(animationID);
		}
	}
}