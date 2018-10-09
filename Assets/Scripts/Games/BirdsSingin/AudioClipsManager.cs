using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipsManager : MonoBehaviour {

    public AudioClip[] audios;

    public AudioClip PlayTheNeededAudio(int index)
    {
        return audios[index];
    }
}
