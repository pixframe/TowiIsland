using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackInstruccions : MonoBehaviour {

    //This are the numbers needed to play the tutorial
    /* this is the object relationship
        0"Aletas",
        1"Cámara",
        2"Carrito",
        3"Cerdito",
        4"Cinturon",
        5"Cuaderno",
        6"Globos",
        7"Guantes",
        8"Láiz",
        9"Lentes",
        10"Linterna",
        11"Pantalon",
        12"Patineta",
        13"Piano",
        14"Pino de boliche",
        15"Raqueta",
        16"Robot",
        17"Sombrero",
        18"Sombrilla",
        19"Tabla de Surf",
        20"Tambor",
        21"Tiro al blanco",
        22"Zapato" */
    
    public static List<int> tutorialNeeds = new List<int>{
        13,
        19,
        9
    };

    public static List<int> tutorialBackNeeds = new List<int>{
        4,
        12
    };

	public static List<int> rememberUnpack = new List<int>
	{
        19,
        2,
        16
	};

    public static List<int> GoingFowardLevels(int level, int numberoftry){
        switch(level){
            case 0:
                switch(numberoftry){
                    case 0:
                        return new List<int>
                        {
                            3,
                            13,
                            2
                        };
                    default:
                        return new List<int>
                        {
                            17,
                            14,
                            19
                        };
                }
            case 1:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
                            2,
                            14,
                            15,
                            19
                        };
                    default:
                        return new List<int>
                        {
                            15,
                            3,
                            13,
                            20
                        };
                }
            case 2:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
                            15,
                            13,
                            20,
                            14,
                            3
                        };
                    default:
                        return new List<int>
                        {
                            17,
                            11,
                            20,
                            16,
                            19
                        };
                }
            case 3:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
                            20,
                            3,
                            2,
                            17,
                            11,
                            15
                        };
                    default:
                        return new List<int>
                        {
                            16,
                            19,
                            20,
                            14,
                            2,
                            11
                        };
                }
            case 4:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
                            13,
                            3,
                            2,
                            15,
                            11,
                            16,
                            19
                        };
                    default:
                        return new List<int>
                        {
                            14,
                            17,
                            15,
                            3,
                            11,
                            13,
                            2
                        };
                }
            case 5:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
                            11,
                            20,
                            16,
                            19,
                            15,
                            2,
                            17,
                            13
                        };
                    default:
                        return new List<int>
                        {
                            3,
                            14,
                            16,
                            11,
                            15,
                            17,
                            19,
                            13
                        };
                }
            case 6:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
                            19,
                            13,
                            2,
                            15,
                            11,
                            16,
                            20,
                            17,
                            14
                        };
                    default:
                        return new List<int>
                        {
                            3,
                            13,
                            2,
                            15,
                            11,
                            16,
                            20,
                            19,
                            14
                        };
                }
            default:
                return null;
        }
    }

	public static List<int> GoingBackLevels(int level, int numberoftry)
    {
        switch (level)
        {
            case 0:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
							0,
                            6
                        };
                    default:
                        return new List<int>
                        {
							1,
							8
                        };
                }
            case 1:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
							4,
                            12,
                            22
                        };
                    default:
                        return new List<int>
                        {
							4,
                            1,
                            0
                        };
                }
            case 2:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
							12,
                            8,
                            4,
                            5
                        };
                    default:
                        return new List<int>
                        {
							0,
                            5,
                            1,
                            8
                        };
                }
            case 3:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
                            10,
                            1,
                            8,
                            12,
                            22
                        };
                    default:
                        return new List<int>
                        {
							0,
                            6,
                            4,
                            5,
                            22
                        };
                }
            case 4:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
                            12,
                            6,
                            4,
                            8,
                            1,
                            10
                        };
                    default:
                        return new List<int>
                        {
                            1,
                            6,
                            8,
                            4,
                            22,
                            10
                        };
                }
            case 5:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
							0,
                            6,
                            8,
                            5,
                            22,
                            10,
                            1
                        };
                    default:
                        return new List<int>
                        {
							0,
                            5,
                            4,
                            8,
                            22,
                            12,
                            1
                        };
                }
            case 6:
                switch (numberoftry)
                {
                    case 0:
                        return new List<int>
                        {
                            0,
                            6,
                            8,
                            5,
                            22,
                            10,
                            1,
                            21
                        };
                    default:
                        return new List<int>
                        {
                            0,
                            6,
                            4,
                            5,
                            22,
                            10,
                            1,
                            21
                        };
                }
            default:
                return null;
        }
    }
}
