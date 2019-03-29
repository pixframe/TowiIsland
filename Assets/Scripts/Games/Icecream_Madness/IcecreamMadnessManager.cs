using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IcecreamMadnessManager : MonoBehaviour {

    public GameObject TableCenterManager;
    public GameObject Canvas;
    public GameObject uiCanvas;

    IcecreamChef chef;

    readonly List<int> possibleToppings = new List<int> { 4, 5, 6, 7, 8 };
    List<int> kindOfMachines = new List<int>();
    List<int> kindOfCookedMeals = new List<int>();
    List<int> kindOfContainer = new List<int>();
    List<int> kindOfIngredients = new List<int>();

    List<int> availableToppings = new List<int>();
    List<int> recepiesToShow = new List<int>();
    List<IceCreamOrders> ordersList = new List<IceCreamOrders>();
    List<float> timesOfOrder = new List<float>();

    List<GameObject> fullTables = new List<GameObject>();
    List<string> tableNames = new List<string>();
    List<string> machineTableNames = new List<string>();

    Dictionary<string, int> tableMapPositions = new Dictionary<string, int>();

    readonly Dictionary<string, string> plusCornerPositions = new Dictionary<string, string> { { "C1", "U1" }, { "C2", "R1" }, { "C3", "D1" }, { "C4", "L1" } };
    readonly Dictionary<string, string> lessCornerPositions = new Dictionary<string, string> { { "C1", "L5" }, { "C2", "U7" }, { "C3", "R5" }, { "C4", "D7" } };
    readonly List<int> levelsTutorials = new List<int> { 0, 1, 2 };

    IcecreamUI icecreamUI;

    ParticleSystem confettiSystem;
    ParticleSystem crossesSystem;

    int maxAmountOfOrdersAtTheSameTime = 5;
    int minAmountOforders = 1;

    int amountOfTopings = 5;
    int amoutOfChoppers = 1;

    int amountOfTrashPlaces = 1;

    int currentOrders;

    int currentEssay = 0;
    const int maxNumberOfEssay = 4;

    int[] wellMade;
    int[] badMade;
    int[] trashOrders;
    int[] ordersAsked;
    int[] ordersDelivered;
    int[] ordersMissed;
    int[] tipsEarned;
    int[] ordersWithTips;
    int[] totalScores;

    int level = 0;
    int maxLevelNumber = 4;

    int targetCoins;

    const float essayTime = 180f;
    float currentEssayTime;

    float newOrderTiming = 20f;
    float currentNewOrderTiming;

    bool isGameTime = false;
    bool needsTutorialIcecream;
    bool needsTutorialBlender;
    bool needsTutorialWaffle;
    #region Standart Region

    // Use this for initialization
    void Awake()
    {
        GameParametersInitialization();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameTime)
        {
            NewOrderForTime();

            SetTimersOnTime();
        }
    }

    #endregion

    #region Initilizations
    /// <summary>
    /// This one start all the parameters that only has to be initialized one time per game session
    /// </summary>
    void GameParametersInitialization()
    {
        TableInitilization();

        //Counters are initilized
        wellMade = new int[maxNumberOfEssay];
        badMade = new int[maxNumberOfEssay];
        trashOrders = new int[maxNumberOfEssay];
        ordersAsked = new int[maxNumberOfEssay];
        ordersDelivered = new int[maxNumberOfEssay];
        ordersMissed = new int[maxNumberOfEssay];
        tipsEarned = new int[maxNumberOfEssay];
        ordersWithTips = new int[maxNumberOfEssay];
        totalScores = new int[maxNumberOfEssay];

        //Especilieced Objects are initialized
        icecreamUI = new IcecreamUI(uiCanvas, this);

        confettiSystem = GameObject.FindGameObjectWithTag("Arrow").GetComponent<ParticleSystem>();
        crossesSystem = GameObject.FindGameObjectWithTag("Ground").GetComponent<ParticleSystem>();

    }

    /// <summary>
    /// This parameters has to be set in every essay of the game session
    /// </summary>
    public void EssayParameterInitilization()
    {
        currentEssayTime = essayTime;

        currentNewOrderTiming = newOrderTiming;

        SetLevelData();

        ClearTables();
    }

    public void SetChef(string chefName)
    {
        string pathOfChef = $"{FoodDicctionary.prefabGameObjectDirection}{FoodDicctionary.chefDirection}{chefName}";
        chef = Instantiate(Resources.Load<GameObject>(pathOfChef)).GetComponent<IcecreamChef>();
    }

    /// <summary>
    /// Obatain all the data need for the tables
    /// </summary>
    void TableInitilization()
    {
        for (int i = 0; i < TableCenterManager.transform.childCount; i++)
        {
            for (int j = 0; j < TableCenterManager.transform.GetChild(i).childCount; j++)
            {
                var gameObjectToDeal = TableCenterManager.transform.GetChild(i).GetChild(j).gameObject;
                fullTables.Add(gameObjectToDeal);
                tableNames.Add(gameObjectToDeal.name);
                tableMapPositions.Add(gameObjectToDeal.name, tableNames.Count - 1);
                if (gameObjectToDeal.name.Contains("U") || gameObjectToDeal.name.Contains("D"))
                {
                    machineTableNames.Add(gameObjectToDeal.name);
                }
            }
        }
    }

    /// <summary>
    /// We destroy all the previous table information to reset a new game
    /// </summary>
    void ClearTables()
    {
        for (int i = 0; i < fullTables.Count; i++)
        {
            if (fullTables[i].GetComponent<Table>())
            {
                fullTables[i].GetComponent<Table>().RestoreToOriginal();
                Destroy(fullTables[i].GetComponent<Table>());
            }

        }
    }

    void SetLevelData()
    {
        var levelConfigurator = GameConfigurator.GetIcecreamConfiguration(level);

        kindOfMachines.Clear();
        kindOfContainer.Clear();
        kindOfCookedMeals.Clear();
        kindOfMachines.Clear();
        kindOfIngredients.Clear();
        availableToppings.Clear();
        recepiesToShow.Clear();

        NeedsTutorial();

        amoutOfChoppers = levelConfigurator.amountOfChoppers;
        amountOfTopings = levelConfigurator.amountOfToppings;

        targetCoins = levelConfigurator.amountOfCoinsToPassLevel;

        maxAmountOfOrdersAtTheSameTime = levelConfigurator.maxNumberOfOrders;

        newOrderTiming = levelConfigurator.newOrderTiming;

        for (int i = 0; i < levelConfigurator.kindOfMeals.Count; i++)
        {
            int kindOfMeal = levelConfigurator.kindOfMeals[i];
            kindOfCookedMeals.Add(kindOfMeal);
            kindOfContainer.Add(kindOfMeal);
            switch (kindOfMeal)
            {
                case 0:
                    kindOfMachines.Add(0);
                    break;
                case 1:
                    kindOfMachines.Add(1);
                    kindOfIngredients.Add(0);
                    break;
                case 2:
                    kindOfMachines.Add(2);
                    kindOfMachines.Add(3);
                    kindOfIngredients.Add(1);
                    kindOfIngredients.Add(2);
                    kindOfIngredients.Add(3);
                    break;
                default:
                    Debug.LogError("its an exception");
                    break;
            }
        }

        icecreamUI.SetNeedCoins(targetCoins);
    }
    #endregion

    public int GetNeededCoins()
    {
        return targetCoins;
    }

    void NeedsTutorial()
    {
        if (levelsTutorials.Contains(level))
        {
            switch (level)
            {
                case 0:
                    recepiesToShow.Add(0);
                    recepiesToShow.Add(1);
                    recepiesToShow.Add(2);
                    break;
                case 1:
                    recepiesToShow.Add(3);
                    recepiesToShow.Add(4);
                    break;
                case 2:
                    recepiesToShow.Add(5);
                    recepiesToShow.Add(6);
                    recepiesToShow.Add(7);
                    recepiesToShow.Add(8);
                    recepiesToShow.Add(9);
                    break;
                default:
                    break;
            }
        }
    }

    void SetTimersOnTime()
    {
        foreach (IceCreamOrders order in ordersList)
        {
            order.ChangeTimeStatus();
        }
        currentEssayTime -= Time.deltaTime;
        icecreamUI.PrintTheStoreTime(currentEssayTime);

        if (currentEssayTime <= 0)
        {
            CloseRestaurant();
        }
    }

    void CloseRestaurant()
    {
        isGameTime = false;
        chef.PrepareTheChef();
        foreach (IceCreamOrders order in ordersList)
        {
            Destroy(order.order);
        }
        ordersList.Clear();
        ClearTables();
        int totalScore = icecreamUI.PrintTheEarnings(wellMade[currentEssay], tipsEarned[currentEssay], (badMade[currentEssay] + trashOrders[currentEssay]), ordersMissed[currentEssay]);
        totalScores[currentEssay] = totalScore;
        icecreamUI.SetButtonToPrintResults(totalScore, targetCoins);
    }

    public void HandleResult(bool passTheLevel)
    {
        currentEssay++;
        if (passTheLevel)
        {
            level++;
        }
        if (currentEssay < maxNumberOfEssay)
        {
            icecreamUI.SetButtonToPlayAgain();
        }
        else
        {
            icecreamUI.SetButtonToFinish();
        }
    }

    void NewOrderForTime()
    {
        currentNewOrderTiming -= Time.deltaTime;
        if (currentNewOrderTiming <= 0)
        {
            currentNewOrderTiming = newOrderTiming;
            AskForAnOrder();
        }
    }

    public void FillRandomTables()
    {
        var tempTablenames = new List<string>();
        var tempMachineTableNames = new List<string>();

        foreach (string s in tableNames)
        {
            tempTablenames.Add(s);
        }

        foreach (string s in machineTableNames)
        {
            tempMachineTableNames.Add(s);
        }

        //Create the finsh tables
        int randomFinishPlace = Random.Range(0, TableCenterManager.transform.GetChild(0).childCount);
        fullTables[tableMapPositions[tempTablenames[randomFinishPlace]]].AddComponent<TableFinish>();
        if (Random.Range(0, 2) == 0)
        {
            fullTables[tableMapPositions[plusCornerPositions[tempTablenames[randomFinishPlace]]]].AddComponent<TableFinish>();
            if (plusCornerPositions[tempTablenames[randomFinishPlace]].Contains("U") || plusCornerPositions[tempTablenames[randomFinishPlace]].Contains("D"))
            {
                tempMachineTableNames.Remove(plusCornerPositions[tempTablenames[randomFinishPlace]]);
            }
            tempTablenames.Remove(plusCornerPositions[tempTablenames[randomFinishPlace]]);
        }
        else
        {
            fullTables[tableMapPositions[lessCornerPositions[tempTablenames[randomFinishPlace]]]].AddComponent<TableFinish>();
            if (lessCornerPositions[tempTablenames[randomFinishPlace]].Contains("U") || lessCornerPositions[tempTablenames[randomFinishPlace]].Contains("D"))
            {
                tempMachineTableNames.Remove(lessCornerPositions[tempTablenames[randomFinishPlace]]);
            }
            tempTablenames.Remove(lessCornerPositions[tempTablenames[randomFinishPlace]]);
        }
        tempTablenames.Remove(tempTablenames[randomFinishPlace]);

        //Create the trash Table
        int randomTrashPlace = Random.Range(0, TableCenterManager.transform.GetChild(0).childCount-1);
        fullTables[tableMapPositions[tempTablenames[randomTrashPlace]]].AddComponent<TableTrash>();
        tempTablenames.Remove(tempTablenames[randomTrashPlace]);

        fullTables[tableMapPositions[tempTablenames[0]]].AddComponent<TableSimple>();
        tempTablenames.RemoveAt(0);
        fullTables[tableMapPositions[tempTablenames[0]]].AddComponent<TableSimple>();
        tempTablenames.RemoveAt(0);

        for (int i = 0; i < kindOfMachines.Count; i++)
        {
            int numToInput = Random.Range(0, tempMachineTableNames.Count);
            switch (kindOfMachines[i])
            {
                case 0:
                    fullTables[tableMapPositions[tempMachineTableNames[numToInput]]].AddComponent<TableIcreamMachine>();
                    break;
                case 1:
                    fullTables[tableMapPositions[tempMachineTableNames[numToInput]]].AddComponent<TableBlender>();
                    break;
                case 2:
                    fullTables[tableMapPositions[tempMachineTableNames[numToInput]]].AddComponent<TableMixer>();
                    break;
                case 3:
                    fullTables[tableMapPositions[tempMachineTableNames[numToInput]]].AddComponent<TableWaffleMaker>();
                    break;
            }
            tempTablenames.Remove(tempMachineTableNames[numToInput]);
            tempMachineTableNames.Remove(tempMachineTableNames[numToInput]);
        }

        //Create all the toppings that will be avilable
        var tempToppings = new List<int>();
        foreach (int i in possibleToppings)
        {
            tempToppings.Add(i);
        }
        for (int i = 0; i < amountOfTopings; i++)
        {
            int numToInput = Random.Range(0, tempTablenames.Count);
            int toppingNum = Random.Range(0, tempToppings.Count);
            fullTables[tableMapPositions[tempTablenames[numToInput]]].AddComponent<TableIngredients>().ingridientNumber = tempToppings[toppingNum];
            availableToppings.Add(tempToppings[toppingNum]);
            tempTablenames.Remove(tempTablenames[numToInput]);
            tempToppings.Remove(tempToppings[toppingNum]);
        }

        for (int i = 0; i < kindOfIngredients.Count; i++)
        {
            int numToInput = Random.Range(0, tempTablenames.Count);
            fullTables[tableMapPositions[tempTablenames[numToInput]]].AddComponent<TableIngredients>().ingridientNumber = kindOfIngredients[i];
            tempTablenames.Remove(tempTablenames[numToInput]);
        }

        //Creation of chopper tables
        for (int i = 0; i < amoutOfChoppers; i++)
        {
            int numToInput = Random.Range(0, tempTablenames.Count);
            fullTables[tableMapPositions[tempTablenames[numToInput]]].AddComponent<TableChopper>();
            tempTablenames.Remove(tempTablenames[numToInput]);
        }

        //Crate the table containers that will be in the game
        for (int i = 0; i < kindOfContainer.Count; i++)
        {
            int numToInput = Random.Range(0, tempTablenames.Count);
            fullTables[tableMapPositions[tempTablenames[numToInput]]].AddComponent<TableContainers>().kindOfContainer = kindOfContainer[i];
            tempTablenames.Remove(tempTablenames[numToInput]);
        }

        for (int i = 0; i < tempTablenames.Count; i++)
        {
            fullTables[tableMapPositions[tempTablenames[i]]].AddComponent<TableSimple>();
        }
    }

    void AskForAnOrder()
    {
        if (ordersList.Count < maxAmountOfOrdersAtTheSameTime)
        {
            ordersAsked[currentEssay]++;
            IceCreamOrders tempOrder = new IceCreamOrders(Instantiate(Resources.Load<GameObject>("IcecreamMadness/Prefabs/Order"), Canvas.transform.GetChild(0)), this);
            int randomFood = Random.Range(0, kindOfContainer.Count);
            int randomTooping = Random.Range(0, availableToppings.Count);
            tempOrder.SetAnOrder(kindOfContainer[randomFood], kindOfCookedMeals[randomFood], availableToppings[randomTooping]);
            tempOrder.SetOrderPosistion(ordersList.Count);
            ordersList.Add(tempOrder);
            tempOrder.order.name = ("Order_" + ordersList.Count);
        }
    }

    public bool CompareTrays(int?[] trayDelivered)
    {
        bool hasAMatch = false;

        for (int i = 0; i < ordersList.Count; i++)
        {
            if (ordersList[i].IsTheOrderWellMade(trayDelivered))
            {
                int tip = ordersList[i].TipsForThisOrder();
                if (tip > 0)
                {
                    ordersWithTips[currentEssay]++;
                }
                tipsEarned[currentEssay] += tip;
                Destroy(ordersList[i].order);
                ordersList.Remove(ordersList[i]);
                hasAMatch = true;
                break;
            }
        }
        if (hasAMatch)
        {
            for (int i = 0; i < ordersList.Count; i++)
            {
                ordersList[i].SetOrderPosistion(i);
            }

            if (ordersList.Count <= 0)
            {
                AskForAnOrder();
            }
        }

        return hasAMatch;
    }

    public void GoodAnswer(Vector3 positionOfTable)
    {
        wellMade[currentEssay]++;
        StartCoroutine(AnswerRoutine(confettiSystem, positionOfTable));
    }

    public void BadAnswer(Vector3 positionOfTable)
    {
        badMade[currentEssay]++;
        crossesSystem.transform.GetChild(0).gameObject.SetActive(false);
        crossesSystem.transform.GetChild(1).gameObject.SetActive(true);
        StartCoroutine(AnswerRoutine(crossesSystem, positionOfTable));

    }

    public void TrashAnswer(Vector3 positionOfTable)
    {
        trashOrders[currentEssay]++;
        crossesSystem.transform.GetChild(0).gameObject.SetActive(false);
        crossesSystem.transform.GetChild(1).gameObject.SetActive(true);
        StartCoroutine(AnswerRoutine(crossesSystem, positionOfTable));

    }

    IEnumerator AnswerRoutine(ParticleSystem particleSystem, Vector3 positionOfTable)
    {
        var initialPos = particleSystem.transform.position;
        particleSystem.transform.position = positionOfTable;
        particleSystem.Play();

        while (particleSystem.isPlaying)
        {
            yield return null;
        }

        particleSystem.transform.position = initialPos;

    }

    public void MissOrder(IceCreamOrders orderToDelete)
    {

        ordersMissed[currentEssay]++;
        crossesSystem.transform.GetChild(0).gameObject.SetActive(true);
        crossesSystem.transform.GetChild(1).gameObject.SetActive(false);
        StartCoroutine(MissRoutine(crossesSystem, orderToDelete));
    }

    IEnumerator MissRoutine(ParticleSystem particleSystem, IceCreamOrders orderToDelete)
    {

        var initialPos = particleSystem.transform.position;

        particleSystem.transform.position = orderToDelete.order.transform.position;

        ordersList.Remove(orderToDelete);
        Destroy(orderToDelete.order);

        particleSystem.Play();

        while (particleSystem.isPlaying)
        {
            yield return null;
        }

        particleSystem.transform.position = initialPos;
        for (int i = 0; i < ordersList.Count; i++)
        {
            ordersList[i].SetOrderPosistion(i);
        }
    }

    public bool IsGameOnAction()
    {
        return isGameTime;
    }

    public void StartGameNow()
    {
        isGameTime = true;
        AskForAnOrder();
    }

    public List<int> GetRecipes()
    {
        return recepiesToShow;
    }
}

