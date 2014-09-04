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
	}
	
	// Update is called once per frame
	void Update () {
		RenderSettingsSetter rss;
		
		if ((!init) && (!Application.isLoadingLevel))
		{
			artisticSceneParent = GameObject.Find("ArtisticSceneParent");
			browserSceneParent = GameObject.Find("BrowserSceneParent");
			browserSceneParent.SetActive(false);
			rss = LoadLevels.artisticSceneParent.transform.Find("Scripts/BaseScripts").GetComponent<RenderSettingsSetter>();
			rss.set();
			init = true;
		}
		
	}
}
