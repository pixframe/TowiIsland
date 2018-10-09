using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent (typeof (AudioSource))]

public class FloorDetection : MonoBehaviour
{

	public bool disableFunc;

	//footstep map
	public Texture2D footStepMap;

	public bool useMap;

	//texture info
	Color[] pixelColors;
	bool pixelsSet = false;

	public string type;
	public AudioClip[] grassClips;
	public AudioClip[] sandClips;
	public AudioClip[] waterClips;
	public AudioClip[] woodClips;
	public AudioClip[] jumpClips;
	AudioSource aud;
	// Use this for initialization
	void Start ()
	{
		switch(SceneManager.GetActiveScene().name)
		{
			case "RecoleccionTesoro":
				useMap = true;
			    break;
			case "Selection":
				useMap = false;
			    break;
            default:
                useMap = true;
                break;
		}

		aud = GetComponent<AudioSource>();
		if(useMap)
			pixelColors = footStepMap.GetPixels();
	}

	// Update is called once per frame
	void Update ()
	{

		if(!disableFunc)
		{
			if(useMap)
			{
				Ray ray = new Ray(transform.position, -transform.up);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, .1f))
				{
					Color hitColor;
					Vector2 pixCoor = hit.textureCoord;
					float xPos = pixCoor.x * (float)footStepMap.width;
					float yPos = pixCoor.y * (float)footStepMap.height;
					hitColor = pixelColors[((int)yPos * footStepMap.width) + (int)xPos];

					if(hitColor == new Color(1,0,0,1))
					{

						type = "sand";
					}
					else if(hitColor == new Color(0,1,0,1))
					{

						type = "grass";
					}
					else if(hitColor == new Color(0,0,1,1))
					{

						type = "water";
					}
					else
					{
						//Debug.Log("sand1");
						//type = "sand";
					}

				}
			}
			else 
			{
				Ray ray = new Ray(transform.position, -transform.up);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, .1f))
				{
					if(hit.transform.gameObject.tag == "Ground")
					{
						FloorProp floor = hit.transform.GetComponent<FloorProp>();
						if(floor.Type == TypeOfFloor.grass)
						{
							type = "grass";
						}
						else if( floor.Type == TypeOfFloor.sand)
						{
							type = "sand";
						}
						else if(floor.Type == TypeOfFloor.water)
						{
							type = "water";
						}
						else if(floor.Type == TypeOfFloor.wood)
						{
							type = "wood";
						}
					}
					
				}
			}
		}


	}

	public void FootStep()
	{
		if(type == "grass")
		{
			int i = Random.Range(0, grassClips.Length);
			aud.clip = grassClips[i];
			aud.Play();
		}
		else if(type == "sand")
		{
			int i = Random.Range(0, sandClips.Length);
			aud.clip = sandClips[i];
			aud.Play();
		}
		else if(type == "water")
		{
			int i = Random.Range(0, waterClips.Length);
			aud.clip = waterClips[i];
			aud.Play();
		}
		else if(type == "wood")
		{
			int i = Random.Range(0, woodClips.Length);
			aud.clip = woodClips[i];
			aud.Play();
		}
	}

	public void Jump()
	{
		int i = Random.Range(0, jumpClips.Length);
		aud.clip = jumpClips[i];
		aud.Play();
	}
}