public class IceCreamOrders
{
    public GameObject order;
    public IcecreamMadnessManager manager;
    public Image container;
    public Image cookedMeal;
    public Image topping;
    public Image baseBar;
    public Image dynamicBar;
    public Image helperImage;

    int?[] orderType = new int?[3];
    float time;
    float totalTime;

    const float originalPositionInX = 65f;
    const float spaceBetweenCenters = 130f;

    public IceCreamOrders(GameObject prefabOrder, IcecreamMadnessManager managerScript)
    {
        manager = managerScript;
        order = prefabOrder;
        container = prefabOrder.transform.GetChild(0).GetComponent<Image>();
        cookedMeal = container.transform.GetChild(0).GetComponent<Image>();
        topping = cookedMeal.transform.GetChild(0).GetComponent<Image>();
        baseBar = prefabOrder.transform.GetChild(1).GetComponent<Image>();
        dynamicBar = baseBar.transform.GetChild(0).GetComponent<Image>();
        helperImage = order.transform.GetChild(2).GetComponent<Image>();
    }

    public void SetAnOrder(int? container, int? cookedMeal, int? topping)
    {
        orderType[0] = container;
        orderType[1] = cookedMeal;
        orderType[2] = topping;
        float timeToDeliver = 0;
        switch (orderType[0])
        {
            case 0:
                timeToDeliver = FoodDicctionary.icecremPreparationTime;
                break;
            case 1:
                timeToDeliver = FoodDicctionary.smoothiePreparationTime;
                break;
            case 2:
                timeToDeliver = FoodDicctionary.wafflePreparationTime;
                break;
            default:
                timeToDeliver = FoodDicctionary.icecremPreparationTime;
                break;
        }
        totalTime = timeToDeliver;
        SetImage();
    }

