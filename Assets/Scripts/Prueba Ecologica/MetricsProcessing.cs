using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MetricsProcessing : MonoBehaviour
{
	PEMainLogic main;
	public List<AgeData> tableCoins;
	public List<AgeData> waitingRoom;
	public List<AgeData> packNormal;
	public List<AgeData> unPack;
	public List<AgeData> packInverse;
	public List<AgeData> labPlanning;
	public List<AgeData> labTime;
	public List<AgeData> labErrors;
	public List<AgeData> flyLatency;
	public List<AgeData> flyCorrect;
	public float[] percentiles;
	public float[] scalarScore;
	public float[] values;

	public string[] paths;
	// Use this for initialization
	void Start ()
	{
		main = GameObject.Find("Main").transform.GetComponent<PEMainLogic>();
		tableCoins = TextProcessing(paths[0]);
		flyCorrect = TextProcessing(paths[1]);
		flyLatency = TextProcessing(paths[2]);
		labPlanning = TextProcessing(paths[3]);
		labErrors = TextProcessing(paths[4]);
		labTime = TextProcessing(paths[5]);
		packInverse = TextProcessing(paths[6]);
		packNormal = TextProcessing(paths[7]);
		unPack = TextProcessing(paths[8]);
		waitingRoom = TextProcessing(paths[9]);
		values = new float[10]{0,0,0,0,0,0,0,0,0,0};
		percentiles = new float[10]{0,0,0,0,0,0,0,0,0,0};
		scalarScore = new float[10]{0,0,0,0,0,0,0,0,0,0};


//		tableCoins = new AgeData[]{new AgeData(new Data[]{new Data(23, 24, 99, 19), new Data(23, 24, 99, 19)}),
//		new AgeData(new Data[]{new Data(21, 22, 99, 18), new Data(21, 22, 99, 18)})};
//
//
//
//
//
//		tableCoins=new AgeData[10];
//		tableCoins[0]=new AgeData(new Data[]{new Data(23, 24, 99, 19), new Data(23, 24, 99, 19)});
//		tableCoins[1]=new AgeData(new Data[]{new Data(21, 22, 99, 18), new Data(21, 22, 99, 18)});
//
//
//
//
//
//		tableCoins=new AgeData[10];
//		Data[]tempData=new Data[2];
//		tempData[0]=new Data(23, 24, 99, 19);
//		tempData[1]=new Data(23, 24, 99, 19);
//		tableCoins[0]=new AgeData(tempData);
//		tempData=new Data[2];
//		tempData[0]=new Data(21, 22, 99, 18);
//		tempData[1]=new Data(21, 22, 99, 18);
//		tableCoins[1]=new AgeData(tempData);

	}

	// Update is called once per frame
	void Update ()
	{
	 	
	}
	public void Process()
	{
		percentiles[0] = DataPercentProcessing(values[0], tableCoins);
		percentiles[1] = DataPercentProcessing(values[1], flyCorrect);
//		percentiles[2] = DataPercentProcessing(values[2], flyLatency);
		percentiles[3] = DataPercentProcessing(values[3], labPlanning);
		percentiles[4] = DataPercentProcessing(values[4], labErrors);
		percentiles[5] = DataPercentProcessing(values[5], labTime);
		percentiles[6] = DataPercentProcessing(values[6], packInverse);
		percentiles[7] = DataPercentProcessing(values[7], packNormal);
		percentiles[8] = DataPercentProcessing(values[8], unPack);
		percentiles[9] = DataPercentProcessing(values[9], waitingRoom);
		scalarScore[0] = DataPercentProcessing(values[0], tableCoins);
		scalarScore[1] = DataPercentProcessing(values[1], flyCorrect);
//		scalarScore[2] = DataPercentProcessing(values[2], flyLatency);
		scalarScore[3] = DataPercentProcessing(values[3], labPlanning);
		scalarScore[4] = DataPercentProcessing(values[4], labErrors);
		scalarScore[5] = DataPercentProcessing(values[5], labTime);
		scalarScore[6] = DataPercentProcessing(values[6], packInverse);
		scalarScore[7] = DataPercentProcessing(values[7], packNormal);
		scalarScore[8] = DataPercentProcessing(values[8], unPack);
		scalarScore[9] = DataPercentProcessing(values[9], waitingRoom);

	}
	public float DataPercentProcessing(float value, List<AgeData> list)
	{
		int age;
		age = main.ageOfPlayer;
		if(age <= 6)
		{
			age = 0;
		}
		else if(age > 6 && age < 12)
		{
			age = age - 6;
		}
		else if(age >= 12)
		{
			age = 6;
		}
			
		for(int i = 0; i < list.Count; i++)
		{
			if(value >= list[i].ages[age].minValue && value <= list[i].ages[age].maxValue)
			{
				Debug.Log(list[i].ages[age].percentile);
				return list[i].ages[age].percentile;
				break;
			}
		}
		return 0;
	}

	public float DataEscalarProcessing(float value, List<AgeData> list)
	{
		int age;
		age = main.ageOfPlayer;
		if(age <= 6)
		{
			age = 0;
		}
		else if(age > 6 && age < 12)
		{
			age = age - 6;
		}
		else if(age >= 12)
		{
			age = 6;
		}
		
		for(int i = 0; i < list.Count; i++)
		{
			if(value >= list[i].ages[age].minValue && value <= list[i].ages[age].maxValue)
			{
				Debug.Log(list[i].ages[age].scalarScore);
				return list[i].ages[age].scalarScore;
				break;
			}
		}
		return 0;
	}

	[System.Serializable]

	public class Data
	{

		public float minValue;
		public float maxValue;
		public float percentile;
		public float scalarScore;
		public float singleValue;

		public Data(float minVal, float maxVal, float percent, float scalar)
		{
			minValue = minVal;
			maxValue = maxVal;
			percentile = percent;
			scalarScore = scalar;

		}

		public Data(float singV, float percent, float scalar)
		{
			singleValue = singV;
			percentile = percent;
			scalarScore = scalar;
		}

	}

	[System.Serializable]

	public class AgeData
	{
		public List<Data> ages;
		public AgeData()
		{
			ages = new List<Data>();
		}
	}

	public List<AgeData> TextProcessing(string path)
	{

		TextAsset textObj = null;
		
		textObj = (TextAsset)Resources.Load(path, typeof(TextAsset));
		
		string[] coinsSelectiveAttentionLines = textObj.text.Split(new string[]{"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
		
		string[] coinsSelectiveAttentionValues;
		
		List<AgeData> container = new List<AgeData>();
		
		for(int i = 0; i < coinsSelectiveAttentionLines.Length; i++)
		{
			coinsSelectiveAttentionValues = coinsSelectiveAttentionLines[i].Split(new string[]{"/"}, StringSplitOptions.RemoveEmptyEntries);
			
			container.Add(new AgeData());
			
			for(int r = 0; r < coinsSelectiveAttentionValues.Length; r++)
			{
				if(coinsSelectiveAttentionValues[r].Contains("-"))
				{
					string[] temp = coinsSelectiveAttentionValues[r].Split(new string[]{"-", " "}, StringSplitOptions.RemoveEmptyEntries);
					
					float[] tempFloat = new float[temp.Length];
					for(int t = 0; t < temp.Length; t++)
					{
						float v;
						float.TryParse(temp[t], out v);
						tempFloat[t] = v;
					}
					container[container.Count - 1].ages.Add(new Data(tempFloat[0], tempFloat[1], tempFloat[2], tempFloat[3]));
				}
				else
				{
					string[] temp = coinsSelectiveAttentionValues[r].Split(new string[]{" "}, StringSplitOptions.RemoveEmptyEntries);
					float[] tempFloat = new float[temp.Length];
					for(int t = 0; t < temp.Length; t++)
					{
						float v;
						float.TryParse(temp[t], out v);
						tempFloat[t] = v;
					}
					container[container.Count - 1].ages.Add(new Data(tempFloat[0], tempFloat[1], tempFloat[2]));
				}
			}
		}
		return container;
	}
}


