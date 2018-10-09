using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;

public class TrialManager : MonoBehaviour {
	DateTime lastDay;
	DateTime today;
	public int remainingDays=-1;
	[HideInInspector]
	public TrialRegisterController controller;
	public Login loginRef;
	// Use this for initialization
	void Awake () {
		loginRef = GetComponent<Login> ();
		controller = GetComponent<TrialRegisterController> ();
		//PlayerPrefs.SetInt("TrialRemaining",-1);
		//PlayerPrefs.SetString("TrialDay","");
		today = DateTime.Now;
	}
	void Start()
	{
		if(LoadDate())
		{
			remainingDays=PlayerPrefs.GetInt("TrialRemaining",0);
			TimeSpan span = today.Subtract(lastDay);
			object objDate=LoadStartDate();
			DateTime startDate;
			if(objDate==null)
				startDate=today;
			else
				startDate=(DateTime)objDate;
			controller.GetTrial(span,remainingDays,startDate,today);
			//			if(span.Days>0)
			//			{	
			//				remainingDays-=span.Days;
			//				remainingDays = Mathf.Max(0,remainingDays);
			//				PlayerPrefs.SetInt("TrialRemaining",remainingDays);
			//			}
			//loginRef.remainingDays=remainingDays;
			SaveDate();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetRemaining(int remDays)
	{
		remainingDays = remDays;
	}
	
	public int GetRemaining()
	{
		return remainingDays;
	}

	public void ActivateTrial()
	{
		if(remainingDays==-1)
		{
			controller.StartTrial();
			today = DateTime.Now;
			SaveDate();
			SaveStartDate();
			remainingDays=7;
			PlayerPrefs.SetInt("TrialRemaining",remainingDays);
		}
	}

	public void SaveDate()
	{
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

        /*
        XmlSerializer serializer = new XmlSerializer(typeof(DateTime));
        MemoryStream stream = new MemoryStream();
        serializer.Serialize(stream, today);
        PlayerPrefs.SetString("TrialDay", Convert.ToBase64String(stream.GetBuffer()));

        */
		BinaryFormatter b = new BinaryFormatter();
		//Create an in memory stream
		MemoryStream m = new MemoryStream();
		//Save the scores
		b.Serialize(m, today);
		//Add it to player prefs
		PlayerPrefs.SetString("TrialDay", Convert.ToBase64String(m.GetBuffer()));
	}

	public void SaveStartDate()
	{
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

        /*
        XmlSerializer serializer = new XmlSerializer(typeof(DateTime));
        MemoryStream stream = new MemoryStream();
        serializer.Serialize(stream, today);
        PlayerPrefs.SetString("StartTrialDay", Convert.ToBase64String(stream.GetBuffer()));

        */
		BinaryFormatter b = new BinaryFormatter();
		//Create an in memory stream
		MemoryStream m = new MemoryStream();
		//Save the scores
		b.Serialize(m, today);
		//Add it to player prefs
		PlayerPrefs.SetString("StartTrialDay", Convert.ToBase64String(m.GetBuffer()));
	}
	
	bool LoadDate()
	{
		//Get the data
		string data = PlayerPrefs.GetString("TrialDay","");
		//If not blank then load it
		if(!string.IsNullOrEmpty(data))
		{
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
            /*
            XmlSerializer serializer = new XmlSerializer(typeof(DateTime));
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(data));
            lastDay = (DateTime)serializer.Deserialize(stream);
            */
			//Binary formatter for loading back
			BinaryFormatter b = new BinaryFormatter();
			//Create a memory stream with the data
			MemoryStream m = new MemoryStream(Convert.FromBase64String(data));
			//Load back the scoress
			lastDay = (DateTime)b.Deserialize(m);
			return true;
		}
		return false;
	}

	object LoadStartDate()
	{
		//Get the data
		string data = PlayerPrefs.GetString("StartTrialDay","");
		//If not blank then load it
		if(!string.IsNullOrEmpty(data))
		{
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

            /*
            XmlSerializer serializer = new XmlSerializer(typeof(DateTime));
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(data));
            return (DateTime)serializer.Deserialize(stream);

            */
			//Binary formatter for loading back
			BinaryFormatter b = new BinaryFormatter();
			//Create a memory stream with the data
			MemoryStream m = new MemoryStream(Convert.FromBase64String(data));
			//Load back the scoress
			return((DateTime)b.Deserialize(m));
		}
		return null;
	}

	public void registerParentAndKid(string name, string lastName, string email, string password,string kidName, string kidLastName)
	{
		controller.RegisterParentAndKid (name, lastName, email, password,kidName,kidLastName);
	}
}
