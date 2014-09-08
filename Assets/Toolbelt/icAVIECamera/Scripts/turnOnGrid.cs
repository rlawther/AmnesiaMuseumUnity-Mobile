using UnityEngine;
using System.Collections;

public class turnOnGrid : MonoBehaviour {
	InitialisationScript myIs;
	bool done = false;
	// Use this for initialization
	void Start () {
		myIs = this.GetComponent<InitialisationScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!done && myIs.isInitialized) {
			done = true;
			myIs.setGrid(true);
		}
	}
}
