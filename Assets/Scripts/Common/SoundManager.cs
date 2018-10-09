using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
	public AudioClip[]sounds;
	public Transform[] location;
	public float volume;
	List<int> queue;
    [HideInInspector]
	public bool queuePlaying=false;
	public bool sendMessages;
	[HideInInspector]
	public bool pauseQueue=false;
	bool firstQueueElement;
	List<bool> messages;
	List<bool> pause;
	// Use this for initialization
	void Awake () {
		queue = new List<int> ();
		messages = new List<bool> ();
		pause = new List<bool> ();
		firstQueueElement = true;
	}

	public bool IsPlaying()
	{
		return GetComponent<AudioSource>().isPlaying;
	}
	public void PlaySound(int index)
	{
		if (index < sounds.Length) {
			GetComponent<AudioSource>().Stop();
			GetComponent<AudioSource>().clip=sounds[index];
			GetComponent<AudioSource>().Play();
		}
	}
	public void PlaySoundQueue(int[] indexs)
	{
		queue.AddRange (indexs);
		if (queue.Count > 0) {
			GetComponent<AudioSource>().Stop();
			if(queue[0]<sounds.Length)
			{
				GetComponent<AudioSource>().clip=sounds[queue[0]];
				GetComponent<AudioSource>().Play();
				queue.RemoveAt(0);
				queuePlaying=true;
			}
		}
	}
	public void AddSoundToQueue(int index)
	{
		queue.Add (index);
		messages.Add (sendMessages);
		pause.Add (true);
		queuePlaying=true;
	}
	public void AddSoundToQueue(int index,bool play)
	{
		queue.Add (index);
		messages.Add (sendMessages);
		pause.Add (true);
		if(play)
			queuePlaying=true;
	}
	public void AddSoundToQueue(int index,bool play,bool send)
	{
		queue.Add (index);
		messages.Add (send);
		pause.Add (false);
		if(play)
			queuePlaying=true;
	}
	public void PlayQueue()
	{
		queuePlaying = true;
		//Debug.Log (queue.Count);
	}
	public void PlaySound(int index,int locationIdx)
	{
		if(index<sounds.Length&&locationIdx<location.Length)
			AudioSource.PlayClipAtPoint (sounds[index],location[locationIdx].position,volume);
	}

	public void PlaySound(int index,float vol)
	{
		if(index<sounds.Length)
		{	
			GetComponent<AudioSource>().volume=vol;
			GetComponent<AudioSource>().Stop();
			GetComponent<AudioSource>().clip=sounds[index];
			GetComponent<AudioSource>().Play();
		}
	}
	public void PlaySound(int index,int locationIdx,float vol)
	{
		if(index<sounds.Length&&locationIdx<location.Length)
			AudioSource.PlayClipAtPoint (sounds[index],location[locationIdx].position,vol);
	}
	public void ContinueQueue()
	{
		if(queue.Count>0)
		{
			GetComponent<AudioSource>().clip=sounds[queue[0]];
			queue.RemoveAt(0);
			GetComponent<AudioSource>().Play();
		}
		queuePlaying = true;
	}


	// Update is called once per frame
	void Update () {
		//Debug.Log (queuePlaying);
		if(queuePlaying)
		{
			if(!GetComponent<AudioSource>().isPlaying)
			{
				if(queue.Count>0)
				{
					GetComponent<AudioSource>().Stop();
					if(queue[0]<sounds.Length)
					{
						if(!firstQueueElement)
						{	

							if(pause.Count>0&&pause[0]){
							//if(pauseQueue){
								Debug.Log("PausedQueue");
								queuePlaying=false;
							}else{
								GetComponent<AudioSource>().clip=sounds[queue[0]];
								queue.RemoveAt(0);
								GetComponent<AudioSource>().Play();
							}
							if(pause.Count>0){
								pause.RemoveAt(0);
							}
							if(sendMessages){
								if(messages.Count>0&&!messages[0])
								{

								}else{
									SendMessage("ClipEnded");
								}
								if(messages.Count>0)
									messages.RemoveAt(0);
							}
						}else
						{
							GetComponent<AudioSource>().clip=sounds[queue[0]];
							queue.RemoveAt(0);
							GetComponent<AudioSource>().Play();
						}
						firstQueueElement=false;
					}
				}else{
					messages.Clear();
					pause.Clear();
					queuePlaying=false;
					firstQueueElement=true;
					if(sendMessages)
					{
						SendMessage("InstructionsEnded");
					}
				}
			}
		}
	}
}
