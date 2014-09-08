using UnityEngine;
using System.Collections;

namespace Toolbelt {

public class GetClusterMeshes : GetDebugCamera {
	public GameObject ClusterRoot;
	public override GameObject[] getDebugPanelCameras() {
		icMaterial[] icms = ClusterRoot.GetComponentsInChildren<icMaterial>();
		GameObject[] result = new GameObject[icms.Length];
		for (int i = 0; i < icms.Length; i++) {
			result[i] = icms[i].gameObject;
		}
		
		return result;
	}
}
}