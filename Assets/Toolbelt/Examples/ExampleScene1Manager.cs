using UnityEngine;
using System.Collections;
using Toolbelt;

// Notes
// You have to add both scenes into the build list.
public class ExampleScene1Manager : ToolbeltManager {
	private bool loadedOther = false;
	private int scene2id = -1;
	protected override void UpdateSubclass() {		
		if (ToolbeltManager.FirstInstance == this &&!this.loadedOther) {
			scene2id = this.loadScene("ExampleScene2");
			this.loadedOther = true;
		}

	}
	//called when another object finished loading by this tm
	protected override void DoneLoadOtherScene(ToolbeltManager tm) { 
		if (tm.getUniqueID() == this.scene2id) {
			Debug.Log ("scene2 loaded!");
			tm.ActivateSceneParent();
		}
		//you can identify the other tm via its uniqueID.
		
	}
}
