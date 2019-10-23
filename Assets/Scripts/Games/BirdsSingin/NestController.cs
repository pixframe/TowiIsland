using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestController : MonoBehaviour 
{
    ParticleSystem notesToAir;
    AudioManager audioManager;
    int songOfTheNest;
    Vector3 dadBirdPos;


    void Start() 
    {
        dadBirdPos = transform.Find("Dad Pos").position;
        notesToAir = transform.GetComponentInChildren<ParticleSystem>();
    }

    public void SetANestSong(int data)
    {
        songOfTheNest = data;
    }

    public int GetTheNestSong()
    {
        return songOfTheNest;
    }

    /*void OnMouseUp()
    {
        if (manager.phase == BirdsSingingManager.GamePhase.Game)
        {
            Debug.Log("nest selected " + name);
            PlayTheNotes();
            manager.PlayANestSong(songOfTheNest);
        }
    }*/

    public void PlayTheNotes()
    {
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
