using UnityEngine;
using System.Collections;

public class DragBird : MonoBehaviour
{
	public int birdIdx;
	bool drag = false;
	public bool active=false;
	bool inPosition=false;
	public GameObject halo;
	Vector3 birdPosition;
	Vector3 originalPosition;
	public float xOffset;
	public float yOffset;
	public bool available;
	public int soundIndex;
	Vector3 nestPosition;
	public bool wrongNest;
	SoundTree mainTree;
	float holdTime = 0;
	float speed = 0.03f;
	bool moving;
	Color originalColor;
	Color highlightColor;
	bool highlightUp=true;
	bool highlight=false;
	private Animator anim;		
	SkinnedMeshRenderer meshRenderer;
	SkinnedMeshRenderer picoSup;
	SkinnedMeshRenderer picoInf;
	bool open=false;
	bool close=false;
	bool clicked;
	
	// Use this for initialization
	void Start ()
	{
		halo = transform.Find("Halo").gameObject;
		birdPosition = transform.position;
		wrongNest = false;
		meshRenderer = transform.Find ("pajaro_mesh").GetComponent<SkinnedMeshRenderer> ();
		picoSup = transform.Find ("PicoSup").GetComponent<SkinnedMeshRenderer> ();
		picoInf = transform.Find ("PicoInf").GetComponent<SkinnedMeshRenderer> ();
		anim = GetComponent<Animator>();
		anim.ForceStateNormalizedTime(Random.Range(0.0f, 1.0f));
		originalColor=meshRenderer.material.color;
		highlightColor=Color.white;
		moving=false;
		mainTree = GameObject.Find ("SoundTreeMain").GetComponent<SoundTree> ();
		originalPosition = transform.position;
		available = true;
		nestPosition = Vector3.zero;
	}
	
