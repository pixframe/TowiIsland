using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowGameController : MonoBehaviour {
    //scripts needed for the game
    EvaluationController evaluationController;
    AudioManager audioManager;
    FlashProbes probes;
    ParticleSystem arrowExplotion;
    AudioClip[] audioInScene;
    AudioClip ohNoAudio;
    AudioClip veryGoodAudio;

    //In this list we will save the data
    List<float> latencies;
    List<float> answers;

    //this will handle the possible tutorial error
    enum TutorialErrors {None, Miss, BadGreen, BadDireccion, NotDef, DontTouch}
    TutorialErrors error;

    int direction;
    int playerDirection;
    int difficulty;

    //the colors are 0 green, 1 red, 2 prple, 3 orange
    //intrusion equal 0
    List<int> farbenOrdnungTutorial = new List<int>
    {
        2,0,3
    };
    List<int> farbenOrndung = new List<int>
    {
        2,0,1,2,3,0,1,0,2,3,2,1,0,1,3,0,2,3,1,1,0,3,0,0,2,3,0,1,2,3
    };

    //The direccion order 1 is right and -1 is left
    List<int> directionOrderTutorial = new List<int>
    {
        1,1,-1
    };
    List<int> directionOrder = new List<int>
    {
        1,1,-1,1,-1,-1,1,-1,-1,1,1,1,-1,-1,1,-1,1,-1,-1,1,1,-1,1,-1,-1,-1,1,-1,1,1
    };

    List<float> arrowsLatencies = new List<float>();
    List<float> greenArrowLatencies = new List<float>();
    List<float> arrowBadLatencies = new List<float>();
    List<float> greenBadlatencies = new List<float>();

    //here we have a record of every interaction this will save the answer with the arrow 0 = good, 1 = bad, 2 = mised
    List<int> fullInteractions = new List<int>();
    List<float> fullLatencies = new List<float>();

    //This is the ui needed in this section
    public GameObject instruccionPanel;
    public Button leftArrowButton;
    public Button rightArrowButton;
    public Button readyButton;
    public Text instruccionText;

    //this is the arrow and all the posibilitie it has to transform
    public GameObject whiteArrow;
    public Color[] farben;

    public TextAsset textAsset;
    string[] stringsToShow;

    //All the data we need to save is in this variables
    int arrowIndex;
    int arrowTutorialIndex;
    int goodAnswer;
    int goodGreenAnswer;
    int badAnswer;
    int badGreenAnswer;
    int missAnswer;
    int missGreenAnswer;
    float timeToInput = 2.5f;
    float arrowLatencie;
    bool needInput;
    bool tutorialMode;

    // Use this for initialization
    void Start()
    {
        whiteArrow.SetActive(false);
        evaluationController = FindObjectOfType<EvaluationController>();
        audioManager = FindObjectOfType<AudioManager>();
        audioInScene = Resources.LoadAll<AudioClip>("Audios/Evaluation/Scene_5");
        ohNoAudio = Resources.Load<AudioClip>("Audios/Evaluation/Frases/Frase_4");
        veryGoodAudio = Resources.Load<AudioClip>("Audios/Evaluation/Frases/Frase_5");
        probes = FindObjectOfType<FlashProbes>();
        arrowExplotion = FindObjectOfType<ParticleSystem>();
        if (probes != null)
        {
            difficulty = probes.ArrowData();
        }
        else
        {
            int age = evaluationController.GetTheAgeOfPlayer();
            if (age < 7)
            {
                difficulty = 0;
            }
            else
            {
                difficulty = 1;
            }
        }
        stringsToShow = TextReader.TextsToShow(textAsset);
        evaluationController.StarCounting();
        rightArrowButton.onClick.AddListener(PressRight);
        leftArrowButton.onClick.AddListener(PressLeft);
        SetTutotrial();
    }

    //This function will set the fisrt frame of the tutorial
    void SetTutotrial()
    {
        audioManager.PlayClip(audioInScene[0]);
        instruccionText.text = stringsToShow[0];
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        readyButton.onClick.AddListener(SetSecondTutorial);
        readyButton.gameObject.SetActive(false);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    void SetSecondTutorial()
    {
        if (difficulty == 0)
        {
            audioManager.PlayClip(audioInScene[7]);
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                instruccionText.text = stringsToShow[2] + stringsToShow[4];
                audioManager.PlayClip(audioInScene[2], audioInScene[4]);
            }
            else if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                instruccionText.text = stringsToShow[1] + stringsToShow[4];
                audioManager.PlayClip(audioInScene[2], audioInScene[4]);
            }
        }
        else
        {
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                instruccionText.text = stringsToShow[2] + stringsToShow[3];
                audioManager.PlayClip(audioInScene[2], audioInScene[3]);
            }
            else if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                instruccionText.text = stringsToShow[1] + stringsToShow[3];
                audioManager.PlayClip(audioInScene[1], audioInScene[3]);
            }
        }
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(WaitALittle);
        tutorialMode = true;
        needInput = false;
        readyButton.gameObject.SetActive(false);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    void WaitALittle()
    {
        instruccionPanel.SetActive(false);
        if (tutorialMode)
        {
            Invoke("PassToTheTutorial", 2f);
        }
        else
        {
            Invoke("PassToTheTest", 2f);
        }
    }

    //This function will set the fisrt frame of the test
    void SetTheTest()
    {
        instruccionPanel.SetActive(true);
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        if (difficulty == 0)
        {
            audioManager.PlayClip(veryGoodAudio, audioInScene[6]);
            instruccionText.text = TextReader.AddBeforeStrings(2, stringsToShow[6]);
        }
        else
        {
            audioManager.PlayClip(veryGoodAudio, audioInScene[5]);
            instruccionText.text = TextReader.AddBeforeStrings(2, stringsToShow[5]);
        }

        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(WaitALittle);
        readyButton.gameObject.SetActive(false);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        tutorialMode = false;
        needInput = false;
    }

    // Update is called once per frame
    void Update () {
        if (needInput) {
            timeToInput -= Time.deltaTime;
            arrowLatencie += Time.deltaTime;
            if (timeToInput <= 0) {
                CheckTheAnswer();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ExploteAnArrow();
                playerDirection = 1;
                CheckTheAnswer();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                ExploteAnArrow();
                playerDirection = -1;
                CheckTheAnswer();
            }
        }
	}

    void ExploteAnArrow()
    {
        arrowExplotion.Play();
        var m = arrowExplotion.main;
        if (tutorialMode)
        {
            m.startColor = farben[farbenOrdnungTutorial[arrowTutorialIndex]];
        }
        else
        {
            m.startColor = farben[farbenOrndung[arrowIndex]];
        }
    }

    //This will change a color of the arrow that is show
    void PutAColor(int colorNumber)
    {
        if (tutorialMode)
        {
            whiteArrow.GetComponent<SpriteRenderer>().color = farben[farbenOrdnungTutorial[colorNumber]];
        }
        else
        {
            whiteArrow.GetComponent<SpriteRenderer>().color = farben[farbenOrndung[colorNumber]];
        }

    }

    //This will change the direccion of the arrow
    void PutADireccion(int dir) {
        if (tutorialMode)
        {
            whiteArrow.transform.localScale = new Vector3(directionOrderTutorial[dir], 1f, 1f);
        }
        else {
            whiteArrow.transform.localScale = new Vector3(directionOrder[dir], 1f, 1f);
        }
    }

    //This will place an arrow if it is needed
    void PlaceAnArrow(int index) {
        whiteArrow.SetActive(true);
        PutAColor(index);
        PutADireccion(index);
        if (tutorialMode)
        {
            if (difficulty == 0)
            {
                if (farbenOrdnungTutorial[arrowTutorialIndex] == 0)
                {
                    timeToInput = 3.0f;
                }
                else
                {
                    timeToInput = 3.5f;
                }
            }
            else
            {
                timeToInput = 3.5f;
            }
        }
        else
        {
            timeToInput = 2.5f;
        }
        arrowLatencie = 0;
        needInput = true;
    }

    //this will check if the awnswer input for the player
    void CheckTheAnswer() {
        needInput = false;
        whiteArrow.SetActive(false);
        fullLatencies.Add(arrowLatencie);
        if (playerDirection != 0)
        {
            if (!tutorialMode)
            {
                if (farbenOrndung[arrowIndex] == 0)
                {
                    direction = (directionOrder[arrowIndex] * -1);
                    AnswerManager(TutorialErrors.BadGreen , 1);
                }
                else
                {
                    direction = (directionOrder[arrowIndex]);
                    AnswerManager(TutorialErrors.BadDireccion , 0);
                }

            }
            else
            {
                if (farbenOrdnungTutorial[arrowTutorialIndex] == 0)
                {
                    direction = (directionOrder[arrowTutorialIndex] * -1);
                    AnswerManager(TutorialErrors.BadGreen , 1);
                }
                else
                {
                    direction = (directionOrder[arrowTutorialIndex]);
                    AnswerManager(TutorialErrors.BadDireccion , 0);
                }

            }
        }
        else
        {
            if (!tutorialMode)
            {
                fullInteractions.Add(2);
                if (farbenOrndung[arrowIndex] == 0) {
                    if (difficulty == 0)
                    {
                        goodGreenAnswer++;
                    }
                    else
                    {
                        missGreenAnswer++;
                    }
                }
                else
                {
                    missAnswer++;
                }
            }
            else
            {
                if (difficulty == 0)
                {
                    if (farbenOrdnungTutorial[arrowTutorialIndex] == 0)
                    {
                        error = TutorialErrors.None;
                    }
                    else
                    {
                        error = TutorialErrors.Miss;
                    }

                }
                else
                {
                    error = TutorialErrors.Miss;
                }
            }

        }
        HandleAnswer();
    }

    void CallNewArrow()
    {
        arrowExplotion.Stop();
        if (tutorialMode)
        {
            PlaceAnArrow(arrowTutorialIndex);
        }
        else
        {
            PlaceAnArrow(arrowIndex);
        }

    }

    //this will handle the next step after knowing what the answer was
    void HandleAnswer() {
        if (!tutorialMode)
        {
            arrowIndex++;
            if (arrowIndex < directionOrder.Count)
            {
                Invoke("CallNewArrow", 0.4f);
                direction = 0;
                playerDirection = 0;
            }
            else {
                FinishTheSection();
            }
        }
        else {
            if (error == TutorialErrors.None)
            {
                arrowTutorialIndex++;
                if (arrowTutorialIndex < directionOrderTutorial.Count)
                {
                    Invoke("CallNewArrow", 0.4f);
                    direction = 0;
                    playerDirection = 0;
                }
                else
                {
                    SetTheTest();
                }
            }
            else
            {
                ManageErrors();
            }
        }
    }

    //This will see if the answer is right or Wrong
    void AnswerManager(TutorialErrors theErrorToShow, int isGreenAnswer) {
        if (playerDirection == direction)
        {
            if (!tutorialMode)
            {
                fullInteractions.Add(0);
                if (isGreenAnswer == 1)
                {
                    if (difficulty == 0)
                    {
                        badGreenAnswer++;
                        greenBadlatencies.Add(arrowLatencie);
                    }
                    else
                    {
                        goodGreenAnswer++;
                        greenArrowLatencies.Add(arrowLatencie);
                    }
                }
                else
                {
                    goodAnswer++;
                    arrowsLatencies.Add(arrowLatencie);
                }

            }
            else
            {
                if (difficulty == 0)
                {
                    if (isGreenAnswer == 1)
                    {
                        error = TutorialErrors.DontTouch;
                    }
                    else
                    {
                        error = TutorialErrors.None;
                    }
                }
                else
                {
                    error = TutorialErrors.None;
                }
            }
        }
        else
        {
            if (!tutorialMode)
            {
                fullInteractions.Add(1);
                if (isGreenAnswer == 1)
                {
                    badGreenAnswer++;
                    greenBadlatencies.Add(arrowLatencie);
                }
                else
                {
                    badAnswer++;
                    arrowBadLatencies.Add(arrowLatencie);
                }
            }
            else
            {
                if (difficulty == 0)
                {
                    if (isGreenAnswer == 1)
                    {
                        error = TutorialErrors.DontTouch;
                    }
                    else
                    {
                        error = TutorialErrors.BadDireccion;
                    }
                }
                else
                {
                    error = theErrorToShow;
                }
            }

        }
    }

    //This will put a left direction in a touch mode
    void PressLeft() {
        playerDirection = -1;
        CheckTheAnswer();
    }

    //This will put a right direction in a touch mode
    void PressRight() {
        playerDirection = 1;
        CheckTheAnswer();
    }

    //This is the between pass betwen playing the test and the set of the test
    void PassToTheTest() {
        PlaceAnArrow(arrowIndex);
    }

    //This is the between pass betwen playing the test and the set of the tutorial
    void PassToTheTutorial() {
        PlaceAnArrow(arrowTutorialIndex);
    }

    //This will move to the next part of the evaluation
    void FinishTheSection() {
        instruccionPanel.SetActive(true);
        audioManager.PlayClip(veryGoodAudio, audioInScene[11]);
        instruccionText.text = TextReader.AddBeforeStrings(2, stringsToShow[11]);
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(FinishTheGameOfArrows);
        readyButton.gameObject.SetActive(false);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    //this will handle what happen after an error is detected
    void ManageErrors() {
        readyButton.onClick.RemoveAllListeners();
        instruccionPanel.SetActive(true);
        readyButton.gameObject.SetActive(false);
        arrowTutorialIndex = 0;
        evaluationController.TutorialPlay();
        switch (error) {
            case TutorialErrors.Miss:
                audioManager.PlayClip(ohNoAudio, audioInScene[9]);
                instruccionText.text = TextReader.AddBeforeStrings(1, stringsToShow[9]);
                break;
            case TutorialErrors.BadGreen:
                audioManager.PlayClip(ohNoAudio, audioInScene[5]);
                instruccionText.text = TextReader.AddBeforeStrings(1, stringsToShow[5]);
                break;
            case TutorialErrors.BadDireccion:
                if (SystemInfo.deviceType == DeviceType.Desktop)
                {
                    audioManager.PlayClip(ohNoAudio, audioInScene[7]);
                    instruccionText.text = TextReader.AddBeforeStrings(1, stringsToShow[7]);
                }
                else if (SystemInfo.deviceType == DeviceType.Handheld)
                {
                    instruccionText.text = TextReader.AddBeforeStrings(1, stringsToShow[8]);
                    audioManager.PlayClip(ohNoAudio, audioInScene[8]);
                }
                break;
            case TutorialErrors.DontTouch:
                audioManager.PlayClip(ohNoAudio, audioInScene[6]);
                instruccionText.text = TextReader.AddBeforeStrings(1, stringsToShow[6]);
                break;
        }
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        readyButton.onClick.AddListener(WaitALittle);
    }

    //This will turn on the ready button on 
    void ReadyButtonOn()
    {
        readyButton.gameObject.SetActive(true);
    }

    void FinishTheGameOfArrows() {
        if (probes != null)
        {
            if (probes.IsSecondRow())
            {
                evaluationController.SaveArrows2(goodAnswer, goodGreenAnswer, badAnswer, badGreenAnswer, missAnswer, missGreenAnswer,
                arrowsLatencies, greenArrowLatencies, arrowBadLatencies, greenBadlatencies, fullInteractions, fullLatencies);
            }
            else
            {
                probes.NewArrowScene();
                evaluationController.SaveArrows1(goodAnswer, goodGreenAnswer, badAnswer, badGreenAnswer, missAnswer, missGreenAnswer,
                arrowsLatencies, greenArrowLatencies, arrowBadLatencies, greenBadlatencies, fullInteractions, fullLatencies);
            }
        }
        else
        {
            evaluationController.SaveTheArrowProgress(goodAnswer, goodGreenAnswer, badAnswer, badGreenAnswer, missAnswer, missGreenAnswer,
                arrowsLatencies, greenArrowLatencies, arrowBadLatencies, greenBadlatencies, fullInteractions, fullLatencies, difficulty);
        }

    }

}
