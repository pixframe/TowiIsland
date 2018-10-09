using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveToPosition : MonoBehaviour
{
	//scripts var
	WhereIsTheBallLogic logicScript;
	Animator anim;
	
	//logic var
	Vector3 pos = Vector3.zero;
	public bool inPos = false;
	bool leaving = false;
	public bool posAssigned = false;

	//Game var
	public float time = .3f;
	Vector3 iniPos;
	public float movingSpeed;
	GameObject positions1;
	GameObject positions2;
	public bool move;


	
	
	
	// Use this for initialization
	void Start ()
	{
		iniPos = transform.position;
		logicScript = GameObject.FindGameObjectWithTag("Main").GetComponent<WhereIsTheBallLogic>();
		positions1 = GameObject.Find("Positions1");
		positions2 = GameObject.Find("Positions2");
		anim = transform.Find("MonModel").GetComponent<Animator>();
		leaving = true;
		move = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(logicScript.numberOfMonkeys > 0 && !posAssigned)
		{
			if(name.Contains("1"))
			{
				if(logicScript.numberOfMonkeys == 3)
				{
					pos = positions1.transform.Find("Pos2").transform.position;
					logicScript.plats.Add(positions1.transform.Find("Pos2").transform.Find("Platform").gameObject);
					posAssigned = true;
				
				}
				else if(logicScript.numberOfMonkeys == 4)
				{
					pos = positions2.transform.Find("Pos1").transform.position;
					logicScript.plats.Add(positions2.transform.Find("Pos1").transform.Find("Platform").gameObject);
					posAssigned = true;
					
				}
				else if(logicScript.numberOfMonkeys == 5)
				{
					pos = positions1.transform.Find("Pos1").transform.position;
					logicScript.plats.Add(positions1.transform.Find("Pos1").transform.Find("Platform").gameObject);
					posAssigned = true;
					
				}
				
			}
			else if(name.Contains("2"))
			{
				if(logicScript.numberOfMonkeys == 3)
				{
					pos = positions1.transform.Find("Pos3").transform.position;
					logicScript.plats.Add(positions1.transform.Find("Pos3").transform.Find("Platform").gameObject);
					posAssigned = true;
					
				}
				else if(logicScript.numberOfMonkeys == 4)
				{
					pos = positions2.transform.Find("Pos2").transform.position;
					logicScript.plats.Add(positions2.transform.Find("Pos2").transform.Find("Platform").gameObject);
					posAssigned = true;
				}
				else if(logicScript.numberOfMonkeys == 5)
				{
					pos = positions1.transform.Find("Pos2").transform.position;
					logicScript.plats.Add(positions1.transform.Find("Pos2").transform.Find("Platform").gameObject);
					posAssigned = true;
				}
			}
			else if(name.Contains("3"))
			{
				if(logicScript.numberOfMonkeys == 3)
				{
					pos = positions1.transform.Find("Pos4").transform.position;
					logicScript.plats.Add(positions1.transform.Find("Pos4").transform.Find("Platform").gameObject);
					posAssigned = true;
				}
				else if(logicScript.numberOfMonkeys == 4)
				{
					pos = positions2.transform.Find("Pos3").transform.position;
					logicScript.plats.Add(positions2.transform.Find("Pos3").transform.Find("Platform").gameObject);
					posAssigned = true;
				}
				else if(logicScript.numberOfMonkeys == 5)
				{
					pos = positions1.transform.Find("Pos3").transform.position;
					logicScript.plats.Add(positions1.transform.Find("Pos3").transform.Find("Platform").gameObject);
					posAssigned = true;
				}
				
			}
			else if(name.Contains("4"))
			{
				if(logicScript.numberOfMonkeys == 4)
				{
					pos = positions2.transform.Find("Pos4").transform.position;
					logicScript.plats.Add(positions2.transform.Find("Pos4").transform.Find("Platform").gameObject);
					posAssigned = true;
				}
				else if(logicScript.numberOfMonkeys == 5)
				{
					pos = positions1.transform.Find("Pos4").transform.position;
					logicScript.plats.Add(positions1.transform.Find("Pos4").transform.Find("Platform").gameObject);
					posAssigned = true;
				}
				
			}
			else if(name.Contains("5"))
			{
				if(logicScript.numberOfMonkeys == 5)
				{
					pos = positions1.transform.Find("Pos5").transform.position;
					logicScript.plats.Add(positions1.transform.Find("Pos5").transform.Find("Platform").gameObject);
					posAssigned = true;
				}
				
			}
		}
		if(logicScript.state == "Start" || logicScript.state == "IncreaseDifficulty" || logicScript.state == "DecreaseDifficulty" || logicScript.state == "SameLevel")
		{
			if(move)
			if(!leaving)
			{
			
				if(Vector3.Distance(iniPos, transform.position) >= Vector3.Distance(iniPos, pos))
				{
					if(name.Contains("3") || name.Contains("4") || name.Contains("5"))
					{
						transform.Find("MonModel").transform.Rotate(new Vector3(0, 270, 0));
					}
					else
					{

						transform.Find("MonModel").transform.Rotate(new Vector3(0, 90, 0));
					}

					anim.SetFloat("Speed", 0f);
					inPos = true;
					transform.position = pos;
					move = false;
				}	
			}
			else
			{

				if(Vector3.Distance(transform.position, pos) >= Vector3.Distance(iniPos, pos) && Vector3.Distance(transform.position, iniPos) != 0)
				{
					if(name.Contains("3") || name.Contains("4") || name.Contains("5"))
					{
						transform.Find("MonModel").transform.Rotate(new Vector3(0, 270, 0));
					}
					else
					{

						transform.Find("MonModel").transform.Rotate(new Vector3(0, 90, 0));
					}
					anim.SetFloat("Speed", 0f);
					inPos = true;
					transform.position = iniPos;
					move = false;
				}
			}	
		}
		
		
		if(logicScript.state == "Start" || logicScript.state == "Move")
		{
			if(!logicScript.moveToPos)
			{
				inPos = false;
			}	
		}
		
		
	}
	public void MoveToPosFunc()
	{

		move = true;
//		transform.position = Vector3.Lerp(transform.position, pos, time);
		anim.SetFloat("Speed", 10f);
		if(leaving)
		if(name.Contains("3") || name.Contains("4") || name.Contains("5"))
		{
			transform.Find("MonModel").transform.Rotate(new Vector3(0, 270, 0));
		}
		else
		{

			transform.Find("MonModel").transform.Rotate(new Vector3(0, 90, 0));
		}
		movingSpeed = 10;
		transform.Translate((pos - transform.position).normalized * Time.deltaTime * movingSpeed);
		leaving = false;
	}
	public void MoveOutOfPosFunc()
	{
	
		move = true;
		anim.SetFloat("Speed", 10f);
		if(leaving == false)
		{
			if(name.Contains("3") || name.Contains("4") || name.Contains("5"))
			{
				transform.Find("MonModel").transform.Rotate(new Vector3(0, 270, 0));
			}
			else
			{
				transform.Find("MonModel").transform.Rotate(new Vector3(0, 90, 0));
			}
		}

		movingSpeed = 10;
		movingSpeed += 5;
		transform.Translate((iniPos - transform.position).normalized * Time.deltaTime * movingSpeed);
//		transform.position = Vector3.Lerp(transform.position, iniPos, time);
		leaving = true;
	}
}
