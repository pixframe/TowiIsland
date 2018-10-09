using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Boomlagoon.JSON;

public class LastSceneManager : MonoBehaviour {

    EvaluationController evaluationController;
    ProgressHandler progressHandler;
    AudioManager audioManager;
    AudioSource player;
    public TextAsset textAsset;

    public Text storyText;

    AudioClip[] audioInScene;

    string[] stringsToShow;

    bool canMove = false;

    // Use this for initialization
    void Start ()
    {
        stringsToShow = TextReader.TextsToShow(textAsset);
        audioManager = FindObjectOfType<AudioManager>();
        evaluationController = FindObjectOfType<EvaluationController>();
        progressHandler = FindObjectOfType<ProgressHandler>();
        player = audioManager.GetComponent<AudioSource>();

        audioInScene = Resources.LoadAll<AudioClip>("Audios/Evaluation/Scene_8");

        storyText.text = stringsToShow[0];
        audioManager.PlayClip(audioInScene[0]);

        progressHandler.PostEvaluationData(this);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!player.isPlaying && canMove)
        {
            canMove = false;
            evaluationController.FinishEvaluation();
        }
	}

    public void MoveToMenu()
    {
        canMove = true;
    }
    /*IEnumerator PostEvaluation(JSONObject json)
    {

    }*/
}
