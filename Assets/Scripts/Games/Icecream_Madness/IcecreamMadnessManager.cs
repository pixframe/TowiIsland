using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IcecreamMadnessManager : MonoBehaviour {
    public GameObject TableCenterManager;
    public GameObject Canvas;

    readonly List<int> possibleToppings = new List<int> { 4, 5, 6, 7, 8 };
    List<int> kindOfMachines = new List<int> { 0 };
    List<int> kindOfCookedMeals = new List<int> { 0 };
    List<int> kindOfContainer = new List<int> { 0 };
    List<int> availableToppings = new List<int>();
    List<IceCreamOrders> ordersList = new List<IceCreamOrders>();
    List<float> timesOfOrder = new List<float>();

    List<GameObject> FullTables = new List<GameObject>();
    List<string> tableNames = new List<string>();

    Dictionary<string, int> tableMapPositions = new Dictionary<string, int>();
    readonly Dictionary<string, string> plusCornerPositions = new Dictionary<string, string> { { "C1", "U1" }, { "C2", "R1" }, { "C3", "D1" }, { "C4", "L1" } };
    readonly Dictionary<string, string> lessCornerPositions = new Dictionary<string, string> { { "C1", "L5" }, { "C2", "U7" }, { "C3", "R5" }, { "C4", "D7" } };

    int maxAmountOfOrdersAtTheSameTime = 5;

    int amountOfTopings = 5;
    int amoutOfMachines = 1;
    int amoutOfChoppers = 1;

    int amountOfTrashPlaces = 1;

    int wellMade;
    int ordersAsked;
    
    // Use this for initialization
    void Awake()
    {
        for (int i = 0; i < TableCenterManager.transform.childCount; i++)
        {
            for (int j = 0; j < TableCenterManager.transform.GetChild(i).childCount; j++)
            {
                FullTables.Add(TableCenterManager.transform.GetChild(i).GetChild(j).gameObject);
                tableNames.Add(TableCenterManager.transform.GetChild(i).GetChild(j).name);
                tableMapPositions.Add(tableNames[tableNames.Count - 1], tableNames.Count - 1);
            }
        }

        ClearTables();

        FillRandomTables();

        AskForAnOrder();

        AskForAnOrder();
    }

    void FillRandomTables()
    {
        var tempTablenames = tableNames;

        //Create the finsh tables
        int randomFinishPlace = Random.Range(0, TableCenterManager.transform.GetChild(0).childCount);
        FullTables[tableMapPositions[tempTablenames[randomFinishPlace]]].AddComponent<TableFinish>();
        if (Random.Range(0, 2) == 0)
        {
            FullTables[tableMapPositions[plusCornerPositions[tempTablenames[randomFinishPlace]]]].AddComponent<TableFinish>();
            tempTablenames.Remove(plusCornerPositions[tempTablenames[randomFinishPlace]]);
        }
        else
        {
            FullTables[tableMapPositions[lessCornerPositions[tempTablenames[randomFinishPlace]]]].AddComponent<TableFinish>();
            tempTablenames.Remove(lessCornerPositions[tempTablenames[randomFinishPlace]]);
        }
        tempTablenames.Remove(tempTablenames[randomFinishPlace]);

        //Create the trash Table
        int randomTrashPlace = Random.Range(0, TableCenterManager.transform.GetChild(0).childCount-1);
        FullTables[tableMapPositions[tempTablenames[randomTrashPlace]]].AddComponent<TableTrash>();
        tempTablenames.Remove(tempTablenames[randomTrashPlace]);

        tempTablenames.RemoveAt(0);
        tempTablenames.RemoveAt(0);

        //Create all the toppings that will be avilable
        var tempToppings = possibleToppings;
        for (int i = 0; i < amountOfTopings; i++)
        {
            int numToInput = Random.Range(0, tempTablenames.Count);
            int toppingNum = Random.Range(0, tempToppings.Count);
            FullTables[tableMapPositions[tempTablenames[numToInput]]].AddComponent<TableIngredients>().ingridientNumber = tempToppings[toppingNum];
            availableToppings.Add(tempToppings[toppingNum]);
            tempTablenames.Remove(tempTablenames[numToInput]);
            tempToppings.Remove(tempToppings[toppingNum]);
        }

        //Creation of chopper tables
        for (int i = 0; i < amoutOfChoppers; i++)
        {
            int numToInput = Random.Range(0, tempTablenames.Count);
            FullTables[tableMapPositions[tempTablenames[numToInput]]].AddComponent<TableChopper>();
            tempTablenames.Remove(tempTablenames[numToInput]);
        }

        for (int i = 0; i < kindOfMachines.Count; i++)
        {
            int numToInput = Random.Range(0, tempTablenames.Count);
            switch (kindOfMachines[i])
            {
                case 0:
                    FullTables[tableMapPositions[tempTablenames[numToInput]]].AddComponent<TableIcreamMachine>();
                    break;
                case 1:
                    FullTables[tableMapPositions[tempTablenames[numToInput]]].AddComponent<TableBlender>();
                    break;
                case 2:
                    FullTables[tableMapPositions[tempTablenames[numToInput]]].AddComponent<TableMixer>();
                    break;
                default:
                    FullTables[tableMapPositions[tempTablenames[numToInput]]].AddComponent<TableChopper>();
                    break;
            }
            tempTablenames.Remove(tempTablenames[numToInput]);
        }

        //Crate the table containers that will be in the game
        for (int i = 0; i < kindOfContainer.Count; i++)
        {
            int numToInput = Random.Range(0, tempTablenames.Count);
            FullTables[tableMapPositions[tempTablenames[numToInput]]].AddComponent<TableContainers>().kindOfContainer = kindOfContainer[i];
            tempTablenames.Remove(tempTablenames[numToInput]);
        }

        for (int i = 0; i < tempTablenames.Count; i++)
        {
            FullTables[tableMapPositions[tempTablenames[i]]].AddComponent<TableSimple>();
        }
    }

    void AskForAnOrder()
    {
        if (ordersList.Count < 5)
        {
            Debug.Log("Create An order");
            IceCreamOrders tempOrder = new IceCreamOrders(Instantiate(Resources.Load<GameObject>("IcecreamMadness/Prefabs/Order"), Canvas.transform.GetChild(0)));
            int randomConatainer = Random.Range(0, kindOfContainer.Count);
            int randomCookedMeal = Random.Range(0, kindOfCookedMeals.Count);
            int randomTooping = Random.Range(0, availableToppings.Count);
            tempOrder.SetAnOrder(kindOfContainer[randomConatainer], kindOfCookedMeals[randomCookedMeal], availableToppings[randomTooping],20.0f);
            tempOrder.SetOrderPosistion(ordersList.Count);
            ordersList.Add(tempOrder);
            tempOrder.order.name = ("Order " + ordersList.Count);
        }
        else
        {

        }
    }

    public void CompareTrays(int?[] trayDelivered)
    {
        bool hasAMatch = false;

        for (int i = 0; i < ordersList.Count; i++)
        {
            if (ordersList[i].IsTheOrderWellMade(trayDelivered))
            {
                hasAMatch = true;
                Destroy(ordersList[i].order);
                ordersList.Remove(ordersList[i]);
                break;
            }
        }
        if (hasAMatch)
        {
            for (int i = 0; i < ordersList.Count; i++)
            {
                ordersList[i].SetOrderPosistion(i);
            }
        }
        else
        {
            Debug.Log("You deliver otmar");
        }
    }

    // Update is called once per frame
    void Update ()
    {
        foreach (IceCreamOrders order in ordersList)
        {
            order.ChangeTimeStatus();
        }
	}

    void ClearTables()
    {
        for (int i = 0; i < FullTables.Count; i++)
        {
            Destroy(FullTables[i].GetComponent<Table>());
        }
    }
}

