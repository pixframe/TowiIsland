using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestController : MonoBehaviour {

    BirdsSingingManager manager;
    ParticleSystem notesToAir;
    AudioManager audioManager;
    int songOfTheNest;
    Vector3 dadBirdPos;


    void Start() {
        manager = FindObjectOfType<BirdsSingingManager>();
        dadBirdPos = transform.GetChild(3).transform.position;
    }

    public void SetANestSong(int data)
    {
        songOfTheNest = data;
    }

    public int GetTheNestSong()
    {
        return songOfTheNest;
    }

    void OnMouseUp()
    {
        if (manager.phase == BirdsSingingManager.GamePhase.Game)
        {
            Debug.Log("nest selected " + name);
            PlayTheNotes();
            manager.PlayANestSong(songOfTheNest);
        }
    }

    public void PlayTheNotes()
    {
        notesToAir = transform.GetChild(2).GetComponent<ParticleSystem>();
        audioManager = FindObjectOfType<AudioManager>();
        ChangeNotesColor(Color.black);
        notesToAir.Play();
        Invoke("StopTheNotes", audioManager.ClipDuration());
    }

    public void PlayTheNotes(bool answerIs)
    {
        if (answerIs)
        {
            ChangeNotesColor(Color.green);
        }
        else
        {
            ChangeNotesColor(Color.red);
        }
        notesToAir.Play();
        Invoke("StopTheNotes", audioManager.ClipDuration());
    }

    void StopTheNotes()
    {
        notesToAir.Stop();
    }

    public Vector3 DadPosition()
    {
        return dadBirdPos;
    }

    public void ChangeNotesColor(Color notesFarben)
    {
        var m = notesToAir.main;
        m.startColor = notesFarben;
    }
}
