using UnityEngine;
using System.Collections;

public class Events
 : MonoBehaviour {
 
 	private bool mArtisticMode = true;
 	private GameObject mActiveNarrativeScenario = null;

	// Use this for initialization
	void Start () {
	
	}
	
	/* sets the child at the given index to active, all other children
	 * to inactive.
	 * Returns the now active child
	 */
	GameObject setOnlyActiveChild(GameObject parent, int childIndex)
	{
		GameObject activeChild = null;
		int i = 1;
		foreach(Transform child in parent.transform)
		{
			Debug.Log (child.name);
			if (i == childIndex)
			{
				child.gameObject.SetActive(true);
				activeChild = child.gameObject;
			} 
			else
			{
				child.gameObject.SetActive(false);
			}
			
			i++;
		}
		return activeChild;
	}
	
	/*
	 * Sets all children of the parent to active
	 */
	void setAllActiveChildren(GameObject parent)
	{
		foreach(Transform child in parent.transform)
			child.gameObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
	
		RenderSettingsSetter rss;
	
		if (Input.GetKey("o"))
		{
			if (!mArtisticMode)
			{
				Debug.Log ("Into artisitc");
				mArtisticMode = true;
				LoadLevels.artisticSceneParent.SetActive(true);
				LoadLevels.browserSceneParent.SetActive(false);
				rss = LoadLevels.artisticSceneParent.transform.Find("Scripts/BaseScripts").GetComponent<RenderSettingsSetter>();
				rss.set();
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
				rss = LoadLevels.browserSceneParent.transform.Find("Scripts/BaseScripts").GetComponent<RenderSettingsSetter>();
				rss.set();
			}
		}
		
		if (Input.GetKey ("1"))
		{
			Debug.Log ("group 1");
			GameObject photoParent = GameObject.Find ("Photos");
			mActiveNarrativeScenario = setOnlyActiveChild(photoParent, 1);
		}
		else if (Input.GetKey ("2"))
		{
			Debug.Log ("group 2");
			GameObject photoParent = GameObject.Find ("Photos");
			mActiveNarrativeScenario = setOnlyActiveChild(photoParent, 2);
		}
		
		if (mActiveNarrativeScenario != null)
		{
			if (Input.GetKey(KeyCode.Keypad0))
				setAllActiveChildren(mActiveNarrativeScenario);
			else if (Input.GetKey(KeyCode.Keypad1))
				setOnlyActiveChild(mActiveNarrativeScenario, 1);
			else if (Input.GetKey(KeyCode.Keypad2))
				setOnlyActiveChild(mActiveNarrativeScenario, 2);
			else if (Input.GetKey(KeyCode.Keypad3))
				setOnlyActiveChild(mActiveNarrativeScenario, 3);
			else if (Input.GetKey(KeyCode.Keypad4))
				setOnlyActiveChild(mActiveNarrativeScenario, 4);
		}
				
	}
}