class IceCreamOrders
{
    public GameObject order;
    public Image container;
    public Image cookedMeal;
    public Image topping;
    public Image baseBar;
    public Image dynamicBar;

    int?[] orderType = new int?[3];
    float time;
    float totalTime;

    const float originalPositionInX = 65f;
    const float spaceBetweenCenters = 130f;

    public IceCreamOrders(GameObject prefabOrder)
    {
        order = prefabOrder;
        container = prefabOrder.transform.GetChild(0).GetComponent<Image>();
        cookedMeal = container.transform.GetChild(0).GetComponent<Image>();
        topping = cookedMeal.transform.GetChild(0).GetComponent<Image>();
        baseBar = prefabOrder.transform.GetChild(1).GetComponent<Image>();
        dynamicBar = baseBar.transform.GetChild(0).GetComponent<Image>();
    }

    public void SetAnOrder(int? container, int? cookedMeal, int? topping, float timeToDeliver)
    {
        orderType[0] = container;
        orderType[1] = cookedMeal;
        orderType[2] = topping;
        time = timeToDeliver;
        totalTime = timeToDeliver;
        SetImage();
    }

    void SetImage()
    {
        if (orderType[0] != null) {
            container.sprite = ChangeImage(FoodDicctionary.Containers.ShapeOfConatiner((int)orderType[0]));
            container.color = Color.white;
        }
        if (orderType[1] != null)
        {
            cookedMeal.sprite = ChangeImage(FoodDicctionary.CookedMeal.ShapeOfCookedMeal((int)orderType[1]));
            cookedMeal.color = FoodDicctionary.CookedMeal.ColorOfCookedMeal((int)orderType[1]);
        }
        if (orderType[2] != null)
        {
            topping.sprite = ChangeImage(FoodDicctionary.Toppings.ShapeOfTopping((int)orderType[2]));
            topping.color = FoodDicctionary.RawIngridients.ColorOfRawIngridient((int)orderType[2]);
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

    public void ChangeTimeStatus()
    {
        time -= Time.deltaTime;
        float percentageTimeLeft = time / totalTime;
        dynamicBar.fillAmount = percentageTimeLeft;
    }
}
