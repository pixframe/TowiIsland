using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgeAndBuy : MonoBehaviour {

    /*This Script controlls the first activity of the evaluation in which
     the kid input his age and includes some data to buy a ticket */

    //This script controlls the overall of all the test 
    EvaluationController evaluationController;

    //The UI elemets that has to be transalated
    [Header("Translatables")]
    public Text[] translatables;
    public Text[] commonTranslatables;
    // Assset used for the text of the game
    public TextAsset textAsset;
    //string array that saves the strings of the game
    string[] textsOfGame;

    //The UI elements we need to this game
    [Header("UI Elemts")]
    public Button readyButton;
    public Button goBackButton;
    public InputField ageInput;
    public InputField birthdayInput;
    public InputField nameInput;
    public InputField placeInput;
    public InputField dateInput;
    public Toggle dontRembemberToogle;
    public GameObject agePanel;
    public GameObject namePanel;
    public GameObject placePanel;
    public GameObject datePanel;
    public GameObject boardOrnament;
    public GameObject ticketPanel;
    public GameObject tryPanel;
    public GameObject storyCanvas;
    public GameObject lightsImage;


    //Game Varaiables
    //this variable will record the age that the player input
    [System.NonSerialized]
    public int ageOfPlayer;
    //this variable will record the birthdate that the player input
    [System.NonSerialized]
    public string birthdayDateOfPlayer;
    //this is the name that the player input
    [System.NonSerialized]
    public string nameOfPlayer;
    //this is the input of the place that the player input
    [System.NonSerialized]
    public string placeOfPlayer;
    //this is the date that the player input today
    [System.NonSerialized]
    public string dateOfToday;

    //This script controlls all the audios in the evaluiation
    AudioManager audioManager;

    AudioClip[] audioInScene;
    AudioClip extraAudio;

    float latencyTimer;
    float rotationSpeed = -50f;
    bool isLatencyTime;
    bool isStoryTime;

	// Use this for initialization
	void Start () {
        evaluationController = FindObjectOfType<EvaluationController>();
        audioManager = FindObjectOfType<AudioManager>();
        audioInScene = Resources.LoadAll<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Evaluation/Scene_1");
        extraAudio = Resources.Load<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Evaluation/Phrases/phrases_01");
        audioManager.PlayClip(audioInScene[0]);
        SetTheText();
        isStoryTime = true;
        Invoke("StarTheGame", audioManager.ClipDuration() + 1f);
    }

    void StarTheGame()
    {
        storyCanvas.SetActive(false);
        readyButton.onClick.RemoveAllListeners();
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(audioInScene[1]);
        readyButton.onClick.AddListener(SetAgeInput);
        goBackButton.onClick.AddListener(InputAgain);
        agePanel.SetActive(true);
        tryPanel.SetActive(false);
        goBackButton.gameObject.SetActive(false);
        ticketPanel.SetActive(false);
        evaluationController.StarCounting();
        isLatencyTime = true;
        isStoryTime = false;
        ageInput.onValueChanged.AddListener(delegate { StopLatency(); });
        birthdayInput.onValueChanged.AddListener(delegate { StopLatency(); });
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    void Update()
    {
        if (isStoryTime)
        {
            lightsImage.GetComponent<RectTransform>().Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        if (isLatencyTime)
        {
            latencyTimer += Time.deltaTime;
        }
    }

    // set the regular formate for date
    void DateFormat()
    {
        birthdayDateOfPlayer = birthdayInput.text;
        //birthdayDateOfPlayer = birthdayInput[0].text.ToString() + "/" + birthdayInput[1].text.ToString() + "/" + birthdayInput[2].text.ToString();
    }

    //Checks if the input is correct and handle the answer
    void SetAgeInput(){
        if(IsInputCorrect()){
            ageOfPlayer = int.Parse(ageInput.text);
            DateFormat();
            if (birthdayDateOfPlayer == "")
            {
                birthdayDateOfPlayer = "NA";
            }
            evaluationController.SaveAgeProgress(ageOfPlayer, birthdayDateOfPlayer, latencyTimer);
            StartBuyActivity();
        }
        else
        {
            tryPanel.SetActive(true);
            goBackButton.gameObject.SetActive(true);
            audioManager.PlayClip(extraAudio);
        }
    }

    //this proccess check if everything is correct
    bool IsInputCorrect(){
        if (ageInput.text != "")
        {
            if (birthdayInput.text == "" && !dontRembemberToogle.isOn)
            {
                return false;
            }
            return true;
        } else
        {
            return false;
        }
    }

    //Inatiliaces the second part of this secction where you get the ticket and finish the buy
    void StartBuyActivity(){
        agePanel.SetActive(false);
        ticketPanel.SetActive(true);
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(audioInScene[2]);
        PrepareTheBuyPart(evaluationController.DifficultyLevel());
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(() => SetNameInput());
        evaluationController.StarCounting();
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    //this will send a text depending the input of the player
    void SetNameInput(){
        switch (evaluationController.DifficultyLevel())
        {
            case 0:
                if (nameInput.text != "")
                {
                    nameOfPlayer = nameInput.text;
                }else{
                    nameOfPlayer = "Sin Respuesta";
                }
                placeOfPlayer = "NA";
                dateOfToday = "NA";
                break;
            case 1:
                if(nameInput.text != ""){
                    nameOfPlayer = nameInput.text;
                }else{
                    nameOfPlayer = "Sin Respuesta";
                }
                if(placeInput.text != ""){
                    placeOfPlayer = placeInput.text;
                }else{
                    placeOfPlayer = "Sin Respuesta";
                }
                dateOfToday = "NA";
                break;
            case 2:
                if (nameInput.text != "")
                {
                    nameOfPlayer = nameInput.text;
                }
                else
                {
                    nameOfPlayer = "Sin Respuesta";
                }
                if (placeInput.text != "")
                {
                    placeOfPlayer = placeInput.text;
                }
                else
                {
                    placeOfPlayer = "Sin Respuesta";
                }
                if (dateInput.text != ""){
                    dateOfToday = dateInput.text;
                } else{
                    dateOfToday =  "Sin Respuesta";
                }
                break;
        }
        evaluationController.SaveBuyTicketProgress(nameOfPlayer, placeOfPlayer, dateOfToday);
    }



    //In this function we call the level of dificulty to change the possible diffiicult of things
    void PrepareTheBuyPart(int difficult){
        switch (difficult)
        {
            case 0:
                namePanel.SetActive(true);
                placePanel.SetActive(false);
                datePanel.SetActive(false);
                boardOrnament.SetActive(true);
                break;
            case 1:
                namePanel.SetActive(true);
                placePanel.SetActive(true);
                datePanel.SetActive(false);
                boardOrnament.SetActive(true);
                break;
            case 2:
                namePanel.SetActive(true);
                placePanel.SetActive(true);
                datePanel.SetActive(true);
                boardOrnament.SetActive(false);
                break;
            default:
                namePanel.SetActive(true);
                placePanel.SetActive(true);
                datePanel.SetActive(true);
                boardOrnament.SetActive(true);
                break;
        }
    }

    //this set the correct input iin the lenguage
    void SetTheText(){
        textAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Evaluation/Evaluation_01/Evaluation_Scene1");
        textsOfGame = TextReader.TextsToShow(textAsset);
        for (int i = 0; i < translatables.Length;i++){
            translatables[i].text = textsOfGame[i];
        }
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        commonTranslatables[0].text = TextReader.commonStrings[13];
        commonTranslatables[1].text = textsOfGame[4];
    }

    //quits the try again panel and let try to input again
    void InputAgain(){
        tryPanel.SetActive(false);
        goBackButton.gameObject.SetActive(false);
    }

    void StopLatency() {
        isLatencyTime = false;
        ageInput.onValueChanged.RemoveAllListeners();
        birthdayInput.onValueChanged.RemoveAllListeners();
    }

    void ReadyButtonOn()
    {
        readyButton.gameObject.SetActive(true);
    }
}
