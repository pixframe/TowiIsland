using UnityEngine;
using System.Collections;

public class ArrowRotation : MonoBehaviour
{
		GameObject player;
		// Use this for initialization
		void Start ()
		{
			player = GameObject.Find("Character").gameObject;
		}
	
		// Update is called once per frame
		void Update ()
		{
			transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
		}
}
