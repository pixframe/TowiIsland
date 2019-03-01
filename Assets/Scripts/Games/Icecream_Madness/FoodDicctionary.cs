using UnityEngine;
using System.Collections;

public class FoodDicctionary
{
    public const float icecremPreparationTime = 50f;
    public const float smoothiePreparationTime = 60f;
    public const float wafflePreparationTime = 70f;

    public static string prefabSpriteDirection = "IcecreamMadness/Sprites/";
    public static string prefabGameObjectDirection = "IcecreamMadness/Prefabs/";
    public static string machinesDirection = "Machines/";
    public static string logoDirection = "Logos/Logos";
    public static string ingredientDirection = "Ingredients/Ingredients";
    public static string containersDirection = "Containers/Containers";
    public static string cookedMealDirection = "Cooked/Cooked";
    public static string toppingDirection = "Toppings/Toppings";
    public const string toppingServedDirection = "ToppingsServed/";
    public const string helperToppingDirection = "Helpers/Toppings";

    public const string normalTable = "Normal";
    public const string finishTable = "Talavera";
    public const string trashTable = "Trash";


    public const string icecreamMachine = "IcecreamMachine";
    public const string blenderMachine = "Blender";
    public const string trashMachine = "Cocodrile";
    public const string chopperMachine = "Chopper";

    public class Containers
    {
        public enum KindOfContainer { Waffel, Glass, Dish };

        public static string ShapeOfConatiner(int kindOfContainer)
        {
            switch ((KindOfContainer)kindOfContainer)
            {
                case KindOfContainer.Waffel:
                    return "WaffleCone";
                case KindOfContainer.Glass:
                    return "Glass";
                case KindOfContainer.Dish:
                    return "Dish";
                default:
                    return "Cube";
            }
        }

        public static Color ColorOfContainer(int kindOfContainer)
        {
            Color colorToPut;
            switch ((KindOfContainer)kindOfContainer)
            {
                case KindOfContainer.Waffel:
                    ColorUtility.TryParseHtmlString("#E2B232", out colorToPut);
                    break;
                case KindOfContainer.Glass:
                    ColorUtility.TryParseHtmlString("#7BFFF2", out colorToPut);
                    break;
                case KindOfContainer.Dish:
                    colorToPut = Color.white;
                    break;
                default:
                    colorToPut = Color.black;
                    break;
            }

            return colorToPut;
        }


        public static string ContainerTable = "Container";
    }

    public class RawIngridients
    {
        public enum KindOfRawIngridient { Ice, Eggs, Milk, Flour, Pineapple, Kiwi, Orange, Fig, Strawberry }

        public static string ShapeOfRawIngridient(int kindOfRawIngridient)
        {
            switch ((KindOfRawIngridient)kindOfRawIngridient)
            {
                case KindOfRawIngridient.Ice:
                    return "Ice";
                case KindOfRawIngridient.Eggs:
                    return "Eggs";
                case KindOfRawIngridient.Milk:
                    return "Milk";
                case KindOfRawIngridient.Flour:
                    return "Flour";
                case KindOfRawIngridient.Pineapple:
                    return "Pineapple";
                case KindOfRawIngridient.Kiwi:
                    return "Kiwi";
                case KindOfRawIngridient.Orange:
                    return "Orange";
                case KindOfRawIngridient.Fig:
                    return "Fig";
                case KindOfRawIngridient.Strawberry:
                    return "Strawberry";
                default:
                    return "Hexa";
            }
        }

        public static string ShapeOfContainerTable(int kindOfRawIngridient)
        {

            if (kindOfRawIngridient < 4)
            {
                return "Ingredient";
            }
            else
            {
                return "Topping";
            }
        }

