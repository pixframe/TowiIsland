using UnityEngine;
using System.Collections;

public class RiverObject : MonoBehaviour {
	
	public string id;
	public string zone;
	public bool reverse = false;
	float sinRotate=0;
	float sinTranslate=0;
	bool drag=false;
	bool correct=false;
	string inZone="";
	public float riverSpeed;
	Transform effect;
	enum Direction {None,Left,Right};
	Direction lastMovement=Direction.None;
	Vector3 lastPos=Vector3.zero;
	float deathTime=6;
	public float finalY=0;
	public float yOffset=0;
	bool emerge=true;
	MainRiver mainRef;
	public bool simulateDrop=false;
	Vector2 simulatePos;
	[HideInInspector]
	public bool tutorial=false;
	// Use this for initialization
	void Start () {
		mainRef= GameObject.Find("Main").GetComponent<MainRiver>();
		effect = transform.Find ("Stars");
		sinRotate = Random.Range (0, Mathf.PI * 2);
	}
	
	// Update is called once per frame
	void Update () {
		if (Mathf.Abs (transform.position.y - finalY) > 0.1f && emerge) {
			transform.position = new Vector3 (transform.position.x, Mathf.Lerp (transform.position.y, finalY, 0.1f), transform.position.z);
		} else {
			emerge=false;
		}
		//if (lastPos) {
		//	lastPos = new Vector3 (lastPos.x, lastPos.y, lastPos.z - riverSpeed * Time.deltaTime);
		//}
		if (!drag && lastMovement == Direction.None) {
			sinRotate += Time.deltaTime * 1f;
			SimulateMovement ();
		}else if (lastMovement == Direction.Right) {
			deathTime-=Time.deltaTime;
			transform.position = new Vector3 (transform.position.x+riverSpeed*Time.deltaTime, transform.position.y, transform.position.z);
			if(deathTime<=0)
				Destroy(this.gameObject);
		} else if (lastMovement == Direction.Left) {
			deathTime-=Time.deltaTime;
			transform.position = new Vector3 (transform.position.x-riverSpeed*Time.deltaTime, transform.position.y, transform.position.z);
			if(deathTime<=0)
				Destroy(this.gameObject);
		}
		if(drag&&((Input.GetMouseButtonUp(0)&&!tutorial)||simulateDrop)){

			drag=false;
			float screenPosition=0;
			if(simulateDrop)
				screenPosition=(float)simulatePos.x/(float)Screen.width;
			else
				screenPosition=(float)Input.mousePosition.x/(float)Screen.width;
			simulateDrop=false;
			string zoneName="";
			if(screenPosition<=mainRef.forestMargin)
			{
				zoneName="Forest";
			}else if(screenPosition>=mainRef.beachMargin)
			{
				zoneName="Beach";
			}

			if(zone==""){
				correct=false;
				inZone = zoneName;
			}else{
				if(zoneName==zone){
					if(reverse)
						correct=false;
					else
						correct=true;
				}else{
					if(reverse)
						correct=true;
					else
						correct=false;
				}
				inZone = zoneName;
			}

			if(inZone!=""){
				if(correct){
					int childCount=effect.transform.childCount;
					for(int i=0;i<childCount;i++)
					{
						//effect.transform.GetChild(i).GetComponent<ParticleEmitter>().emit=true;
					}
				}
				mainRef.addScore(correct);
				mainRef.activeObjects--;
				if(inZone=="Forest"){
					lastMovement=Direction.Left;
				}
				else{
					lastMovement=Direction.Right;
				}
			}else{
				transform.position=lastPos;
			}
			inZone="";
			lastPos=Vector3.zero;
			correct=false;
		}
	}
	void OnTriggerEnter (Collider other)
	{
		if(other.name=="End"){
			mainRef.activeObjects--;
			if(zone==""){
				mainRef.addScore(true);
				GameObject starsRef=GameObject.Find("Stars");
				//for(int i=0;i<starsRef.transform.childCount;i++){
				//	starsRef.transform.GetChild(i).GetComponent<ParticleEmitter>().Emit();
				//}
			}else{
				mainRef.addScore(false);
			}
			Destroy(gameObject);
		}
	}
	void OnTriggerExit (Collider other)
	{
		inZone = "";
		correct = false;
	}
	void OnMouseDrag() {
		if(!tutorial)
		{
			if(lastMovement==Direction.None)
			{
				if(!drag)
				{
					if(mainRef.configuration.sound==1)
						mainRef.soundMng.PlaySound(32,0,0.6f);
				}
				if (lastPos == Vector3.zero)
					lastPos = transform.position;
				//renderer.material.color -= Color.white * Time.deltaTime;
				Vector3 screenSpace = Camera.main.WorldToScreenPoint (transform.position);
				Vector3 curScreenSpace = new Vector3 (Input.mousePosition.x,Input.mousePosition.y,5);
				Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenSpace)/* + offset*/;
				transform.position = curPosition;
				drag = true;
			}
		}
	}
	public void SimulateDrag(Vector2 pos){
		if(lastMovement==Direction.None)
		{
			if(!drag)
			{
				if(mainRef.configuration.sound==1)
					mainRef.soundMng.PlaySound(32,0,0.6f);
			}
			simulatePos=pos;
			if (lastPos == Vector3.zero)
				lastPos = transform.position;
			//renderer.material.color -= Color.white * Time.deltaTime;
			Vector3 screenSpace = Camera.main.WorldToScreenPoint (transform.position);
			Vector3 curScreenSpace = new Vector3 (pos.x,pos.y,5);
			Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenSpace)/* + offset*/;
			transform.position = curPosition;
			drag = true;
		}
	}
	void SimulateMovement(){
		transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z - riverSpeed * Time.deltaTime);
		transform.eulerAngles=new Vector3(transform.eulerAngles.x,transform.eulerAngles.y ,Mathf.Sin(sinRotate)*20);
		sinRotate %= Mathf.PI * 2;
	}
}
