using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlashDesync : MonoBehaviour {
	public Color flashColor;
	private bool initialized;
	List<Camera> cameras;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (initialized) {
#if NO_CLUSTER

#else
			Color c = flashColor;
			if (!ClusterNetwork.IsDisconnected()) {
				c = Color.black;
			}
			foreach (Camera cam in this.cameras) {
				cam.backgroundColor = c;
			}
#endif
		} else if (this.GetComponent<InitialisationScript>().isInitialized) {

			this.initialized = true;
			this.cameras = new List<Camera>();

			foreach (Camera cam in this.GetComponentsInChildren<Camera>()) {
				if (cam.gameObject.name.Contains ("Slave Camera")) {
					cameras.Add (cam);
				}
			}
		}
	}
}
