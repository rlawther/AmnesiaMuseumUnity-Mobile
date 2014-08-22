using UnityEngine;
using System.Collections;

public class LoadLevels : MonoBehaviour {

	public static GameObject artisticSceneParent = null;
	public static GameObject browserSceneParent;
	
	private bool init = false;
	
	// Use this for initialization
	void Start () {
	
		Debug.Log ("Loading Artistic");
		Application.LoadLevelAdditive("ArtisticScene");
	
		Debug.Log ("Loading Browser");
		Application.LoadLevelAdditive("BrowserScene");
		
		/*
		while (Application.isLoadingLevel)
		{
		}
		*/
		
		Debug.Log ("Done loading");
		
	}
	
	// Update is called once per frame
	void Update () {
		if ((!init) && (!Application.isLoadingLevel))
		{
			GameObject go;
			
			Debug.Log (artisticSceneParent);
			//artisticSceneParent = GameObject.Find("ArtisticSceneParent");
			artisticSceneParent = GameObject.Find("ArtisticSceneParent");
			browserSceneParent = GameObject.Find("BrowserSceneParent");
			Debug.Log (artisticSceneParent);
			init = true;
		}
		
	}
}
