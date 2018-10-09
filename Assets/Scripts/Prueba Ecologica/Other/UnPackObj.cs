using UnityEngine;
using System.Collections;

public class UnPackObj : MonoBehaviour
{
	UnPackLogic unPackScript;
	public bool unPackObj;
	public bool objUsed;
	public string category;
	public Vector3 iniPos;
	public Quaternion iniRot;
	void Start()
	{
		unPackScript = GameObject.Find("UnPack").GetComponent<UnPackLogic>();
		iniPos = transform.position;
		iniRot = transform.rotation;
	}
	void Update()
	{
		if(unPackScript.state == "ShowImage" && !unPackScript.determine)
		{

			transform.position = iniPos;
			objUsed = false;
		}
	}
	public void IniPos()
	{
		transform.position = iniPos;
		transform.rotation = iniRot;
	}
}
