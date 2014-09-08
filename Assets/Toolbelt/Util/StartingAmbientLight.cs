using UnityEngine;
using System.Collections;
namespace Toolbelt {
public class StartingAmbientLight : MonoBehaviour {
	public Color startingColor;
	// Use this for initialization
	void Start () {
		RenderSettings.ambientLight = this.startingColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
}