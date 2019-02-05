using UnityEngine;
using System.Collections;

public class FoodDicctionary
{
    public static string prefabSpriteDirection = "IcecreamMadness/Sprites/";
    public static string logoDirection = "Logos/Logos";
    public static string ingredientDirection = "Ingredients/Ingredients";
    public static string containersDirection = "Containers/Containers";
    public static string cookedMealDirection = "Cooked/Cooked";
    public static string toppingDirection = "Toppings/Toppings";

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
        public enum KindOfRawIngridient { Ice, Eggs, Milk, Flour, Pineapple, Kiwi, Coconut, Banana, Strawberry }

        public static string ShapeOfRawIngridient(int kindOfRawIngridient)
        {
            switch ((KindOfRawIngridient)kindOfRawIngridient)
            {
                case KindOfRawIngridient.Ice:
                    return "Hexa";
                case KindOfRawIngridient.Eggs:
                    return "Hexa";
                case KindOfRawIngridient.Milk:
                    return "Hexa";
                case KindOfRawIngridient.Flour:
                    return "Hexa";
                case KindOfRawIngridient.Pineapple:
                    return "Pineapple";
                case KindOfRawIngridient.Kiwi:
                    return "Kiwi";
                case KindOfRawIngridient.Coconut:
                    return "Coconut";
                case KindOfRawIngridient.Banana:
                    return "Banana";
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
                return "Ingredients";
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
                case KindOfRawIngridient.Coconut:
                    ColorUtility.TryParseHtmlString("#A100D3", out colorToPut);
                    break;
                case KindOfRawIngridient.Banana:
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
                case RawIngridients.KindOfRawIngridient.Coconut:
                    return "Coconut";
                case RawIngridients.KindOfRawIngridient.Banana:
                    return "Banana";
                case RawIngridients.KindOfRawIngridient.Strawberry:
                    return "Strawberry";
                default:
                    return "Triangle";
            }
        }
    }
}
