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

    public bool SetAContainer(int containerNumber, string spriteName)
    {
        Initialization();
        if (!hasARawIngridient)
        {
            typeOfObjects[0] = containerNumber;
            containerSpriteRenderer.sprite = LoadSprite.GetSpriteFromSpriteSheet($"{FoodDicctionary.prefabSpriteDirection}{FoodDicctionary.containersDirection}", spriteName);
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

    public bool SetACookedMeal(int cookIngridients, string spriteName)
    {
        Initialization();
        if (!hasARawIngridient && !hasACookIngridient)
        {
            typeOfObjects[1] = cookIngridients;
            foodSpriteRender.sprite = LoadSprite.GetSpriteFromSpriteSheet($"{FoodDicctionary.prefabSpriteDirection}{FoodDicctionary.cookedMealDirection}", spriteName);
            hasACookIngridient = true;
            //SetImage();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ChangeRawIngridientToTopping (int newIngridient)
    {
        typeOfObjects[2] = newIngridient;
        typeOfObjects[1] = null;
        hasARawIngridient = false;
    }

    public bool SetARawIngridient(int ingredientNumber, string spriteName)
    {
        Initialization();
        if (!hasARawIngridient && !hasACookIngridient && !hasABaseFood && !hasATopping && !hasAContainer)
        {
            DisableAllRenderers();
            foodSpriteRender.enabled = true;

            typeOfObjects[1] = ingredientNumber;
            foodSpriteRender.sprite = LoadSprite.GetSpriteFromSpriteSheet($"{FoodDicctionary.prefabSpriteDirection}{FoodDicctionary.ingredientDirection}", spriteName);
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

    public int? WhatKindOfIngridientsIs()
    {
        Debug.Log($"the ingredient we have is {typeOfObjects[1]}");
        return typeOfObjects[1];
    }

    public bool SetAToping(int toppingNumber, string spriteName)
    {
        Initialization();
        if (!hasARawIngridient || !hasATopping)
        {
            typeOfObjects[2] = toppingNumber;
            string directionOfSprite = "";
            if (typeOfObjects[0] != null && typeOfObjects[1] != null)
            {
                directionOfSprite = $"{FoodDicctionary.prefabSpriteDirection}{FoodDicctionary.toppingServedDirection}{FoodDicctionary.CookedMeal.ShapeOfCookedMeal((int)typeOfObjects[1])}";
            }
            else
            {
                directionOfSprite = $"{FoodDicctionary.prefabSpriteDirection}{FoodDicctionary.toppingDirection}";
            }
            Debug.Log($"direction of topping is {directionOfSprite}");
            toppingSpriteRenderer.sprite = LoadSprite.GetSpriteFromSpriteSheet(directionOfSprite, spriteName);
            hasATopping = true;
            SetImage();
            return true;
        }
        else
        {
            return false;
        }
    }

    public int? WhatKindOfTopping()
    {
        return typeOfObjects[2];
    }

    public bool HasATopping()
    {
        return typeOfObjects[2] != null;
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

    public void TransformIngridientToTopping()
    {
        string shape = FoodDicctionary.RawIngridients.ShapeOfRawIngridient((int)typeOfObjects[1]);
        hasARawIngridient = false;
        typeOfObjects[2] = typeOfObjects[1];
        typeOfObjects[1] = null;
        SetAToping((int)typeOfObjects[2], shape);
    }

    public void SetImage()
    {
        //Initialization();
        if (typeOfObjects[0] != null) containerSpriteRenderer.enabled = true; else containerSpriteRenderer.enabled = false;
        if (typeOfObjects[1] != null) foodSpriteRender.enabled = true; else foodSpriteRender.enabled = false;
        if (typeOfObjects[2] != null) toppingSpriteRenderer.enabled = true; else toppingSpriteRenderer.enabled = false;
    }

    public void SetSmoothieColor()
    {
        foodSpriteRender.color = FoodDicctionary.Toppings.ColorOfSmoothie((int)typeOfObjects[2]);
    }

    public void HideAllImages()
    {
        containerSpriteRenderer.enabled = false;
        foodSpriteRender.enabled = false;
        toppingSpriteRenderer.enabled = false;
    }

    public bool CanMergeTrays(Tray trayToMerge)
    {
        if (hasARawIngridient || trayToMerge.HasRawIngridient())
        {
            return false;
        }
        else
        {
            var typesOfTheOtherTray = trayToMerge.GetIdentifiers();
            var finalMergeData = new int?[typeOfObjects.Length];
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

            Debug.Log($"this is final merge data 0:{finalMergeData[0]} 1:{finalMergeData[1]} 2:{finalMergeData[2]}");
            return true;
        }
    }

    public void MergeTrays(Tray trayToMerge)
    {
        var typesOfTheOtherTray = trayToMerge.GetIdentifiers();
        Debug.Log($"types of this is final merge data 0:{typesOfTheOtherTray[0]} 1:{typesOfTheOtherTray[1]} 2:{typesOfTheOtherTray[2]}");
        var finalMergeData = new int?[typeOfObjects.Length];
        for (int i = 0; i < typeOfObjects.Length; i++)
        {
            if (typeOfObjects[i] != null)
            {
                finalMergeData[i] = typeOfObjects[i];
            }
            else
            {
                Debug.Log("in the debug we do now should only be call once");
                Debug.Log($"{i} in the debug we do now should call once minumun with value {typesOfTheOtherTray[i]}");
                finalMergeData[i] = typesOfTheOtherTray[i];
                if (typesOfTheOtherTray[i] != null)
                {
                    Debug.Log($"in the debug we do now should call once minumun with value {typesOfTheOtherTray[i]}");
                    finalMergeData[i] = typesOfTheOtherTray[i];
                }
            }
        }
        for (int i = 0; i < finalMergeData.Length;i++)
        {
            typeOfObjects[i] = finalMergeData[i];
            Debug.Log($"finalmergedate[{i}] is {finalMergeData[i]}");
        }
        Debug.Log($"merge data is 0: {typeOfObjects[0]} 1: {typeOfObjects[1]} 2: {typeOfObjects[2]}");
        Destroy(trayToMerge.gameObject);
        if (typeOfObjects[0] != null)
        {
            SetAContainer((int)typeOfObjects[0], FoodDicctionary.Containers.ShapeOfConatiner((int)typeOfObjects[0]));
        }
        if (typeOfObjects[1] != null)
        {
            SetACookedMeal((int)typeOfObjects[1], FoodDicctionary.CookedMeal.ShapeOfCookedMeal((int)typeOfObjects[1]));
        }
        if (typeOfObjects[2] != null)
        {
            SetAToping((int)typeOfObjects[2], FoodDicctionary.Toppings.ShapeOfTopping((int)typeOfObjects[2]));
        }
        SetImage();
    }

    public void SetLayers(int layer)
    {
        containerSpriteRenderer.sortingOrder = layer + 1;
        foodSpriteRender.sortingOrder = containerSpriteRenderer.sortingOrder + 1;
        toppingSpriteRenderer.sortingOrder = foodSpriteRender.sortingOrder + 1;
        //foreach (SpriteRenderer sp in transform.GetComponentsInChildren<SpriteRenderer>())
        //{
        //    sp.enabled = true;
        //}
    }

    public int?[] GetMadeComposition()
    {
        return typeOfObjects;
    }

    void DisableAllRenderers()
    {
        containerSpriteRenderer.enabled = false;
        foodSpriteRender.enabled = false;
        toppingSpriteRenderer.enabled = false;
    }
}
