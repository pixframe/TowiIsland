using UnityEngine;
using System.Collections;

public class TreasureSpawner : MonoBehaviour
{
	TreasureLogic mainLogic;
	TreasureChest chest;
	public GameObject spawnedObject;
	// Use this for initialization
	void Awake ()
	{
		mainLogic = (TreasureLogic)GameObject.Find ("Main").GetComponent (typeof(TreasureLogic));
		chest = (TreasureChest)GameObject.Find ("Treasure Chest").GetComponent (typeof(TreasureChest));
	}

    public void Spawn (TreasureLogic.Item item)
    {
        Destroy(spawnedObject);

        if(item!=null)
        {
            if (item.type == "Category")
            {
                string[] objectsInCategory = chest.GetObjectsInCategory(item.id);
                string objectToSpawn = objectsInCategory[Random.Range(0,objectsInCategory.Length)];
                
                mainLogic.spawnedObjects.Add(objectToSpawn);
                spawnedObject = chest.GetTreasure(objectToSpawn, transform);
                Vector3 tempPos = spawnedObject.transform.position;
                spawnedObject.transform.position = new Vector3(tempPos.x, tempPos.y + spawnedObject.GetComponent<Treasure>().yOffset, tempPos.z);
            }
            else
            {
                mainLogic.spawnedObjects.Add(item.id);
                spawnedObject = chest.GetTreasure(item.id, transform);
                Vector3 tempPos = spawnedObject.transform.position;
                spawnedObject.transform.position = new Vector3(tempPos.x, tempPos.y + spawnedObject.GetComponent<Treasure>().yOffset, tempPos.z);
            }
        }else
        {
            string[] tempDistractors = mainLogic.GetDistractors();
            if (tempDistractors.Length > 0)
            {
                int randomIndex = Random.Range(0, tempDistractors.Length);

                mainLogic.spawnedDistractors.Add(tempDistractors[randomIndex]);
                spawnedObject = chest.GetTreasure(tempDistractors[randomIndex], transform);
                Vector3 tempPos = spawnedObject.transform.position;
                spawnedObject.transform.position = new Vector3(tempPos.x, tempPos.y + spawnedObject.GetComponent<Treasure>().yOffset, tempPos.z);
            }
            else
            {
                string[] tempAvailable = mainLogic.GetAvailableObjects();
                int randomIndex = Random.Range(0, tempAvailable.Length);

                mainLogic.spawnedObjects.Add(tempAvailable[randomIndex]);
                spawnedObject = chest.GetTreasure(tempAvailable[randomIndex], transform);
                Vector3 tempPos = spawnedObject.transform.position;
                spawnedObject.transform.position = new Vector3(tempPos.x, tempPos.y + spawnedObject.GetComponent<Treasure>().yOffset, tempPos.z);
            }
        }
    }
}
