using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioManager : MonoBehaviour {
    //These are the public audiclips needed to perform this tasks
    //These are the audios play in the bird game
    [Header("Birds Game")]
    public AudioClip[] birdsSongsCategoryTransportation;
    public AudioClip[] birdsSongsCategoryObjects;
    public AudioClip[] birdsSongsCategoryHumans;
    public AudioClip[] birdsSongsCategoryTools;
    public AudioClip[] birdsSongsCategoryMusicInstruments;
    public AudioClip[] birdsSongsCategoryWeather;
    public AudioClip[] birdsSongsCategoryPlaces;
    public AudioClip[] birdsInstructions;

    //These are the components need in this object to play
    AudioSource master;

	// Use this for initialization
	void Start () {
        master = GetComponent<AudioSource>();
        DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    #region Usefull Comands

    public void StopTheAudio()
    {
        master.Stop();
    }

    public float GetClipLenght() {
        return master.clip.length;
    }

    public bool IsPlayingSound() {
        return master.isPlaying;
    }

    void ChangeTheClipAndPlay(AudioClip clip) {
        master.clip = clip;
        master.Play();
    }

    #endregion
    #region Specific Comands

    #region Bird Singing game

    //This will play the sound of the birds in the Bird singing game
    public void PlayBirdSing(int category, int index) {
        switch (category)
        {
            case 0:
                ChangeTheClipAndPlay(birdsSongsCategoryTransportation[index]);
                break;
            case 1:
                ChangeTheClipAndPlay(birdsSongsCategoryObjects[index]);
                break;
            case 2:
                ChangeTheClipAndPlay(birdsSongsCategoryHumans[index]);
                break;
            case 3:
                ChangeTheClipAndPlay(birdsSongsCategoryTools[index]);
                break;
            case 4:
                ChangeTheClipAndPlay(birdsSongsCategoryMusicInstruments[index]);
                break;
            case 5:
                ChangeTheClipAndPlay(birdsSongsCategoryWeather[index]);
                break;
            case 6:
                ChangeTheClipAndPlay(birdsSongsCategoryPlaces[index]);
                break;
            default:
                ChangeTheClipAndPlay(birdsSongsCategoryTransportation[index]);
                break;
        }
    }

    //this will retur the number of clips by category
    public int AudiosInCategory(int category) {
        switch (category) {
            case 0:
                return birdsSongsCategoryTransportation.Length;
            case 1:
                return birdsSongsCategoryObjects.Length;
            case 2:
                return birdsSongsCategoryHumans.Length;
            case 3:
                return birdsSongsCategoryTools.Length;
            case 4:
                return birdsSongsCategoryMusicInstruments.Length;
            case 5:
                return birdsSongsCategoryWeather.Length;
            case 6:
                return birdsSongsCategoryPlaces.Length;
            default:
                return 0;
        }
    }

    //This will play the instructions in the Bird Game
    public void PlayBirdInstructions(int index) {
        ChangeTheClipAndPlay(birdsInstructions[index]);
    }

    #endregion

    #endregion
}
