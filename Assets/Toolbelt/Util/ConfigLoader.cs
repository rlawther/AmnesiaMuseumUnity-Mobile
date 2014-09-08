using UnityEngine;
using System.Collections;
using System.IO;
using SimpleJSON;

namespace Toolbelt {
public class ConfigLoader : MonoBehaviour {
	private string result = null;
	public bool configLoaded = false;
	public string fpath = null;
	public JSONNode jsonConfig = null;
	public bool showoutput = false;
	// Use this for initialization
	void Start () {
		StartCoroutine("WaitForDownload");
	}
	
	// Update is called once per frame
	void Update () {
		if (this.result != null && !this.configLoaded) {
			this.LoadJSON(this.result);
			configLoaded = true;
		}
	}
	
	private void LoadJSON(string s) {
		try {
			JSONNode n = SimpleJSON.JSON.Parse(s);
			this.jsonConfig = n;
		} catch (JSONException e) {
			Debug.Log("No config loaded..." + e.ToString());
		}		
	}
	
	
	IEnumerator WaitForDownload() {		
		string filePath = Application.dataPath + "/StreamingAssets/config.txt";
		
		if (!filePath.Contains("://")) {
			filePath = "file:///" + filePath;			
		}
			fpath = filePath;
            WWW www = new WWW(filePath);
            yield return www;
            this.result = www.text;
        
	}
	void OnGUI() {
//		if (this.result != null) {
//			GUI.TextArea (new Rect (0, 0, 300,400), this.result);
//		}
//		if (this.fpath!= null)			
//			GUI.TextArea (new Rect (300, 0, 300,400), this.fpath);
		if (this.jsonConfig != null && this.showoutput)			
			GUI.TextArea (new Rect (600, 0, 300,400), this.jsonConfig.ToString());
	}
}
}