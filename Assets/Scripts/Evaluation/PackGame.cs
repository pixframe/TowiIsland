using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackGame : MonoBehaviour
{
    #region Variables

    #region Variables used for the game

    #region Scripts
    //this script is the one that controlls all the evaluation for more info go to that particullar script
    EvaluationController evaluationController;
    //this one is used in the control of all the audio in the evaluation for more info refer to the script
    AudioManager audioManager;
    //this one is used to controll the animator attached to the camera
    Animator cameraAnimator;
    //Audios used in this scene
    public AudioClip[] stimulus;
    public AudioClip zipper;
    AudioClip[] audioInScene;
    AudioClip upsAudio;
    AudioClip tryAudio;
    AudioClip tryAgainAudio;
    AudioClip veryGoodAudio;
    #endregion

    #region UI elements
    //Needed UI Elements
    [Header("UI Elements")]
    public Button readyButton;
    public Text instruccionText;
    public GameObject instruccionsPanel;
    #endregion

    #region GameObjects
    //this one is used to keep the object that is selected and is then used in the script
    GameObject selectedObject;
    //this one is the manager of all the objects that are show to the player
    public GameObject smallObjectList;
    //this one is the manager of all the objects that can be dragged
    public GameObject objectsToDragList;
    #endregion

    #region LayerMasks
    [Header("LayerMask for the game")]
    //this one is used to determine what are the selectable objects
    public LayerMask mask;
    //this one is used to determine ehat is a suitcase
    public LayerMask suitcaseMask;
    #endregion

    #region Booleans
    //this one is used to determine if its time to grab the objects
    bool selectableTime;

    bool objectSelected;
    //this one is used to say when is the first instruccion of tutorial is
    bool firstInstruccion;
    //this will determine whe is the time of the tutorial
    bool tutorialMode;
    //this will determine if its time of the test
    bool testMode;
    //this will say if the player skips or not the part
    bool skipThePart;
    //this will detrmine when its the weather part of the test
    bool weatherMode;
    //this will say when its time to remember the objects
    bool Unpackmode;
    //this will determine if a object is or not selected
    bool isObjectSelected;
    //this will run the weather latency
    bool isWeatherready;
    #endregion

    #region Numbers
    //this one is used to determine if the tests is going foward or backwards
    int direccionOfTheTest;
    //this one is used to know if its the first try or the second in a test
    int numberOfTry;
    //this int is used to know the current level of the test
    int levelOfTest;
    //this is used to have a number wich is used to switch objects
    int lastObjectIndex;
    //this is the index used to travel around the needed objects
    int needsIndex;
    //this is the index that use to switch the current object
    int currentObjectIndex;
    //this is index is used in the final loop to know exctly when to play it again and when not
    int rememberLoopIndex;
    //this index is used to know how many objects are save in the weather part this is particullary usefull because the selection of objects in that part should not exceed 3 objects
    int weatherObjectsSavedIndex;
    //this is used in the determination of the distance between a draggable object and the camera
    float lastKnownPosition;
    //get the latencie of weather
    float weatherLatency;
    #endregion


    #region Text
    [Header("Text Elements")]
    // Assset used for the text of the game
    public TextAsset textAssetForGameText;
    // Assset used for the text of the objects
    public TextAsset textAssetForObjects;
    //string array that saves the strings of the game
    string[] stringsToShow;
    //string array that saves the strings of the objects
    string[] stringsObjects;
    #endregion

    #region Lists
    //this are the objects that are pack by the player in the suitcase
    List<int> packingObjects = new List<int>();
    //this one is used to know what are the objects tath have to be packed by the player
    List<int> needObjects = new List<int>();

    List<List<int>> objectsGrabedInNormal = new List<List<int>>();
    List<List<int>> objectsGrabedInReverse = new List<List<int>>();
    //this list contains all the objects that are show to the player and can be playable by him 
    List<GameObject> smallObjectsToExpand = new List<GameObject>();
    List<GameObject> objectsToDrag = new List<GameObject>();
    #endregion

    #region Enum
    //this is a group of enum that help to know the state of error in the tutorial part
    private enum TutorialErrors { notInTheList, wrongOrder, None };
    //this is the variable that keeps track of the posible error in the tutorial
    TutorialErrors tutorialErrors;
    #endregion

    #endregion

    #region Variables used in the data record
    //this int is used to save the level reach in the foward test at saving moment it will have to be this value +3 to refer the amplitude
    int normalLevelAchive;
    //this int is used to save the level reach in the backward test at saving moment it will have to be this value +2 to refer the amplitude
    int backwardsLevelAchive;
    //this will int is used to have a record of the quantity of second trys in the foward test
    int numberOfErrorsFoward;
    //this will int is used to have a record of the quantity of second trys in the foward test
    int numberOfErrorBackwards;
    //this will save tha data of intruscion
    int numberOfIntrucions;
    //this will save the data of incorrect secuence
    int numberIncorrectSequences;
    //this list will keep a record of all the weather objects that are grabbed for the player
    List<string> weatherObjects = new List<string>();
    #endregion

    #endregion

    // Use this for initialization
    void Start()
    {
        //The initialization of the important values
        audioManager = FindObjectOfType<AudioManager>();
        evaluationController = FindObjectOfType<EvaluationController>();
        readyButton.onClick.AddListener(SetTheFirstInstruccions);
        stringsToShow = TextReader.TextsToShow(textAssetForGameText);
        stringsObjects = TextReader.TextsToShow(textAssetForObjects);
        audioInScene = Resources.LoadAll<AudioClip>("Audios/Evaluation/Scene_2");
        upsAudio = Resources.Load<AudioClip>("Audios/Evaluation/Frases/Frase_3");
        tryAudio = Resources.Load<AudioClip>("Audios/Evaluation/Frases/Frase_0");
        tryAgainAudio = Resources.Load<AudioClip>("Audios/Evaluation/Frases/Frase_1");
        veryGoodAudio = Resources.Load<AudioClip>("Audios/Evaluation/Frases/Frase_2");
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        evaluationController.StarCounting();
        SetTheIntro();
        testMode = false;
        firstInstruccion = true;
        FillTheObjectsList();
        levelOfTest = 0;
        numberOfTry = 0;
        cameraAnimator = FindObjectOfType<Camera>().gameObject.GetComponent<Animator>();
        cameraAnimator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectableTime)
        {
            if (!isObjectSelected)
            {
                SelectAnObject();
            }
            else
            {
                DragTheObject();
                DropAnObject();
            }

        }
        if (isWeatherready)
        {
            weatherLatency += Time.deltaTime;
        }
        
    }

    #region Orders to Follow the game

    //This is the first part of the instruccion here we check that evrything works perfectly
    void SetTheIntro()
    {
        audioManager.PlayClip(audioInScene[0]);
        instruccionText.text = stringsToShow[0];
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(() => SetTheFirstInstruccions());
        readyButton.gameObject.SetActive(false);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    //This is the second partr and the first instruccion for the kid
    void SetTheFirstInstruccions()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            audioManager.PlayClip(audioInScene[2]);
            instruccionText.text = stringsToShow[2];
        }
        else
        {
            audioManager.PlayClip(audioInScene[1]);
            instruccionText.text = stringsToShow[1];
        }
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(MakeSelectableObjects);
        readyButton.gameObject.SetActive(false);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    //This is a minin test to see if the kid actually learn
    void PlayFirstTutorial()
    {
        PutOrderInTheRoom();
        instruccionsPanel.SetActive(true);
        audioManager.PlayClip(audioInScene[3], tryAudio);
        instruccionText.text = TextReader.AddStrings(0, stringsToShow[3]);
        selectableTime = false;
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(PlayTheTutorial);
        readyButton.gameObject.SetActive(false);
        float timeToInvoke = audioManager.ClipDuration();
        Invoke("ReadyButtonOn", timeToInvoke);
        firstInstruccion = false;
        RemoveAllTheListData();
    }

    //this moves from the tutorial to the test
    void PassToTheTest()
    {
        instruccionsPanel.SetActive(true);
        instruccionText.text = TextReader.AddBeforeStrings(2, stringsToShow[5]);
        selectableTime = false;
        tutorialMode = false;
        audioManager.PlayClip(veryGoodAudio, audioInScene[5]);
        if (direccionOfTheTest == 0)
        {
            Invoke("GoingFowardTest", audioManager.ClipDuration());
        }
        else
        {
            Invoke("GoingBackwardTest", audioManager.ClipDuration());
        }

        testMode = true;
        PutOrderInTheRoom();
    }

    //This is used to set the game
    void GoingFowardTest() {
        RemoveAllTheListData();
        needsIndex = 0;
        instruccionsPanel.SetActive(true);
        readyButton.gameObject.SetActive(false);
        needObjects.RemoveRange(0, needObjects.Count);
        needObjects = PackInstruccions.GoingFowardLevels(levelOfTest, numberOfTry);
        PutOrderInTheRoom();
        PresentTheObjects();
    }

    //This is used for the second tutorial
    void PlaySecondTutorial() {
        direccionOfTheTest = 1;
        tutorialMode = true;
        testMode = false;
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        PutOrderInTheRoom();
        instruccionsPanel.SetActive(true);
        audioManager.PlayClip(audioInScene[6], tryAudio);
        instruccionText.text = TextReader.AddStrings(0, stringsToShow[6]);
        selectableTime = false;
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(PlayTheTutorial);
        RemoveAllTheListData();
        readyButton.gameObject.SetActive(false);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    //This is used to set the game backwards
    void GoingBackwardTest() {
        RemoveAllTheListData();
        needsIndex = 0;
        instruccionsPanel.SetActive(true);
        readyButton.gameObject.SetActive(false);
        needObjects.RemoveRange(0, needObjects.Count);
        needObjects = PackInstruccions.GoingBackLevels(levelOfTest, numberOfTry);
        PutOrderInTheRoom();
        PresentTheObjects();
    }

    //This section will say which weather will be there
    void ChatAboutWeather() {
        instruccionsPanel.SetActive(true);
        instruccionText.text = stringsToShow[10];
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        cameraAnimator.Play("CameraFrontRotation");
        audioManager.PlayClip(audioInScene[10]);
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(MakeSelectableObjects);
        RemoveAllTheListData();
        readyButton.gameObject.SetActive(false);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        weatherMode = true;
        testMode = false;
        tutorialMode = false;
        selectableTime = false;
    }

    //This is the final secction here we are going to play a loop that te kid should remember later
    void RememberLoop() {
        needsIndex = 0;
        if (rememberLoopIndex == 0) {
            needObjects.RemoveRange(0, needObjects.Count);
            Unpackmode = true;
            needObjects = PackInstruccions.rememberUnpack;
            audioManager.PlayClip(audioInScene[11]);
            readyButton.gameObject.SetActive(false);
            instruccionsPanel.SetActive(true);
            instruccionText.text = stringsToShow[11];
            Invoke("PresentTheObjects", audioManager.ClipDuration());
        } else {
            audioManager.PlayClip(audioInScene[12]);
            instruccionText.text = stringsToShow[12];
            Invoke("PresentTheObjects", audioManager.ClipDuration());
        }
    }

    //This is the last screen show to the player in wich there is a remainder and makes the moves to next part
    void FinishPart() {
        readyButton.gameObject.SetActive(true);
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(() => FinishPackGame());
        Invoke("FinishPackGame", 3f);
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        instruccionText.text = stringsToShow[11] + "\n" + stringsObjects[needObjects[0]] +
            "\n" + stringsObjects[needObjects[1]] + "\n" + stringsObjects[needObjects[2]];
    }

    #endregion

    #region Functions of SetUp

    //Here we fill the list of objects that are going to swipe to make an animation at selection time
    private void FillTheObjectsList()
    {
        for (int i = 0; i < smallObjectList.transform.childCount; i++)
        {
            smallObjectsToExpand.Add(smallObjectList.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < objectsToDragList.transform.childCount; i++)
        {
            objectsToDrag.Add(objectsToDragList.transform.GetChild(i).gameObject);
        }
    }

    //this is the part we let the objects to be selected
    void MakeSelectableObjects()
    {
        instruccionsPanel.SetActive(false);
        readyButton.gameObject.SetActive(false);
        selectableTime = true;
        if (testMode || weatherMode)
        {
            isWeatherready = true;
            readyButton.gameObject.SetActive(true);
            readyButton.onClick.RemoveAllListeners();
            evaluationController.SetButtonText(readyButton, TextReader.commonStrings[2]);
            readyButton.onClick.AddListener(() => SkipThePart());
            skipThePart = false;
        }
    }

    //This make the important of going up
    void LevelUp()
    {
        if (levelOfTest < 6)
        {
            levelOfTest++;
            numberOfTry = 0;
            if (direccionOfTheTest == 0)
            {
                AddNewData(0);
                GoingFowardTest();
            }
            else
            {
                AddNewData(1);
                GoingBackwardTest();
            }

        }
        else
        {
            NextSeccion();
        }
    }

    //This function is made to handle worng answer
    void ErrorManager() {
        numberOfTry++;
        if (numberOfTry < 2)
        {
            if (direccionOfTheTest == 0)
            {
                AddNewData(0);
                GoingFowardTest();
                numberOfErrorsFoward++;
            }
            else
            {
                AddNewData(1);
                GoingBackwardTest();
                numberOfErrorBackwards++;
            }
        }
        else
        {
            NextSeccion();
        }
    }

    //This helps to move to the next secction
    void NextSeccion() {
        cameraAnimator.enabled = true;

        if (direccionOfTheTest == 0)
        {
            normalLevelAchive = levelOfTest;
        }
        else
        {
            backwardsLevelAchive = levelOfTest;
        }

        levelOfTest = 0;
        numberOfTry = 0;
        numberIncorrectSequences = 0;
        numberOfIntrucions = 0;

        if (direccionOfTheTest == 0)
        {
            AddNewData(0);
            evaluationController.SavePackProgress(normalLevelAchive, numberOfErrorsFoward, objectsGrabedInNormal, numberOfIntrucions, numberIncorrectSequences);
            evaluationController.StarCounting();
            PlaySecondTutorial();
        }
        else
        {
            AddNewData(1);
            evaluationController.SavePackBackProgress(backwardsLevelAchive, numberOfErrorBackwards, objectsGrabedInReverse, numberOfIntrucions, numberIncorrectSequences);
            evaluationController.StarCounting();
            ChatAboutWeather();
        }
    }

    //This will help to play the first tutorial
    void PlayTheTutorial()
    {
        tutorialMode = true;
        needsIndex = 0;
        if (direccionOfTheTest == 0)
        {
            needObjects = PackInstruccions.tutorialNeeds;
        } else {
            needObjects = PackInstruccions.tutorialBackNeeds;
        }
        readyButton.gameObject.SetActive(false);
        PresentTheObjects();
    }

    //This is used when a bad selection is made in the tutorial
    void ShowWarning()
    {
        PutOrderInTheRoom();
        instruccionsPanel.SetActive(true);
        readyButton.gameObject.SetActive(false);
        readyButton.onClick.RemoveAllListeners();
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        RemoveAllTheListData();
        readyButton.onClick.AddListener(PlayTheTutorial);
        string firstBigString;
        if (direccionOfTheTest == 0)
        {
            if (tutorialErrors == TutorialErrors.notInTheList)
            {
                audioManager.PlayClip(audioInScene[4], tryAgainAudio);
                firstBigString = TextReader.AddStrings(1, stringsToShow[4]);
                instruccionText.text = firstBigString;
            }
            else if (tutorialErrors == TutorialErrors.wrongOrder)
            {
                audioManager.PlayClip(upsAudio, audioInScene[8], tryAgainAudio);
                firstBigString = TextReader.AddStrings(1, stringsToShow[8]);
                instruccionText.text = TextReader.AddBeforeStrings(0, firstBigString);
            }
        }
        else
        {
            if (tutorialErrors == TutorialErrors.notInTheList)
            {
                audioManager.PlayClip(upsAudio, audioInScene[7], tryAgainAudio);
                firstBigString = TextReader.AddStrings(1, stringsToShow[7]);
                instruccionText.text = TextReader.AddBeforeStrings(0, firstBigString);
            }
            else if (tutorialErrors == TutorialErrors.wrongOrder)
            {
                audioManager.PlayClip(upsAudio, audioInScene[9], tryAgainAudio);
                firstBigString = TextReader.AddStrings(1, stringsToShow[9]);
                instruccionText.text = TextReader.AddBeforeStrings(0, firstBigString);
            }
        }
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        evaluationController.TutorialPlay();
    }

    //this will play the audios and update the text needed in the test
    void PresentTheObjects()
    {
        audioManager.PlayClip(stimulus[needObjects[needsIndex]]);
        instruccionText.text = stringsObjects[needObjects[needsIndex]];
        selectableTime = false;
        if (needsIndex < needObjects.Count - 1)
        {
            Invoke("PresentTheObjects", 2f);
        }
        else
        {
            if (!Unpackmode)
            {
                Invoke("MakeSelectableObjects", 2f);
            }
            else
            {
                if (rememberLoopIndex < 2)
                {
                    rememberLoopIndex++;
                    Invoke("RememberLoop", 2f);
                }
                else
                {
                    Invoke("FinishPart", 2f);
                }
            }
        }
        needsIndex++;
    }

    //This removes the now useless data
    void RemoveAllTheListData()
    {
        packingObjects.Clear();
    }

    //this scripts helps to set the raw data
    void AddNewData(int dir)
    {
        List<int> dataToStore = new List<int>();
        for (int i = 0; i < packingObjects.Count; i++)
        {
            dataToStore.Add(packingObjects[i]);
        }
        if (dir == 0)
        {
            objectsGrabedInNormal.Add(dataToStore);
        }
        else if (dir == 1)
        {
            objectsGrabedInReverse.Add(dataToStore);
        }
    }

    //This reset the room for a new test
    void PutOrderInTheRoom()
    {
        for (int i = 0; i < smallObjectsToExpand.Count; i++)
        {
            smallObjectsToExpand[i].SetActive(true);
        }
        for (int i = 0; i < objectsToDrag.Count; i++)
        {
            objectsToDrag[i].SetActive(false);
        }
        objectSelected = false;
    }

    //This Function checks if the normal answers are right
    bool AreTheFowardAnswersRight()
    {
        bool isAllRight = true;
        for (int i = 0; i < packingObjects.Count; i++)
        {
            if (packingObjects[i] != needObjects[i])
            {
                bool isBadOrdered = false;
                for (int j = i + 1; j < needObjects.Count; j++)
                {
                    if (packingObjects[i] == needObjects[j])
                    {
                        isBadOrdered = true;
                    }
                }
                if (isBadOrdered)
                {
                    numberIncorrectSequences++;
                    Debug.Log(numberIncorrectSequences);
                }
                else
                {
                    numberOfIntrucions++;
                    Debug.Log(numberOfIntrucions);
                }
                isAllRight = false;
            }
        }
        if (skipThePart)
        {
            isAllRight = false;
        }
        if (!isAllRight)
        {
            return false;
        }
        return true;
    }

    //This function checkls if the backwards answers are right
    bool AreTheBackwardAnswersRight()
    {
        bool isAllRight = true;
        for (int i = 0; i < packingObjects.Count; i++)
        {
            if (packingObjects[i] != needObjects[needObjects.Count - i - 1])
            {
                bool isBadOrdered = false;
                for (int j = 0; j < needObjects.Count; j++)
                {
                    if (packingObjects[i] == needObjects[j] && j != needObjects.Count - i - 1)
                    {
                        isBadOrdered = true;
                    }
                }
                if (isBadOrdered)
                {
                    numberIncorrectSequences++;
                }
                else
                {
                    numberOfIntrucions++;
                }
                isAllRight = false;
            }
        }
        if (skipThePart)
        {
            isAllRight = false;
        }
        if (!isAllRight)
        {
            return false;
        }
        return true;
    }

    //This function will call the next scene
    void FinishPackGame()
    {
        evaluationController.SaveTheWeatherProgress(weatherObjects, weatherLatency, WeatherScore());
    }

    //This will activate the sound when its needed
    void ReadyButtonOn()
    {
        readyButton.gameObject.SetActive(true);
    }

    int WeatherScore()
    {
        if (weatherObjects.Count == 0)
        {
            return 4;
        }
        else if (weatherObjects.Count == 1)
        {
            if (weatherObjects[0] == "sombrillacerrada")
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            for (int i = 0; i < weatherObjects.Count; i++)
            {
                if (weatherObjects[i] == "sombrillacerrada")
                {
                    return 3;
                }
            }
            return 1;
        }
    }

    #endregion

    #region Object Selection

    //Function made to select an object from the pack test
    void SelectAnObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                selectedObject = hit.collider.gameObject;
            }
            if (!objectSelected)
            {
                if (selectedObject != null && selectedObject.name != "SuitCase")
                {
                    currentObjectIndex = smallObjectsToExpand.IndexOf(selectedObject);
                    lastObjectIndex = currentObjectIndex;
                    SwitchObjects(currentObjectIndex);
                    lastKnownPosition = Vector3.Distance(selectedObject.transform.position, Camera.main.transform.position);
                    objectSelected = true;
                    isObjectSelected = true;
                    if (isWeatherready)
                    {
                        isWeatherready = false;
                    }
                }
            }
            else
            {
                if (selectedObject.name == "SuitCase")
                {
                    PackInTheSuitCase();
                }
                else
                {
                    if (!firstInstruccion)
                    {
                        BackToNormalObjects(lastObjectIndex);
                        currentObjectIndex = smallObjectsToExpand.IndexOf(selectedObject);
                        SwitchObjects(currentObjectIndex);
                        lastObjectIndex = currentObjectIndex;
                    }
                    else
                    {

                    }
                }
            }
        }
    }

    //This function will drop an object
    void DropAnObject() {
        if (Input.GetMouseButtonUp(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, suitcaseMask))
            {
                PackInTheSuitCase();
            }
            else
            {
                BackToNormalObjects(lastObjectIndex);
                currentObjectIndex = smallObjectsToExpand.IndexOf(selectedObject);
                lastObjectIndex = currentObjectIndex;
                isObjectSelected = false;
                objectSelected = false;
            }
            isObjectSelected = false;
        }
    }

    //This Funtion is used to change the small object for a drag object
    void SwitchObjects(int changer)
    {
        smallObjectsToExpand[changer].SetActive(false);
        objectsToDrag[changer].SetActive(true);
        selectedObject = objectsToDrag[changer];
    }

    //This function will swap the darg object to became a small one again
    void BackToNormalObjects(int changer)
    {
        smallObjectsToExpand[changer].SetActive(true);
        objectsToDrag[changer].SetActive(false);
    }

    //This function will control what happen after you put an objevt into the suitcase
    void PackInTheSuitCase()
    {
        audioManager.PlayClip(zipper);
        if (weatherMode)
        {
            weatherObjects.Add(objectsToDrag[lastObjectIndex].name);
            weatherObjectsSavedIndex++;
        }
        packingObjects.Add(lastObjectIndex);
        objectsToDrag[lastObjectIndex].SetActive(false);
        objectSelected = false;
        if (firstInstruccion)
        {
            firstInstruccion = false;
            PlayFirstTutorial();
        }

        if (tutorialMode)
        {
            tutorialErrors = IsThereATutorialError();
            if (tutorialErrors == TutorialErrors.None)
            {
                if (packingObjects.Count == needObjects.Count)
                {
                    PassToTheTest();
                }
            }
            else
            {
                ShowWarning();
            }
        }
        else if (testMode)
        {
            if (packingObjects.Count == needObjects.Count)
            {
                CheckTheAnswers();
            }
        }
        if (weatherObjectsSavedIndex >= 3)
        {
            RememberLoop();
        }
    }

    //this method is used to verify the answer after picking objects
    private void CheckTheAnswers()
    {
        if (weatherMode)
        {
            RememberLoop();
        }
        else if (skipThePart)
        {
            if (!weatherMode)
            {
                ErrorManager();
            }
        }
        else
        {
            if (direccionOfTheTest == 0)
            {
                if (AreTheFowardAnswersRight())
                {
                    LevelUp();
                }
                else
                {
                    ErrorManager();
                }
            }
            else
            {
                if (AreTheBackwardAnswersRight())
                {
                    LevelUp();
                }
                else
                {
                    ErrorManager();
                }
            }
        }
    }

    //This will pass the test without answers
    void SkipThePart() {
        skipThePart = true;
        CheckTheAnswers();
    }

    //this bool is only used to know in the tutorial if the objects are correctly input
    private TutorialErrors IsThereATutorialError()
    {
        if (direccionOfTheTest == 0)
        {
            for (int i = 0; i < packingObjects.Count; i++)
            {
                if (packingObjects[i] != needObjects[i])
                {
                    for (int j = 0; j < needObjects.Count; j++)
                    {
                        if (packingObjects[i] == needObjects[j])
                        {
                            return TutorialErrors.wrongOrder;
                        }
                    }
                    return TutorialErrors.notInTheList;
                }
            }
        }
        else
        {
            for (int i = 0; i < packingObjects.Count; i++)
            {
                if (packingObjects[i] != needObjects[needObjects.Count - i - 1])
                {
                    for (int j = 0; j < needObjects.Count; j++)
                    {
                        if (packingObjects[i] == needObjects[j])
                        {
                            return TutorialErrors.wrongOrder;
                        }
                    }
                    return TutorialErrors.notInTheList;
                }
            }
        }

        return TutorialErrors.None;
    }

    //this function will drag the object arround the screen until the player releases it
    void DragTheObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = ray.GetPoint(lastKnownPosition);
        selectedObject.transform.position = pos;
    }

    #endregion

}