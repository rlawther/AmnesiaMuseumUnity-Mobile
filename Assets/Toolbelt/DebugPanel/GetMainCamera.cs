using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class GetMainCamera : GetDebugCamera {

	public override GameObject[] getDebugPanelCameras() {
		return new GameObject[] {Camera.main.gameObject};
	}
}
}