    void SetImage()
    {
        if (orderType[0] != null) {
            container.sprite = LoadSprite.GetSpriteFromSpriteSheet($"{FoodDicctionary.prefabSpriteDirection}{FoodDicctionary.containersDirection}", FoodDicctionary.Containers.ShapeOfConatiner((int)orderType[0]));
        }
        if (orderType[1] != null)
        {
            cookedMeal.sprite = LoadSprite.GetSpriteFromSpriteSheet($"{FoodDicctionary.prefabSpriteDirection}{FoodDicctionary.cookedMealDirection}", FoodDicctionary.CookedMeal.ShapeOfCookedMeal((int)orderType[1]));
            if (orderType[1] == 1)
            {
                cookedMeal.color = FoodDicctionary.Toppings.ColorOfSmoothie((int)orderType[2]);
            }
        }
        if (orderType[2] != null)
        {
            topping.sprite = LoadSprite.GetSpriteFromSpriteSheet($"{FoodDicctionary.prefabSpriteDirection}{FoodDicctionary.toppingServedDirection}{FoodDicctionary.CookedMeal.ShapeOfCookedMeal((int)orderType[1])}", FoodDicctionary.Toppings.ShapeOfTopping((int)orderType[2]));
            helperImage.sprite = LoadSprite.GetSpriteFromSpriteSheet($"{FoodDicctionary.prefabSpriteDirection}{FoodDicctionary.helperToppingDirection}", FoodDicctionary.Toppings.ShapeOfTopping((int)orderType[2]));
        }
    }