        public static Color ColorOfRawIngridient(int kindOfRawIngridient)
        {
            Color colorToPut;

            switch ((KindOfRawIngridient)kindOfRawIngridient)
            {
                case KindOfRawIngridient.Ice:
                    colorToPut = Color.cyan;
                    break;
                case KindOfRawIngridient.Eggs:
                    ColorUtility.TryParseHtmlString("#B17F3E", out colorToPut);
                    break;
                case KindOfRawIngridient.Milk:
                    colorToPut = Color.white;
                    break;
                case KindOfRawIngridient.Flour:
                    ColorUtility.TryParseHtmlString("#CEE389", out colorToPut);
                    break;
                case KindOfRawIngridient.Pineapple:
                    colorToPut = Color.red;
                    break;
                case KindOfRawIngridient.Kiwi:
                    colorToPut = Color.green;
                    break;
                case KindOfRawIngridient.Orange:
                    ColorUtility.TryParseHtmlString("#A100D3", out colorToPut);
                    break;
                case KindOfRawIngridient.Fig:
                    colorToPut = Color.yellow;
                    break;
                case KindOfRawIngridient.Strawberry:
                    ColorUtility.TryParseHtmlString("#FF00C1", out colorToPut);
                    break;
                default:
                    colorToPut = Color.black;
                    break;
            }
            return colorToPut;
        }
    }

    public class MadeIngridients
    {
        public enum KindOfMadeIngridients { Dough, FixableDough, Trashable};
    }

    public class CookedMeal
    {
        public enum KindOfCookedMeal { Icecream, Smoothie, Waffle};

        static public string ShapeOfCookedMeal(int kindOfCookedMeal)
        {
            switch ((KindOfCookedMeal)kindOfCookedMeal)
            {
                case KindOfCookedMeal.Icecream:
                    return "Icecream";
                case KindOfCookedMeal.Smoothie:
                    return "Smoothie";
                case KindOfCookedMeal.Waffle:
                    return "Waffle";
                default:
                    return "Circle";
            }
        }

        public static Color ColorOfCookedMeal(int kindOfCookedMeal)
        {
            Color colorToPut;

            switch ((KindOfCookedMeal)kindOfCookedMeal)
            {
                case KindOfCookedMeal.Icecream:
                    colorToPut = Color.white;
                    break;
                case KindOfCookedMeal.Smoothie:
                    colorToPut = Color.cyan;
                    break;
                case KindOfCookedMeal.Waffle:
                    ColorUtility.TryParseHtmlString("#996633", out colorToPut);
                    break;
                default:
                    colorToPut = Color.black;
                    break;
            }

            return colorToPut;
        }
    }

    public class Toppings
    {
        static public string ShapeOfTopping(int kindOfTopping)
        {
            switch ((RawIngridients.KindOfRawIngridient)kindOfTopping)
            {
                case RawIngridients.KindOfRawIngridient.Pineapple:
                    return "Pineapple";
                case RawIngridients.KindOfRawIngridient.Kiwi:
                    return "Kiwi";
                case RawIngridients.KindOfRawIngridient.Orange:
                    return "Orange";
                case RawIngridients.KindOfRawIngridient.Fig:
                    return "Fig";
                case RawIngridients.KindOfRawIngridient.Strawberry:
                    return "Strawberry";
                default:
                    return "Triangle";
            }
        }

        static public string AnimationOfChopper(int kindOfTopping)
        {
            switch ((RawIngridients.KindOfRawIngridient)kindOfTopping)
            {
                case RawIngridients.KindOfRawIngridient.Pineapple:
                    return "Piña";
                case RawIngridients.KindOfRawIngridient.Kiwi:
                    return "Kiwi";
                case RawIngridients.KindOfRawIngridient.Orange:
                    return "Naranja";
                case RawIngridients.KindOfRawIngridient.Fig:
                    return "Higo";
                case RawIngridients.KindOfRawIngridient.Strawberry:
                    return "Fresa";
                default:
                    return "Triangle";
            }
        }

        static public Color ColorOfSmoothie(int kindOfTopping)
        {
            string colorString = "";
            switch ((RawIngridients.KindOfRawIngridient)kindOfTopping)
            {
                case RawIngridients.KindOfRawIngridient.Pineapple:
                    colorString = "FBCF3B";
                    break;
                case RawIngridients.KindOfRawIngridient.Kiwi:
                    colorString = "C0E654";
                    break;
                case RawIngridients.KindOfRawIngridient.Orange:
                    colorString = "F9A94B";
                    break;
                case RawIngridients.KindOfRawIngridient.Fig:
                    colorString = "A59CBB";
                    break;
                case RawIngridients.KindOfRawIngridient.Strawberry:
                    colorString = "F28B7A";
                    break;
                default:
                    colorString = "FBCF3B";
                    break;
            }

            Color returner;
            ColorUtility.TryParseHtmlString($"#{colorString}", out returner);
            return returner;
        }
    }
}
