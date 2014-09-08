using UnityEngine;
using System.Collections;

public class ActivateFPC : MonoBehaviour {

	public GameObject fpc;
	public GameObject baseScripts;
	public GameObject AVIEPrefab;
	public GameObject mainCamera;
	public bool useAVIECamera;
	
	// Use this for initialization
	void Start () {
	
		/* If this scene is running standalone (ie. hasn't been loaded at
		 * runtime from the parent) then we want to activate the First
		 * Person Controller. Because GameObject.Find cannot find unactive
		 * objects, this will only return an object if there is already
		 * an *active* first person controller. If not, we activate
		 * this one.
		 */
		if (GameObject.Find ("First Person Controller") == null)
			fpc.SetActive(true);
		if (GameObject.Find ("BaseScripts") == null)
			baseScripts.SetActive(true);

		if (useAVIECamera) {
			if (GameObject.Find ("AVIE Prefab") == null)
				AVIEPrefab.SetActive (true);
		}
		else {
			if (GameObject.Find ("Main Camera") == null)
				mainCamera.SetActive (true);
		}

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