    Sprite ChangeImage(string shape)
    {
        return Resources.Load<Sprite>($"IcecreamMadness/Sprites/{shape}");
    }

    public void SetOrderPosistion(int number)
    {
        float xPosition = originalPositionInX + (number * spaceBetweenCenters);
        Vector3 localPos = order.GetComponent<RectTransform>().anchoredPosition;
        localPos.x = xPosition;
        order.GetComponent<RectTransform>().anchoredPosition = localPos;
    }

    public bool IsTheOrderWellMade(int?[] dishToCompare)
    {
        for (int i = 0; i < orderType.Length; i++)
        {
            if (orderType[i] != dishToCompare[i])
            {
                return false;
            }
        }

        return true;
    }

    public void ThisOrderIsServed()
    {

    }

    public int TipsForThisOrder()
    {
        float percentageOfTimeSpend = time / totalTime;
        if (percentageOfTimeSpend <= 0.25)
        {
            return 8;
        }
        else if (percentageOfTimeSpend <= 0.50)
        {
            return 4;
        }
        else if (percentageOfTimeSpend <= 0.75)
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    public void MissedOrder()
    {
        manager.MissOrder(this);
    }

    public void ChangeTimeStatus()
    {
        time += Time.deltaTime;
        float percentageTimeLeft = time / totalTime;
        dynamicBar.fillAmount = percentageTimeLeft;
        if (time > totalTime)
        {
            MissedOrder();
        }
    }
}

class IcecreamUI
{
    GameObject mainCanvas;

