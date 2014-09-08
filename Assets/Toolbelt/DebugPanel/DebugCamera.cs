using UnityEngine;
using System.Collections;
using icGUIHelpers;

namespace Toolbelt {
public class DebugCamera : MonoBehaviour, icDebugGUI {
	FloatPicker orthoSize = new icGUIHelpers.FloatPicker("Orthographic Size",0.0f,1.0f);
	void Start() {
		
	}
	public void DrawDebugGUI() 
	{
		Camera c = this.camera;
		if (c.orthographic) 
		{
			c.orthographicSize = orthoSize.DrawGUI(c.orthographicSize);
		}
	}

}
}