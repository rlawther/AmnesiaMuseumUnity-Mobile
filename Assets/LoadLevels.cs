using UnityEngine;
using System.Collections;

public class LoadLevels : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		Debug.Log ("Loading Artistic");
		//Application.LoadLevelAdditive("ArtisticScene");
		//Debug.Log ("Loading Browser");
		Application.LoadLevelAdditive("BrowserScene");
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
