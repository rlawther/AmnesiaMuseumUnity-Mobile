using UnityEngine;
using System.Collections;
using icExtensionMethods;

public class FaceObject : MonoBehaviour {
	public Transform toFace;
	public Vector3 toFacePosition; //will face position if toFace == null;
	
	public void ManualUpdate() {
		if (toFace != null) {
			this.transform.LookAt(toFace);
		} else {
			this.transform.LookAt (toFacePosition);
		}
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		this.ManualUpdate();
	}
}
