using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class WireFrameChildren : MonoBehaviour, icDebugGUI {

	void OnEnable() {
		WireFrameMode[] wfms = this.GetComponentsInChildren<WireFrameMode>();
		foreach (WireFrameMode wfm in wfms) {
			wfm.enabled = true;
		}
		
	}
	void OnDisable() {
		WireFrameMode[] wfms = this.GetComponentsInChildren<WireFrameMode>();
		foreach (WireFrameMode wfm in wfms) {
			wfm.enabled = false;
		}
	}
	
	public void DrawDebugGUI() {
		GUILayout.Label("Enable/disable toggle WireFrameMode in this objects children.");
	}
}
}