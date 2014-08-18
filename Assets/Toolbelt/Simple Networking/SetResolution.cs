using UnityEngine;
using System.Collections;
namespace Toolbelt {
public class SetResolution : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		Screen.SetResolution(1920,1920,false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
}