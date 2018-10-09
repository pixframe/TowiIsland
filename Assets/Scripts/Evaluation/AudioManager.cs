using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    //This is the auido source clipp to this object
    AudioSource master;

    //These are audios to be played by the evaluation
    /*public AudioClip tryAgainAudio;
    public AudioClip[] onomatopeiaAudios;
    public AudioClip[] addableAudios;
    public AudioClip[] commonAudios;
    public AudioClip[] evaluation_Scene_1_Sounds;
    public AudioClip[] evaluation_Scene_2_Sounds;
    public AudioClip[] evaluation_Scene_3_Sounds;
    public AudioClip[] evaluation_Scene_4_Sounds;
    public AudioClip[] evaluation_Scene_5_Sounds;
    public AudioClip[] evaluation_Scene_6_Sounds;
    public AudioClip[] evaluation_Scene_7_Sounds;
    public AudioClip[] objectsToSay;
    public AudioClip[] flightNumbersToCall;
    public AudioClip[] birdsSounds;*/


    float lenghts;
	// Use this for initialization
	void Awake () {
        if (FindObjectsOfType<AudioManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        master = GetComponent<AudioSource>();
        DontDestroyOnLoad(this.gameObject);
	}

    public bool IsPlayingTheClip()
    {
        return master.isPlaying;
    }

    public float ClipDuration(){
        if (lenghts == 0f)
        {
            return master.clip.length;
        }
        else
        {
            return lenghts;
        }
    }

    public void StopTheAudio() {
        master.Stop();
    }

    public void PlayClip(AudioClip clipAudio1)
    {
        lenghts = 0;
        master.clip = clipAudio1;
        master.Play();
    }

    public void PlayClip(AudioClip clipAudio1, AudioClip clipAudio2)
    {
        lenghts = clipAudio1.length + clipAudio2.length;
        master.clip = clipAudio1;
        master.Play();
        StartCoroutine(PlayMoreThat1Clip(clipAudio2));
    }

    public void PlayClip(AudioClip clipAudio1, AudioClip clipAudio2, AudioClip clipAudio3)
    {
        lenghts = clipAudio1.length + clipAudio2.length + clipAudio3.length;
        master.clip = clipAudio1;
        master.Play();
        StartCoroutine(PlayMoreThat1Clip(clipAudio2, clipAudio3));
    }

    IEnumerator PlayMoreThat1Clip(AudioClip clipToPlay)
    {
        yield return new WaitForSeconds(master.clip.length);
        PlayClip(clipToPlay);
    }

    IEnumerator PlayMoreThat1Clip(AudioClip clipToPlay, AudioClip clipToPlay2)
    {
        yield return new WaitForSeconds(master.clip.length);
        PlayClip(clipToPlay);
        yield return new WaitForSeconds(master.clip.length);
        PlayClip(clipToPlay2);
    }
}
