using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureChest : MonoBehaviour {
	
	public GameObject[] treasures;
	public Category[] categories;
	List<string> keys=new List<string>();
    List<string> categoryKeys = new List<string>();
	List<string[]> names=new List<string[]>();
	List<Texture> images=new List<Texture>();
	List<string> categoryImageKeys=new List<string>();
	// Use this for initialization
	void Awake () {
		for(int i=0;i<treasures.Length;i++)
		{
			Treasure treasureTemp=(Treasure)treasures[i].GetComponent(typeof(Treasure));
			keys.Add(treasureTemp.id);
            categoryKeys.Add(treasureTemp.category);
			images.Add(treasureTemp.image);
			names.Add(new string[]{treasureTemp.name,treasureTemp.namePlural});
		}
		for(int i=0;i<categories.Length;i++)
		{
			categoryImageKeys.Add(categories[i].key);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateNames(string[] langNames, int start, int end)
	{
		int index = start;
		for(int i=0;i<names.Count;i++)
		{
			names[i][0]=langNames[index++];
			names[i][1]=langNames[index++];
		}
		for(int i=0;i<categories.Length;i++)
		{
			categories[i].name=langNames[index++];
			categories[i].namePlural=langNames[index++];
		}
	}

	public GameObject GetTreasure(string identifier, Transform transformation){
		for(int i=0;i<treasures.Length;i++)
		{
			if(keys[i]==identifier)
			{
				return (GameObject)GameObject.Instantiate(treasures[i],transformation.position,treasures[i].transform.rotation);
			}
		}
		return null;
	}
	
	public string GetTreasureCategory(string identifier){
		for(int i=0;i<treasures.Length;i++)
		{
			if(keys[i]==identifier)
			{
				return treasures[i].GetComponent<Treasure>().category;
			}
		}
		return null;
	}

    public string[] GetObjectsInCategory(string identifier)
    {
        List<string> objects = new List<string>();

        for (int i = 0; i < treasures.Length; i++)
        {
            if (categoryKeys[i] == identifier)
            {
                objects.Add(keys[i]);
            }
        }

        return objects.ToArray();
    }
	
	public Texture GetTreasureImage(string identifier){
		for(int i=0;i<treasures.Length;i++)
		{
			if(keys[i]==identifier)
			{
				return images[i];
			}
		}
		return null;
	}
	public string[] GetTreasureNames(string identifier){
		for(int i=0;i<treasures.Length;i++)
		{
			if(keys[i]==identifier)
			{
				return names[i];
			}
		}
		return null;
	}
	public string[] GetTreasurCategoryNames(string identifier){
		for(int i=0;i<categories.Length;i++)
		{
			if(categoryImageKeys[i]==identifier)
			{
				return new string[]{categories[i].name,categories[i].namePlural};
			}
		}
		return null;
	}
	public Texture GetTreasureCategoryImage(string identifier){
		for(int i=0;i<categories.Length;i++)
		{
			if(categoryImageKeys[i]==identifier)
			{
				return categories[i].texture;
			}
		}
		return null;
	}

	[System.Serializable]
	public class Category{
		public string key;
		public Texture texture;
		public string name;
		public string namePlural;
	}
}