    Image timerReadyPanel;
    Text timerReadyText;
    Button OkButton;

    Image timerPanel;
    Text timerText;

    GameObject storyManager;
    List<GameObject> storyImages = new List<GameObject>();

    Image storyPanel;
    Text storyText;
    Button nextButton;
    string[] storyStrings = new string[]{
        "Los animales de la isla necesitan de nutrientes para seguir jugando",
        "Las frutas que tenemos en la isla son una gran fuente de vitaminas",
        "Ayudanos a preparar deliciosos platillos frutales para todos"
    };

    int storyIndex = 0;
    int recipesIndex = 0;

    GameObject recipesPanel;
    List<GameObject> recipesObjects = new List<GameObject>();
    Button recipesButton;

    GameObject characterSelectionPanel;
    GameObject selectCharacterPanel;
    Text selectCharacterText;

    IcecreamMadnessManager manager;

    public IcecreamUI(GameObject canvas, IcecreamMadnessManager managerToRefer)
    {
        manager = managerToRefer;
        mainCanvas = canvas;

        timerReadyPanel = mainCanvas.transform.GetChild(0).GetComponent<Image>();
        timerReadyText = timerReadyPanel.transform.GetChild(0).GetComponent<Text>();
        OkButton = timerReadyPanel.transform.GetChild(1).GetComponent<Button>();

        timerPanel = mainCanvas.transform.GetChild(1).GetComponent<Image>();
        timerText = timerPanel.GetComponentInChildren<Text>();

        storyManager = mainCanvas.transform.GetChild(2).gameObject;
        for (int i = 0; i < storyManager.transform.childCount; i++)
        {
            storyImages.Add(storyManager.transform.GetChild(i).gameObject);
        }

        storyPanel = mainCanvas.transform.GetChild(3).GetComponent<Image>();
        storyText = storyPanel.GetComponentInChildren<Text>();
        nextButton = storyPanel.GetComponentInChildren<Button>();
        nextButton.onClick.AddListener(PrintTheStory);

        recipesPanel = mainCanvas.transform.GetChild(4).gameObject;
        for (int i = 0; i < recipesPanel.transform.childCount - 1; i++)
        {
            recipesObjects.Add(recipesPanel.transform.GetChild(i).gameObject);
            recipesObjects[i].SetActive(false);
        }
        recipesButton = recipesPanel.transform.GetChild(recipesPanel.transform.childCount - 1).GetComponent<Button>();

        characterSelectionPanel = mainCanvas.transform.GetChild(5).gameObject;
        selectCharacterPanel = characterSelectionPanel.transform.GetChild(0).gameObject;
        selectCharacterText = selectCharacterPanel.transform.GetComponentInChildren<Text>();
        var characterSelectionManager = characterSelectionPanel.transform.GetChild(1);
        for (int i = 0; i < characterSelectionManager.childCount; i++)
        {
            var buttonToSet = characterSelectionManager.GetChild(i).GetComponent<Button>();
            buttonToSet.onClick.AddListener(() => SelectCharacter(buttonToSet.transform.GetChild(0).name));
        }

        HideAllManagers();
        characterSelectionPanel.SetActive(true);
    }