	void OnTriggerEnter (Collider other)
	{
		Debug.Log("Collision");
		if (other.tag == "Nest") 
		{
			if(other.GetComponent<Nest> ().soundIndex == soundIndex)
				nestPosition = other.transform.position;
		}
	}
	void OnTriggerStay (Collider other){
		if (other.tag == "Nest"&&other.GetComponent<Nest> ().soundIndex != soundIndex) 
		{
			wrongNest=true;
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Nest") 
		{
			if(other.GetComponent<Nest> ().soundIndex == soundIndex)
			{
				if(available)
					nestPosition = Vector3.zero;
			}
			wrongNest=false;
		}
	}
	
	void OnMouseEnter ()
	{
		//renderer.material.color = mouseOverColor;
	}

	void OnMouseExit ()
	{
		//renderer.material.color = originalColor;
	}

	void OnMouseDrag ()
	{
		if(mainTree.state=="Play")
		{
			holdTime += Time.deltaTime;
		}
	}

	void OnMouseDown()
	{
		switch (mainTree.state) {
			case "Birds":
			if(!mainTree.source.isPlaying && (mainTree.tutorialPhase == 1 ||mainTree.tutorialPhase == 2)&& !mainTree.tutSource.isPlaying)
			{
				OpenMouth();
				halo.SetActive(false);
				clicked = true;
				mainTree.PlaySound (soundIndex);
				holdTime = 0;
			}
			break;
		}
	}

	public void OpenMouth()
	{
		open = true;
	}

	public void CloseMouth()
	{
		close = true;
	}

	public void SetBird(Vector3 pos)
	{
		speed = 0.03f;
		active = true;
		inPosition = false;
		birdPosition = pos;
		anim.SetBool("Flying",true);
	}
	public void SetBirdAway()
	{
		speed = 0.01f;
		active = false;
		inPosition = false;
		birdPosition = originalPosition;
		anim.SetBool("Flying",true);
	}
	public void Reset()
	{
		//transform.position=birdPosition;
		wrongNest = false;
		available=true;
		moving = false;
		drag=false;
		soundIndex=-1;
		holdTime=0;
		anim.SetBool("Flying",false);
		nestPosition = Vector3.zero;
	}
	public void StopFlying()
	{
		//anim.SetBool("Flying",false);
	}

	public void SetHighlight(bool mode)
	{
		if(mode)
		{
			highlight=true;
			highlightUp=true;
		}
		else
		{
			highlight=false;
			meshRenderer.material.color=originalColor;
		}
	}
	
	void Update ()
	{
		if(open)
		{
			close=false;
			float weight=picoSup.GetBlendShapeWeight(0);
			if(weight+100*Time.deltaTime>=100)
			{
				if(!mainTree.source.isPlaying)
				{
					open=false;
					CloseMouth();
				}
				picoSup.SetBlendShapeWeight (0, 100);
				picoInf.SetBlendShapeWeight (0, 100);
			}else
			{
				picoSup.SetBlendShapeWeight (0, weight+400*Time.deltaTime);
				picoInf.SetBlendShapeWeight (0, weight+400*Time.deltaTime);
			}

		}
		if (close) {
			open=false;
			float weight=picoSup.GetBlendShapeWeight(0);
			if(weight-100*Time.deltaTime<=0)
			{
				close=false;
				picoSup.SetBlendShapeWeight (0, 0);
				picoInf.SetBlendShapeWeight (0, 0);
			}else
			{
				picoSup.SetBlendShapeWeight (0, weight-400*Time.deltaTime);
				picoInf.SetBlendShapeWeight (0, weight-400*Time.deltaTime);
			}
		}
		if (mainTree.state != "Pause")
		{
			if (!inPosition) 
			{
				transform.position = Vector3.Lerp (transform.position, birdPosition, speed);
				if (Vector3.Distance (transform.position, birdPosition) < 0.01f) 
				{
					inPosition = true;
					anim.SetBool ("Flying", false);
				}
			}
			if (highlight)
			{
				if (highlightUp)
				{
					Vector4 temp = Vector4.Lerp (meshRenderer.material.color, highlightColor, 0.1f);
					meshRenderer.material.color = temp;
					if (Vector4.Distance (meshRenderer.material.color, highlightColor) < 0.1)
					{
						highlightUp = false;
					}
												
				} 
				else
				{
					Vector4 temp = Vector4.Lerp (meshRenderer.material.color, originalColor, 0.1f);
					meshRenderer.material.color = temp;
					if (Vector4.Distance (meshRenderer.material.color, originalColor) < 0.1)
					{
						highlightUp = false;
					}
												
				}
			}
			if (moving) 
			{
				transform.position = Vector3.Lerp (transform.position, birdPosition, 0.03f);
				if (Vector3.Distance (transform.position, birdPosition) < 0.01f && mainTree.branchesInPlace) 
				{
						//transform.position=originalPosition;
						moving = false;
						anim.SetBool ("Flying", false);
				}
			}
		
			switch (mainTree.state) {
			case "LevelUp":

					break;
			case "Birds":

				if(clicked && mainTree.birdsListenedPre[birdIdx] == 0)
				{
					if(!mainTree.source.isPlaying)
					{
						mainTree.birdsListenedPre[birdIdx]++;
					}
				}

					break;
			case "Play":
				clicked = false;
					if (holdTime > 0.13f && !mainTree.tutSource.isPlaying)
							drag = true;
				if (available) 
				{
					if (Input.GetMouseButtonUp (0)) 
					{
							Debug.Log(wrongNest);
							
						if (holdTime > 0 && !drag && !mainTree.tutSource.isPlaying) 
						{
							OpenMouth();
							mainTree.PlaySound (soundIndex);
							mainTree.birdsListened[birdIdx]++;
						}
						if(wrongNest)
						{	
							mainTree.errors++;
                            mainTree.ResetPickTimer();
							mainTree.scoreEffects.DisplayError(mainTree.configuration.sound==1);
                            mainTree.scoreScript.prevCorMult=0;
						}
						holdTime = 0;
						if (nestPosition != Vector3.zero) 
						{
                            mainTree.ResetPickTimer();
                            mainTree.scoreEffects.DisplayScore(mainTree.scoreScript.TempScoreSum(), mainTree.configuration.sound == 1);
                            mainTree.scoreScript.prevCorMult++;
							mainTree.birdsSet++;
							mainTree.PlaySound (soundIndex);
							available = false;
							transform.position = new Vector3(nestPosition.x,nestPosition.y+0.07f,nestPosition.z);
							drag = false;
						}
						drag = false;	
					}
							
					if (!drag && available && transform.position != birdPosition) 
					{
									
						transform.position = Vector3.Lerp (transform.position, birdPosition, 0.3f);
					}

					if (drag) 
					{
						anim.SetBool ("Flying", true);
						Vector3 screenSpace = Camera.main.WorldToScreenPoint (transform.position);
						//Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
						//offset = new Vector3 (Input.mousePosition.x - screenSpace.x, Input.mousePosition.y - screenSpace.y, 0);
						//Debug.Log(screenSpace);
						//if (Input.GetMouseButton(0)) {
						Vector3 curScreenSpace = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
						Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenSpace)/* + offset*/;
						curPosition.y -= 0.2f;
						transform.position = curPosition;
									//}
					} 
					else 
					{
						anim.SetBool ("Flying", false);
					}
				}
					break;
			}
		}
	}
	public void Move ()
	{
		Debug.Log("e");
		moving = true;
		anim.SetBool("Flying",true);
	}
}
