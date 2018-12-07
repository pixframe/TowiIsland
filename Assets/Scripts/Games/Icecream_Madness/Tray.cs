using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour {

    int?[] typeOfObjects = new int?[3];

    bool hasAContainer;
    bool hasARawIngridient;
    bool hasACookIngridient;
    bool hasABaseFood;
    bool hasATopping;

    SpriteRenderer containerSpriteRenderer;
    SpriteRenderer foodSpriteRender;
    SpriteRenderer toppingSpriteRenderer;

    void Initialization()
    {
        containerSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        foodSpriteRender = containerSpriteRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>();
        toppingSpriteRenderer = foodSpriteRender.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public bool SetAContainer(int containerNumber, string spriteName, Color colorToPut)
    {
        Initialization();
        if (!hasARawIngridient)
        {
            typeOfObjects[0] = containerNumber;
            containerSpriteRenderer.color = colorToPut;
            containerSpriteRenderer.sprite = Resources.Load<Sprite>("IcecreamMadness/Icons/" + spriteName);
            hasAContainer = true;
            SetImage();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasAContainer()
    {
        return hasAContainer;
    }

    public int KindOfConatiner()
    {
        return (int)typeOfObjects[0];
    }

    public bool SetACookedMeal(int cookIngridients, string spriteName, Color colorToPut)
    {
        Initialization();
        if (!hasARawIngridient && !hasACookIngridient)
        {
            typeOfObjects[1] = cookIngridients;
            foodSpriteRender.color = colorToPut;
            foodSpriteRender.sprite = Resources.Load<Sprite>("IcecreamMadness/Icons/" + spriteName);
            hasACookIngridient = true;
            SetImage();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SetARawIngridient(int ingridientNumber , string spriteName, Color colorToPut)
    {
        Initialization();
        if (!hasARawIngridient && !hasACookIngridient && !hasABaseFood && !hasATopping && !hasAContainer)
        {
            typeOfObjects[1] = ingridientNumber;
            foodSpriteRender.color = colorToPut;
            foodSpriteRender.sprite = Resources.Load<Sprite>("IcecreamMadness/Icons/" + spriteName);
            hasARawIngridient = true;
            SetImage();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasRawIngridient()
    {
        return hasARawIngridient;
    }

    public bool SetAToping(int toppingNumber , string spriteName, Color colorToPut)
    {
        Initialization();
        if (!hasARawIngridient || !hasATopping)
        {
            typeOfObjects[2] = toppingNumber;
            toppingSpriteRenderer.color = colorToPut;
            toppingSpriteRenderer.sprite = Resources.Load<Sprite>("IcecreamMadness/Icons/" + spriteName);
            hasATopping = true;
            SetImage();
            return true;
        }
        else
        {
            return false;
        }
    }

    public int?[] GetIdentifiers()
    {
        return typeOfObjects;
    }

    public int GetIngridient()
    {
        Debug.Log("The ingridients is " + typeOfObjects[1]);
        return (int)typeOfObjects[1];
    }

    public void TransformIngridientToTopping(string shape)
    {
        hasARawIngridient = false;
        typeOfObjects[2] = typeOfObjects[1];
        typeOfObjects[1] = null;
        SetAToping((int)typeOfObjects[2], shape, foodSpriteRender.color);
    }

    void SetImage()
    {
        //Initialization();
        if (typeOfObjects[0] != null) containerSpriteRenderer.enabled = true; else containerSpriteRenderer.enabled = false;
        if (typeOfObjects[1] != null) foodSpriteRender.enabled = true; else foodSpriteRender.enabled = false;
        if (typeOfObjects[2] != null) toppingSpriteRenderer.enabled = true; else toppingSpriteRenderer.enabled = false;
    }

    public bool CanMergeTrays(Tray trayToMerge)
    {
        if (hasARawIngridient || trayToMerge.HasRawIngridient())
        {
            return false;
        }
        else
        {
            int?[] typesOfTheOtherTray = trayToMerge.GetIdentifiers();
            int?[] finalMergeData = new int?[typeOfObjects.Length];
            for (int i = 0; i < typeOfObjects.Length; i++)
            {
                if (typeOfObjects[i] != null && typesOfTheOtherTray[i] != null)
                {
                    return false;
                }
                if (typeOfObjects[i] != null)
                {
                    finalMergeData[i] = typeOfObjects[i];
                }
                else if (typesOfTheOtherTray[i] != null)
                {
                    finalMergeData[i] = typesOfTheOtherTray[i];
                }
            }
            if (finalMergeData[0] == null)
            {
                return false;
            }

            if (finalMergeData[0] != null && finalMergeData[1] == null && finalMergeData[2] != null)
            {
                return false;
            }

            if (finalMergeData[0] != null && finalMergeData[1] != null)
            {
                if (finalMergeData[0] != finalMergeData[1])
                {
                    return false;
                }
            }
            return true;
        }
    }

    public void MergeTrays(Tray trayToMerge)
    {
        int?[] typesOfTheOtherTray = trayToMerge.GetIdentifiers();
        int?[] finalMergeData = new int?[typeOfObjects.Length];
        for (int i = 0; i < typeOfObjects.Length; i++)
        {
            if (typeOfObjects[i] != null)
            {
                finalMergeData[i] = typeOfObjects[i];
            }
            else if (typesOfTheOtherTray[i] != null)
            {
                finalMergeData[i] = typesOfTheOtherTray[i];
            }
        }
        typeOfObjects = finalMergeData;
        Destroy(trayToMerge.gameObject);
        if (typeOfObjects[0] != null)
        {
            SetAContainer((int)typeOfObjects[0], FoodDicctionary.Containers.ShapeOfConatiner((int)typeOfObjects[0]), FoodDicctionary.Containers.ColorOfContainer((int)typeOfObjects[0]));
        }
        if (typeOfObjects[1] != null)
        {
        }
        if (typeOfObjects[2] != null)
        {
            SetAToping((int)typeOfObjects[2], FoodDicctionary.Toppings.ShapeOfTopping((int)typeOfObjects[2]), FoodDicctionary.RawIngridients.ColorOfRawIngridient((int)typeOfObjects[2]));
        }
        SetImage();
    }

    public void SetLayers(int layer)
    {
        containerSpriteRenderer.sortingOrder = layer + 1;
        foodSpriteRender.sortingOrder = containerSpriteRenderer.sortingOrder + 1;
        toppingSpriteRenderer.sortingOrder = foodSpriteRender.sortingOrder + 1;
    }

    public int?[] GetMadeComposition()
    {
        return typeOfObjects;
    }
}
