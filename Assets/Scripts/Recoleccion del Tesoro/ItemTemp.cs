using UnityEngine;
using System.Collections;

public class ItemTemp:MonoBehaviour
{
	public string id;
	public int number;

	public ItemTemp (string objectID, int quantity)
	{
		id = objectID;
		number = quantity;
	}
}
