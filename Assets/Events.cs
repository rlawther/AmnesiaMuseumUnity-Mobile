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
	
		if (Input.GetKey("9"))
		{
			if (!mArtisticMode)
			{
				Debug.Log ("Into artisitc");
				mArtisticMode = true;
				Application.LoadLevel(0);
			}
		}
		else if (Input.GetKey("0"))
		{
			if (mArtisticMode)
			{
				Debug.Log ("Into browser");
				mArtisticMode = false;
				Application.LoadLevel(1);
			}
		}
	}
}
