using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;
namespace Toolbelt
{

public class ToolbeltJSONParser {
	public static JSONNode parseFile(string filePath) {
		string text = string.Empty;		
		//Debug.Log (filePath);
		using (StreamReader streamReader = new StreamReader(filePath))
		{            
			text = streamReader.ReadToEnd();
		}
		return JSON.Parse(text);
	}
}
}