    void SelectCharacter(string nameOfCharacter)
    {
        manager.SetChef(nameOfCharacter);
        PrintTheStory();
    }

    void HideAllManagers()
    {
        timerReadyPanel.gameObject.SetActive(false);
        timerPanel.gameObject.SetActive(false);
        storyManager.SetActive(false);
        storyPanel.gameObject.SetActive(false);
        recipesPanel.SetActive(false);
        characterSelectionPanel.SetActive(false);
    }

    public void PrintTheStory()
    {
        HideAllManagers();
        storyManager.SetActive(true);
        storyPanel.gameObject.SetActive(true);

        foreach (GameObject o in storyImages)
        {
            o.SetActive(false);
        }

        storyImages[storyIndex].SetActive(true);
        storyText.text = storyStrings[storyIndex];

        storyIndex++;
        if (storyIndex >= storyImages.Count)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(PrintAreYouReady);
        }
    }

    public void PrintRecipes()
    {
        HideAllManagers();

        recipesPanel.SetActive(true);
        var recipes = manager.GetRecipes();

        foreach (GameObject o in recipesObjects)
        {
            o.SetActive(false);
        }

        recipesObjects[recipes[recipesIndex]].SetActive(true);

        recipesIndex++;

        recipesButton.onClick.RemoveAllListeners();
        if (recipesIndex < recipes.Count)
        {
            recipesButton.onClick.AddListener(PrintRecipes);
        }
        else
        {
            recipesIndex = 0;
            recipesButton.onClick.AddListener(() => PrintYouNeed(manager.GetNeededCoins()));
        }
    }

