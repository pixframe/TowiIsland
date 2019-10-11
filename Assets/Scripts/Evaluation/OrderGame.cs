using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderGame : MonoBehaviour {

    #region Varaibles

    #region Variables used for the game

    #region Scripts

    //this script is the one that controlls all the evaluation for more info go to that particullar script
    EvaluationController evaluationController;
    //this one is used in the control of all the audio in the evaluation for more info refer to the script
    AudioManager audioManager;

    AudioClip[] audiosInScene;
    #endregion

    #region LayerMask

    public LayerMask mask;
    public LayerMask saver;
    public LayerMask positioner;

    #endregion

    #region UI

    public GameObject instrucctionPanel;
    public Button readyButton;
    public Button upButton;
    public TextMeshProUGUI textInstruction;
    public GameObject storyCanvas;
    public TextMeshProUGUI storyText0;
    public TextMeshProUGUI storyText1;

    #endregion

    #region Gameobjects
    //this game object uis used to kkep a selected object
    GameObject selectedObject;

    public GameObject unpackManager;
    public GameObject objectHandler;
    public GameObject holderHandler;
    public GameObject panelHolder;
    public GameObject selectionPanel;
    public GameObject smallContainer;
    public GameObject bigContainer;
    public GameObject positionsHandler;
    public GameObject ornaments;
    public GameObject packObjects;
    public Camera uiCamera;
    public Camera storyCam;
    public Camera mainCamera;

    ObjectToOrder[] objectsToOrder;
    List<GameObject> dropedObjects = new List<GameObject>();

    #endregion

    #region Booleans

    bool orderingTime;
    bool rememberPlaceTime;
    bool theFisrtObject;
    bool somethingSelected;

    #endregion

    #region Numbers

    int bigIndex;
    int showerIndex;
    int numberOfObjectsToShow;
    int numberOfTry;
    int numberOfComparassion;
    int comparassionIndex = 0;
    int badRecognictionIndex = 0;
    float timeToShow = 0.4f;
    float assayLatencie = 0f;

    int[] usedSpaces = new int[3];

    #endregion

    #region Text

    string firstItem;
    string firstCorrectItem;

    List<string> selectedObjectsList = new List<string>();
    List<string> occupiedPlaces = new List<string>();
    List<string> goodRemberObjets = new List<string>();
    List<string> itemsToRemember = new List<string>();
    List<string> firstObjects = new List<string>();
    List<string> lastObjects = new List<string>();
    List<string> objectsToRemember = new List<string> {
        "Tabla de surf", "Carrito", "Robot"
    };
    List<string> reminderList = new List<string>();

    #endregion

    #region Vector3

    Vector3 lastKnownPosition;
    Vector3 mousePos;

    #endregion

    #region Text
    [Header("Text Element")]
    // Assset used for the text
    public TextAsset textAsset;
    //string array that saves the strings of the game
    string[] stringsToShow;
    #endregion

    #endregion

    #region Variables used for data recolection

    #region Text

    string[] unpackObjects = new string[3];

    List<string> goodButTakeItAwayObject = new List<string>();

    #endregion

    #region Numbers
    int rememberScore;
    int perseverations;
    int sizeOfUnpack;

    int[] correct = new int[3];
    int[] incorrect = new int[3];
    int[] reapeted = new int[3];
    int[] scoreFirstObjects = new int[3];
    int[] scoreLastObjects = new int[3];
    int[] positionScores = new int[3];
    int[] positionSamples = new int[3];
    int[] percentageFirstObjects = new int[3];
    int[] percentageLastObjects = new int[3];

    #endregion

    #region Lists

    List<string> takenObjects = new List<string>();
    List<List<string>> goodCompareLists = new List<List<string>>();
    List<List<string>> badCompareLists = new List<List<string>>();
    List<List<string>> orderObjectsList = new List<List<string>>();
    List<float> latenciesOfAssays = new List<float>();

    #endregion

    #endregion

    #endregion

    // Use this for initialization
    void Start() {
        evaluationController = FindObjectOfType<EvaluationController>();
        audioManager = FindObjectOfType<AudioManager>();
        textAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Evaluation/Evaluation_07/Evaluation_Scene7");
        audiosInScene = Resources.LoadAll<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Evaluation/Scene_7");
        stringsToShow = TextReader.TextsToShow(textAsset);
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(audiosInScene[0], audiosInScene[1]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        upButton.gameObject.SetActive(false);
        //ShowThePanel();
        //DesactivateAllTheOrderedObjects();
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        objectsToOrder = new ObjectToOrder[objectHandler.transform.childCount];
        evaluationController.StarCounting();
        theFisrtObject = true;
        storyText0.text = stringsToShow[0];
        storyText1.text = stringsToShow[1];
        instrucctionPanel.gameObject.SetActive(false);
        for (int i = 0; i < objectHandler.transform.childCount; i++)
        {
            objectsToOrder[i] = objectHandler.transform.GetChild(i).GetComponent<ObjectToOrder>();
        }
        readyButton.onClick.AddListener(ActivateTheStuff);
        int age = evaluationController.GetTheAgeOfPlayer();

        if (age < 7)
        {
            numberOfObjectsToShow = 6;
        }
        else if (age >= 7 && age < 10)
        {
            numberOfObjectsToShow = 9;
        }
        else
        {
            numberOfObjectsToShow = 12;
        }
        numberOfComparassion = numberOfObjectsToShow / 3;

    }

    // Update is called once per frame
    void Update() {
        if (orderingTime) {
            assayLatencie += Time.deltaTime;
            if (somethingSelected)
            {
                DragTheObject();
                DropAnObject();
            }
            else
            {
                SelectObject();
            }
        }
    }

    #region Functions

    #region SetUps

    //This moves from the remember part to the order rhe roopm part
    void NextSection()
    {
        FirstRightItem();
        ScoreTheUnpackobjects();
        rememberPlaceTime = true;
        ReturnEveryThingToNormal();
        evaluationController.SaveUnpackProgress(firstItem, firstCorrectItem, unpackObjects, badRecognictionIndex, rememberScore, takenObjects, perseverations, sizeOfUnpack);
        evaluationController.StarCounting();
        ShowThePanel();
        readyButton.onClick.AddListener(PlayTheOrderVideo);
        readyButton.gameObject.SetActive(false);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    //this will set everything in position
    void ReturnEveryThingToNormal()
    {
        for (int i = 0; i < objectsToOrder.Length; i++)
        {
            objectsToOrder[i].ResetToNormalPosition();
        }
        for (int i = 0; i < positionsHandler.transform.childCount; i++)
        {
            positionsHandler.transform.GetChild(i).GetComponent<Positioner>().EmptyTheContainer();
        }
        perseverations = 0;
    }

    void ActivateTheStuff()
    {
        ornaments.SetActive(true);
        unpackManager.SetActive(true);
        packObjects.SetActive(true);
        storyCanvas.SetActive(false);
        ShowThePanel();
        storyCam.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        ActivateTheNeededObjects();
        ActivateTheNeededColliders();
        readyButton.onClick.AddListener(RememberTheStuff);
        for (int i = 0; i < numberOfComparassion; i++)
        {
            firstObjects.Add(smallContainer.transform.GetChild(i).name);
        }
        for (int i = numberOfObjectsToShow - 1; i > numberOfObjectsToShow + (-(numberOfComparassion) - 1); i--)
        {
            lastObjects.Add(smallContainer.transform.GetChild(i).name);
        }
        for (int i = 0; i < numberOfObjectsToShow; i++)
        {
            itemsToRemember.Add(smallContainer.transform.GetChild(i).name);
        }
        DesactivateAllTheOrderedObjects();
    }

    //this is used to show the panel of selecting objects
    void ShowThePanel()
    {
        if (!rememberPlaceTime)
        {
            audioManager.PlayClip(audiosInScene[2]);
            textInstruction.text = stringsToShow[2];
        }
        else
        {
            if (numberOfTry < 1)
            {
                audioManager.PlayClip(audiosInScene[3], audiosInScene[4]);
                textInstruction.text = stringsToShow[3] + stringsToShow[4];
            }
        }
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        evaluationController.SetButtonText(upButton, TextReader.commonStrings[2]);
        instrucctionPanel.SetActive(true);
        readyButton.gameObject.SetActive(true);
        upButton.gameObject.SetActive(false);
        unpackManager.SetActive(false);
        readyButton.onClick.RemoveAllListeners();
    }

    //This is the part when you show the game
    void ShowTheGame()
    {
        Cursor.visible = true;
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        evaluationController.SetButtonText(upButton, TextReader.commonStrings[2]);
        instrucctionPanel.SetActive(false);
        readyButton.gameObject.SetActive(false);
        upButton.gameObject.SetActive(true);
        unpackManager.SetActive(true);
        upButton.onClick.RemoveAllListeners();
        if (rememberPlaceTime)
        {
            assayLatencie = 0;
            holderHandler.SetActive(false);
            panelHolder.SetActive(false);
            DesactivateAllTheOrderedObjects();
            ActivateTheNeededColliders();
            upButton.onClick.AddListener(() => HandleFinishActivity());
        }
    }

    //This part you shoow how it looks when is empty
    void ShowTheVideo()
    {
        ActivateTheNeededObjects();
        DeactivateTRheCollidersAndResets();
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        evaluationController.SetButtonText(upButton, TextReader.commonStrings[2]);
        instrucctionPanel.SetActive(false);
        readyButton.gameObject.SetActive(false);
        upButton.gameObject.SetActive(false);
        unpackManager.SetActive(false);
        Cursor.visible = false;
        upButton.onClick.RemoveAllListeners();
    }

    //this will show the panel where you can choose an object
    void ShowTheSelectionPanel()
    {
        upButton.gameObject.SetActive(true);
        unpackManager.SetActive(true);
        selectionPanel.SetActive(true);
        for (int i = 0; i < objectHandler.transform.childCount; i++)
        {
            objectHandler.transform.GetChild(i).gameObject.SetActive(true);
            for (int j = 0; j < dropedObjects.Count; j++)
            {
                if (objectHandler.transform.GetChild(i).gameObject == dropedObjects[j])
                {
                    objectHandler.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    //this will make the response when you not put in the right place a game object
    void NotPlacedInAcorrectSpot()
    {
        selectedObject.GetComponent<ObjectToOrder>().ResetToNormalPosition();
        selectedObject = null;
    }

    //this will show how the objects are stored
    void PlayTheOrderVideo()
    {
        ShowTheVideo();
        GoingBig();
    }

    //this will make the objects big for .5 second to call the atention of the player
    void GoingBig()
    {
        if (bigIndex < 2)
        {
            bigContainer.transform.GetChild(showerIndex).gameObject.SetActive(true);
            smallContainer.transform.GetChild(showerIndex).gameObject.SetActive(false);
            Invoke("GoingSmall", timeToShow);
        }
        else
        {
            showerIndex++;
            if (showerIndex < numberOfObjectsToShow)
            {
                bigIndex = 0;
                GoingBig();
                return;
            }
            else
            {
                Invoke("ShowTheGame", timeToShow);
                ShowTheGame();
                showerIndex = 0;
                bigIndex = 0;
                return;
            }
        }
        bigIndex++;
    }

    //this will return the game object into its normal sccale
    void GoingSmall()
    {
        bigContainer.transform.GetChild(showerIndex).gameObject.SetActive(false);
        smallContainer.transform.GetChild(showerIndex).gameObject.SetActive(true);
        Invoke("GoingBig", timeToShow);
    }

    //this will disable every gameobject that its not needed in the game
    void DesactivateAllTheOrderedObjects()
    {
        for (int i = 0; i < smallContainer.transform.childCount; i++)
        {
            smallContainer.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    //this will activate every gameobject that is need in the game
    void ActivateTheNeededObjects()
    {
        for (int i = 0; i < numberOfObjectsToShow; i++)
        {
            smallContainer.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    //this activate all the colliders that are need in the gamne part
    void ActivateTheNeededColliders()
    {
        for (int i = 0; i < numberOfObjectsToShow; i++)
        {
            positionsHandler.transform.GetChild(i).gameObject.SetActive(true);
            positionsHandler.transform.GetChild(i).GetComponent<Positioner>().SetAvailable();
        }
    }

    //this desactivate the colliders and reset the colliders
    void DeactivateTRheCollidersAndResets()
    {
        for (int i = 0; i < positionsHandler.transform.childCount; i++)
        {
            positionsHandler.transform.GetChild(i).GetComponent<Positioner>().RestoreTheCollider();
            positionsHandler.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    //this will handle the game after its finish
    void HandleFinishActivity()
    {
        FillDataToStorage();
        AddValues(orderObjectsList[numberOfTry]);
        latenciesOfAssays.Add(assayLatencie);
        numberOfTry++;
        if (numberOfTry < 3)
        {
            dropedObjects.RemoveRange(0, dropedObjects.Count);
            ReturnEveryThingToNormal();
            ShowThePanel();
            audioManager.PlayClip(audiosInScene[numberOfTry + 4]);
            selectedObjectsList.Clear();
            textInstruction.text = stringsToShow[numberOfTry + 4];
            readyButton.onClick.AddListener(() => PlayTheOrderVideo());
            readyButton.gameObject.SetActive(false);
            Invoke("ReadyButtonOn", audioManager.ClipDuration());
            comparassionIndex = 0;

        }
        else
        {
            CompareList(orderObjectsList[0], orderObjectsList[1]);
            CompareList(orderObjectsList[1], orderObjectsList[2]);
            SaveTheData();
        }
    }

    //Save the data of the game 
    void SaveTheData() {
        evaluationController.SaveOrderProgress(numberOfObjectsToShow, correct, incorrect, reapeted,
            percentageFirstObjects, percentageLastObjects, positionScores, positionSamples, latenciesOfAssays,
            orderObjectsList, goodCompareLists, badCompareLists);
    }

    //This will pass to the stage into a Remember Part
    void RememberTheStuff()
    {
        ShowTheGame();
        upButton.onClick.AddListener(() => NextSection());
        orderingTime = true;
    }

    //This will activate the sound when its needed
    void ReadyButtonOn()
    {
        readyButton.gameObject.SetActive(true);
    }

    #endregion

    #region Interaction 

    //This one select a specific object to move
    void SelectObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                selectedObject = hit.collider.gameObject;
                if (selectedObject != null)
                {
                    somethingSelected = true;
                    lastKnownPosition = uiCamera.WorldToScreenPoint(selectedObject.transform.position);
                    if (selectedObject.GetComponent<ObjectToOrder>().IsSavedInAHolder())
                    {
                        unpackObjects[selectedObject.GetComponent<ObjectToOrder>().IsHoldedInNumber()] = null;
                        usedSpaces[selectedObject.GetComponent<ObjectToOrder>().IsHoldedInNumber()] = 0;
                    }
                    if (theFisrtObject)
                    {
                        theFisrtObject = false;
                        firstItem = selectedObject.name;
                    }
                }
                if (rememberPlaceTime)
                {
                    selectionPanel.SetActive(false);
                    for (int i = 0; i < objectsToOrder.Length; i++)
                    {
                        objectsToOrder[i].gameObject.SetActive(false);
                        if (objectsToOrder[i].gameObject == selectedObject)
                        {
                            selectedObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    takenObjects.Add(selectedObject.name);
                }
            }
        }

    }

    //This will move the object arround the screen
    void DragTheObject()
    {
        Vector3 pos = uiCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lastKnownPosition.z));
        selectedObject.transform.position = new Vector3(selectedObject.transform.position.x, pos.y, pos.z);
    }

    //this will handle when an object is drop
    void DropAnObject()
    {
        if (Input.GetMouseButtonUp(0))
        {
            somethingSelected = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            mousePos = Input.mousePosition;
            RaycastHit hit;
            if (rememberPlaceTime)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, positioner))
                {
                    GameObject tupper = hit.transform.gameObject;
                    Positioner positionManager = tupper.GetComponent<Positioner>();
                    if (tupper != null)
                    {
                        if (!positionManager.IsOcuppied())
                        {
                            if (positionManager.CanFillTheCollider(selectedObject.name))
                            {
                                int positionIndexer = tupper.transform.GetSiblingIndex();
                                smallContainer.transform.GetChild(positionIndexer).gameObject.SetActive(true);
                                correct[numberOfTry]++;
                                positionScores[numberOfTry] += 4;
                                positionSamples[numberOfTry] += 1;
                                selectedObjectsList.Add(selectedObject.name);
                                selectedObject.GetComponent<ObjectToOrder>().SaveInPlace();
                                dropedObjects.Add(selectedObject);
                                selectedObject.SetActive(false);
                            }
                            else
                            {
                                OrderForThePlayer(mousePos);
                            }

                        }
                        else
                        {
                            OrderForThePlayer(mousePos);
                        }
                    }
                    else
                    {
                        OrderForThePlayer(mousePos);
                    }
                    dropedObjects.Add(selectedObject);
                    selectedObject = null;
                    ShowTheSelectionPanel();
                }
                else
                {
                    OrderForThePlayer(mousePos);
                    selectedObject = null;
                    ShowTheSelectionPanel();
                }
            }
            else
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, saver))
                {
                    GameObject tupper = hit.transform.gameObject;

                    if (tupper != null)
                    {
                        SetInTheHolder(tupper);
                        selectedObject = null;
                    }

                }
                else
                {
                    NotPlacedInAcorrectSpot();
                }
            }


        }
    }

    //This will put the object in the selected Holder
    void SetInTheHolder(GameObject tupper)
    {
        int spaceToHold = int.Parse(tupper.name.Remove(0, 4));
        if (usedSpaces[spaceToHold] == 0)
        {
            selectedObject.GetComponent<ObjectToOrder>().SaveInAHolder(spaceToHold);
            selectedObject.transform.position = tupper.transform.parent.transform.position;
            unpackObjects[spaceToHold] = selectedObject.name;
            usedSpaces[spaceToHold] = 1;
            if (selectedObject.name == "Robot" || selectedObject.name == "Carrito" || selectedObject.name == "Tabla de surf")
            {
                for (int i = 0; i < goodRemberObjets.Count; i++)
                {
                    if (selectedObject.name == goodRemberObjets[i])
                    {
                        return;
                    }
                }
                goodRemberObjets.Add(selectedObject.name);
            }

        }
        else
        {
            NotPlacedInAcorrectSpot();
        }


    }

    //This will handle if the gameObject is placed in the right place
    void SetInRightPlace(GameObject tupper, string usedSpace)
    {
        if (selectedObject.name == usedSpace)
        {
            int makeItVisibleIndex = tupper.transform.GetSiblingIndex();
            smallContainer.transform.GetChild(makeItVisibleIndex).gameObject.SetActive(true);
            correct[numberOfTry]++;
            dropedObjects.Add(selectedObject);
            occupiedPlaces.Add(tupper.name);
            selectedObject.SetActive(false);
        }
        else
        {
            for (int i = 0; i < smallContainer.transform.childCount; i++)
            {
                if (selectedObject.name == smallContainer.transform.GetChild(i).gameObject.name)
                {
                    smallContainer.transform.GetChild(i).gameObject.SetActive(true);
                    correct[numberOfTry]++;
                    dropedObjects.Add(selectedObject);
                    occupiedPlaces.Add(selectedObject.name);
                    selectedObject.SetActive(false);
                }
            }
        }

    }

    //this will order the object for the player if its not placed in a correct place
    void OrderForThePlayer(Vector3 mPos)
    {
        for (int i = 0; i < positionsHandler.transform.childCount; i++)
        {
            Positioner positionerInList = positionsHandler.transform.GetChild(i).GetComponent<Positioner>();
            if (positionerInList.CanFillTheCollider(selectedObject.name))
            {
                int positionIndexer = positionerInList.transform.GetSiblingIndex();
                smallContainer.transform.GetChild(positionIndexer).gameObject.SetActive(true);
                int positionScore = positionerInList.ScoreDistance(mPos);
                positionScores[numberOfTry] += positionScore;
                correct[numberOfTry]++;
                selectedObjectsList.Add(selectedObject.name);
                selectedObject.GetComponent<ObjectToOrder>().SaveInPlace();
                dropedObjects.Add(selectedObject);
                selectedObject.SetActive(false);

                break;
            }
            else if (positionerInList.IsRepeated(selectedObject.name))
            {
                reapeted[numberOfTry]++;
                selectedObjectsList.Add(selectedObject.name);
                selectedObject.GetComponent<ObjectToOrder>().SaveInPlace();
                dropedObjects.Add(selectedObject);
                DropTheObjectToTheGround();
            }
        }
        if (!selectedObject.GetComponent<ObjectToOrder>().IsSaveInPlace())
        {
            incorrect[numberOfTry]++;
            selectedObjectsList.Add(selectedObject.name);
            DropTheObjectToTheGround();
        }
    }

    //this will trow the object if its not in the correct place
    void DropTheObjectToTheGround()
    {
        Debug.Log("throw the object");
        selectedObject.GetComponent<ObjectToOrder>().SaveInPlace();
        selectedObject.GetComponent<ObjectToOrder>().FlyAhed(); 
    }

    #endregion

    #region Scoring

    //this is the score of the unpacked objects
    void ScoreTheUnpackobjects() {
        for (int i = 0; i < unpackObjects.Length; i++) {
            if (unpackObjects[i] != null)
            {
                sizeOfUnpack++;
            }
            for (int j = 0; j < objectsToRemember.Count; j++) {
                if (unpackObjects[i] == objectsToRemember[j]) {
                    rememberScore += 1;
                }
            }
            for (int j = (i + 1); j < unpackObjects.Length; j++) {
                if (unpackObjects[i] == unpackObjects[j]) {
                    if (unpackObjects[i] != null) {
                        rememberScore -= 1;
                    }
                }
            }
        }
        if (unpackObjects[0] == unpackObjects[1])
        {
            bool isInList = false;
            for (int i = 0; i < objectsToRemember.Count; i++)
            {
                if (unpackObjects[0] == objectsToRemember[i])
                {
                    isInList = true;
                }
            }
            if (isInList)
            {
                perseverations++;
            }
        }
        if (unpackObjects[1] == unpackObjects[2])
        {
            bool isInList = false;
            for (int i = 0; i < objectsToRemember.Count; i++)
            {
                if (unpackObjects[0] == objectsToRemember[i])
                {
                    isInList = true;
                }
            }
            if (isInList)
            {
                perseverations++;
            }
        }
        if (unpackObjects[0] == unpackObjects[2] && unpackObjects[0] != unpackObjects[1])
        {
            bool isInList = false;
            for (int i = 0; i < objectsToRemember.Count; i++)
            {
                if (unpackObjects[0] == objectsToRemember[i])
                {
                    isInList = true;
                }
            }
            if (isInList)
            {
                perseverations++;
            }
        }
    }

    //this will chechk what was the first correct item the player select and leaves in the holders
    void FirstRightItem()
    {
        for (int i = 0; i < goodRemberObjets.Count; i++)
        {
            for (int j = 0; j < unpackObjects.Length; j++)
            {
                if (goodRemberObjets[i] == unpackObjects[j])
                {
                    firstCorrectItem = goodRemberObjets[i];
                    return;
                }
            }
            goodButTakeItAwayObject.Add(goodRemberObjets[i]);
            badRecognictionIndex++;
        }
        firstCorrectItem = "No Good Answer";
    }

    //this one will compare the list to make an arrange and make it work
    void CompareList(List<string> listA, List<string> listB)
    {
        reminderList.Clear();
        List<string> goodies = new List<string>();
        List<string> badies = new List<string>();
        bool readyToPut = false;
        for (int i = 0; i < listA.Count; i++)
        {
            for (int j = 0; j < listB.Count; j++)
            {
                if (listA[i] == listB[j])
                {
                    reminderList.Add(listB[j]);
                    j = listB.Count;
                }
            }
        }
        for (int i = 0; i < reminderList.Count; i++)
        {
            for (int j = 0; j < itemsToRemember.Count; j++)
            {
                if (reminderList[i] == itemsToRemember[j])
                {
                    readyToPut = true;
                }
            }
            if (readyToPut)
            {
                goodies.Add(reminderList[i]);
                readyToPut = false;
            }
            else
            {
                badies.Add(reminderList[i]);
            }
        }
        goodCompareLists.Add(goodies);
        badCompareLists.Add(badies);
    }

    //This function will add the points of 25 to the first or last selected objects
    void AddValues(List<string> strings)
    {
        for (int i = 0; i < strings.Count; i++)
        {
            if (i >= numberOfComparassion) {
                break;
            }
            for (int j = 0; j < numberOfComparassion; j++)
            {
                if (strings[i] == firstObjects[j])
                {
                    scoreFirstObjects[numberOfTry]++;
                    break;
                }
                if (strings[i] == lastObjects[j])
                {
                    scoreLastObjects[numberOfTry]++;
                    break;
                }
            }
        }
        percentageFirstObjects[numberOfTry] = (scoreFirstObjects[numberOfTry] * 100) / numberOfComparassion;
        percentageLastObjects[numberOfTry] = (scoreLastObjects[numberOfTry] * 100) / numberOfComparassion;
    }


    void FillDataToStorage()
    {
        List<string> dataToStorage = new List<string>();

        for (int i = 0; i < selectedObjectsList.Count; i++)
        {
            dataToStorage.Add(selectedObjectsList[i]);
        }

        orderObjectsList.Add(dataToStorage);
    }
    #endregion

    #endregion
}
