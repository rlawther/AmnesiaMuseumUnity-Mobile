using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class ColourSetter : MonoBehaviour {
	
	[SerializeField]
	private Color32 myColor = new Color32(0xFF,0xFF,0xFF,0xFF);	
	[SerializeField]
	private Color32 parentColor = new Color32(0xFF,0xFF,0xFF,0xFF);

	[SerializeField]
	//private bool colorValid = true;
	public bool enableColourParenting = true;
	
	public bool realtimeUpdate = false;
	
	private const float BYTE_FLOAT = 255.0F;
	private Color32 calculateParentedColor() {
		if (!enableColourParenting) {
			return this.myColor;
		}
		Color32 result = new Color32(0,0,0,0);
		result.r = (byte)(int)((myColor.r/BYTE_FLOAT * parentColor.r/BYTE_FLOAT)*BYTE_FLOAT);
		result.g = (byte)(int)((myColor.g/BYTE_FLOAT * parentColor.g/BYTE_FLOAT)*BYTE_FLOAT);
		result.b = (byte)(int)((myColor.b/BYTE_FLOAT * parentColor.b/BYTE_FLOAT)*BYTE_FLOAT);
		result.a = (byte)(int)((myColor.a/BYTE_FLOAT * parentColor.a/BYTE_FLOAT)*BYTE_FLOAT);
		return result;
	}
	
	public Color32 updateRendererMaterial() {
		Color32 finalColour = this.calculateParentedColor();
		if (this.renderer && this.renderer.material) 
		{
			this.renderer.material.SetColor("_Color", finalColour);
		}
		return finalColour;
	}
	
	public void setColour(Color32 c) { //sets colour immediately, but all children 
		this.myColor = c;
		Color32 finalColour = this.updateRendererMaterial ();
		//sets all children's "parentcolour"
		foreach (Transform child in this.transform) {
			ColourSetter cs = child.GetComponent<ColourSetter>();
			if (cs != null && cs.enableColourParenting) {
				cs.setParentColour (finalColour);
			}
		}
	}
	
	public void setParentColour(Color32 c) { 
		//sets colour immediately
		//When the parent's colour changes, 
		//the parent notifies each child of the change.
		this.parentColor = c;		
		this.updateRendererMaterial ();	
	}
	
	public Color32 getColour() {
		return this.myColor;
	}
	
	public Color32 getParentColour() { 
		
		return this.parentColor;		
	}
	
	void Update() {
		if (this.realtimeUpdate) {
			this.setColour (this.myColor);
		}
	}
}
}