    public void PrintAreYouReady()
    {
        HideAllManagers();
        timerReadyPanel.gameObject.SetActive(true);
        timerReadyText.text = "¿Estas listo?";

        manager.EssayParameterInitilization();
    }

    public void SetNeedCoins(int needs)
    {
        OkButton.onClick.RemoveAllListeners();
        if (manager.GetRecipes().Count > 0)
        {
            OkButton.onClick.AddListener(PrintRecipes);
        }
        else
        {
            OkButton.onClick.AddListener(() => PrintYouNeed(needs));
        }
    }

    public void PrintYouNeed(int neededCoins)
    {
        HideAllManagers();
        timerReadyPanel.gameObject.SetActive(true);
        timerReadyText.text = $"Necesitas ${neededCoins} para pasar de nivel";

        manager.EssayParameterInitilization();

        OkButton.onClick.RemoveAllListeners();
        OkButton.onClick.AddListener(() => { manager.StartCoroutine(StartCountDown()); });
    }

    public IEnumerator StartCountDown()
    {
        HideAllManagers();
        timerReadyPanel.gameObject.SetActive(true);
        OkButton.gameObject.SetActive(false);
        manager.FillRandomTables();
        timerReadyText.text = "Abrimos en:\n 3";
        yield return new WaitForSeconds(1f);
        timerReadyText.text = "Abrimos en:\n 2";
        yield return new WaitForSeconds(1f);
        timerReadyText.text = "Abrimos en:\n 1";
        yield return new WaitForSeconds(1f);
        timerReadyText.text = "Comencemos";
        yield return new WaitForSeconds(1f);
        timerReadyPanel.gameObject.SetActive(false);
        timerPanel.gameObject.SetActive(true);
        manager.StartGameNow();

    }

