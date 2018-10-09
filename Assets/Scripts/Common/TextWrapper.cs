// Word Wrap v1.0 for Unity 3D Text Objects

// 

// Author: Michael Colin Voigt

// [url]http://3037.com[/url]

// Free for all purposes, if you use it, just email me at [email]info@3037.com[/email]

using UnityEngine;
using System.Collections;

public class TextWrapper : MonoBehaviour {

	public GameObject textObject;
	public int wrapPoint = 0;
	public string currentText;
	public bool hyphen = true;

	public void Wrap (string name) 
	{
		currentText=name;
		currentText = " " + currentText;
		string finalString ="";
		int currentWrap = wrapPoint;
		int count = 0;
		int startPoint = 0;
		int lastBlankSpace = 0;
		int currentLength = 0;
		string appendText = "";
		
		while( count <= currentText.Length  )	
		{
			count++;
			if( count >= currentText.Length)
			{
				int finalLength = currentText.Length - startPoint;	
				finalString +=  currentText.Substring( startPoint+1 , finalLength-1 );
				break;
			}
			if( currentText[count] == ' ' )	
			{
				lastBlankSpace = count;	
			}
			if( currentText[count] == ' ' && count >= currentWrap )	
			{
				currentLength = count - startPoint;
				appendText = currentText.Substring( startPoint+1 , currentLength ) + "\n" ;
				finalString += appendText;
				currentWrap = count + wrapPoint;
				startPoint = count;
			}
			if( currentText[count] != ' ' && count >= currentWrap )	
			{
				for(var i = count; i > startPoint;i--)		
				{
					if( currentText[i] == ' ' ) 		
					{
						Debug.Log(currentText[i]);	
						currentLength = i - startPoint;
						appendText = currentText.Substring( startPoint+1 , currentLength ) + "\n" ;
						finalString += appendText;
						currentWrap = i + wrapPoint;
						startPoint = i;
					}
				}
			}
			if(hyphen)	
			{
				int worldLength = (count - lastBlankSpace);	
				if(  worldLength  >    wrapPoint )		
				{
					currentLength = count - startPoint;	
					appendText = currentText.Substring( startPoint+1 , currentLength ) + "-\n" ;
					finalString += appendText;
					currentWrap = count + wrapPoint;
					startPoint = count; 
					lastBlankSpace = count; 
				}
			}
		}
		textObject.GetComponent <TextMesh>().text = finalString;
	}
}