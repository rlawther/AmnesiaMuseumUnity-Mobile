using UnityEngine;
using System.Collections;

public class GUITouch : MonoBehaviour {

	public GUIScript gs;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// get the main camera's GUILayer:
		GUILayer gLayer = Camera.main.GetComponent< GUILayer>();
		for (int i = 0; i < Input.touchCount; ++i)
		{
			if (Input.GetTouch(i).phase.Equals(TouchPhase.Began))
			{
				Debug.Log ("touch!");
				// Get the GUIElement touched, if any:
				GUIElement button = gLayer.HitTest(Input.GetTouch(i).position);
				if (button) // if any GUIElement touched...
				{
					// call OnMouseDown in its scripts:
					//button.SendMessage("OnMouseDown");
					//gs.buttonPressed(button);
					//Debug.Log ("touch button!");

				}
			}
		}
	
	}
}