    public void PrintTheStoreTime(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"<color=#{ColorToPrint(minutes)}>{minutes}:{seconds.ToString("00")}</color>";
    }

    string ColorToPrint(int minutesRemaining)
    {
        var returner = minutesRemaining > 0 ? "42210B" : "B32006";
        return returner;
    }

    string ColorToPrintTotal(int totalAmount)
    {
        var returner = totalAmount >= 0 ? "42210B" : "B32006";
        return returner;
    }

    string SimbolToPrint(int totalAmount)
    {
        var returner = totalAmount < 0 ? " - " : "   ";
        return returner;
    }

    public int PrintTheEarnings(int objectsWellMade, int tipScore, int objectsBadMade, int objectsMissed)
    {
        timerReadyPanel.gameObject.SetActive(true);
        timerPanel.gameObject.SetActive(false);
        OkButton.gameObject.SetActive(true);

        int salesTotal = objectsWellMade * 20;
        int lossesTotal = objectsBadMade * 5;
        int penalizationTotal = objectsMissed * 10;
        int totals = salesTotal + tipScore - lossesTotal - penalizationTotal;

        string sales = $"<color=#42210B>Ventas     :   ${salesTotal.ToString("000")}</color>";
        string tips = $"<color=#42210B>Propinas  :   ${tipScore.ToString("000")}</color>";
        string looses = $"<color=#B32006>Perdidas  : - ${lossesTotal.ToString("000")}</color>";
        string penalization = $"<color=#B32006>Reclamos : - ${penalizationTotal.ToString("000")}</color>";
        string total = $"<color=#{ColorToPrintTotal(totals)}>Total         :{SimbolToPrint(totals)}${Mathf.Abs(totals).ToString("000")}</color>";



        timerReadyText.text = $"{sales}\n{tips}\n{looses}\n{penalization}\n{total}";

        return totals;
    }

    public void PrintTheResults(int totalCoins, int needCoins)
    {
        string total = $"<color=#42210B>Ganaste        :   ${totalCoins.ToString("000")}</color>";
        string needs = $"<color=#42210B>Necesitabas :   ${needCoins.ToString("000")}</color>";
        timerReadyText.text = $"{total}\n{needs}";

        bool isCorrect = totalCoins >= needCoins;

        if (isCorrect)
        {
            timerReadyText.text += "\n¡Muy Bien! Lo conseguiste avanza al siguiente nivel";
        }
        else
        {
            timerReadyText.text += "\nOh no! vuelve a intentarlo";
        }

        manager.HandleResult(isCorrect);
    }

    public void SetButtonToPrintResults(int totals, int needCoins)
    {
        OkButton.onClick.RemoveAllListeners();
        OkButton.onClick.AddListener(() => PrintTheResults(totals, needCoins));
    }

    public void SetButtonToPlayAgain()
    {
        OkButton.onClick.RemoveAllListeners();
        OkButton.onClick.AddListener(PrintAreYouReady);
    }

    public void SetButtonToFinish()
    {
        OkButton.onClick.RemoveAllListeners();
        OkButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
    } 
}
