using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class StartColour : MonoBehaviour {
	
	public Color color;
	public Color hue;
	public bool startColour = true;
	public bool startHue = true;
	// Use this for initialization
	void Awake () {
		if (startColour)
			this.GetComponent<icMaterial>().icColour = this.color;
		if (startHue)
			this.renderer.material.SetColor("_HsvAdjust", hue);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
}