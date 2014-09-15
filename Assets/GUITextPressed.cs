
using UnityEngine;
using System.Collections;

public class GUITextPressed : MonoBehaviour {

	public GameObject GUIMenu;
	private GUIScript gs;
	private GUIText gt;


	// Use this for initialization
	void Start () {
	
		gs = GUIMenu.GetComponent<GUIScript>();
		gt = this.GetComponent<GUIText>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMouseDown() 
	{
		Debug.Log ("mouse down " + this.name);
		gs.buttonPressed (gt);
	}
}
