using UnityEngine;
using System.Collections;

public class Events
 : MonoBehaviour {
 
 	private bool mArtisticMode = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKey("o"))
		{
			if (!mArtisticMode)
			{
				Debug.Log ("Into artisitc");
				mArtisticMode = true;
				LoadLevels.artisticSceneParent.SetActive(true);
				LoadLevels.browserSceneParent.SetActive(false);
			}
		}
		else if (Input.GetKey("p"))
		{
			if (mArtisticMode)
			{
				Debug.Log ("Into browser" + LoadLevels.artisticSceneParent);
				mArtisticMode = false;
				
				LoadLevels.artisticSceneParent.SetActive(false);
				LoadLevels.browserSceneParent.SetActive(true);
			}
		}
	}
}
