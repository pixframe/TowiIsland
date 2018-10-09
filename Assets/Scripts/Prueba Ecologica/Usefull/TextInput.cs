using UnityEngine;
using System.Collections;

public class TextInput : MonoBehaviour
{
	int i;
	string final;
	
	public string InputText(string text, bool numbers, bool letters, int maxChars)
	{
		if(numbers && !letters)
		{
			text = "";
			foreach(char c in Input.inputString)
			{
				string x = c.ToString();
				if (x == "\b")//chacks for backspace
				{
					if (text.Length != 0)
					{
						text = text.Substring(0, text.Length - 1);
						
					}  	
				}    
	            else
				{
					if (x == "\n" || x == "\r")//checks for enter or intro
					{
						if(text == "")
						{
							return "0";
						}
						else
						{
							final = text;
							return final;// returns a string that can be converted into an int 	
						}
						
					}
	                else
					{
						if(text.Length < maxChars && int.TryParse(Input.inputString, out i))//checks for the max amount of characters and if they are numbers
						{

							text.Insert(text.Length, x);	//prints the numbers to the screen
						
						}
						
					}   
				}     
			}	
		}
		else if(!numbers && letters)
		{
			foreach(char c in Input.inputString)
			{
				if (c == "\b"[0])//chacks for backspace
				{
					if (text.Length != 0)
					{
						text = text.Substring(0, text.Length - 1);
					}
					
	                   	
				}    
	            else
				{
					if (c == "\n"[0] || c == "\r"[0])//checks for enter or intro
					{
						final = text;
						return final;// returns a string with out numbers
					}
	                else
					{
						if(text.Length < maxChars && !int.TryParse(Input.inputString, out i))//checks for the max amount of characters and if they are numbers
						{
							text += c;	//prints the numbers to the screen
						}	
						
					}   
				}     
			}
		}
		else if(numbers && letters)
		{
			foreach(char c in Input.inputString)
			{
				if (c == "\b"[0])//chacks for backspace
				{
					if (text.Length != 0)
					{
						text = text.Substring(0, text.Length - 1);
					}
					
					
	                   	
				}    
	            else
				{
					if (c == "\n"[0] || c == "\r"[0])//checks for enter or intro
					{
						final = text;
						return final;//returns a string with numbers
					}
	                else
					{
						if(text.Length < maxChars)//checks for the max amount of characters and if they are numbers
						{
							text += c;	//prints the numbers to the screen
						}
						
					}   
				}     
			}
		}
		
		return "0";
		
	}
}
