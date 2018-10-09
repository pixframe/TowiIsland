using UnityEngine;
using System.Collections;

public class Objects : MonoBehaviour
{
	public string shadow;
	void Start()
	{
		shadow = this.name + "Sombra";
	}
		
}
