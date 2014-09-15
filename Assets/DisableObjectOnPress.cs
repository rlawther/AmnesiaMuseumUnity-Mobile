using UnityEngine;
using System.Collections;

public class DisableObjectOnPress : MonoBehaviour {

	public GameObject toDisable;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMouseDown()
	{
		toDisable.SetActive (false);
	}
}
