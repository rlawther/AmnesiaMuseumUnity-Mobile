using UnityEngine;
using System.Collections;
using Toolbelt;
#if NO_CLUSTER
#else
public class ClusterInputTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		ClusterInput.AddInput("MouseTestX", "testDevice1", "10.0.1.28", 0, ClusterInputType.CustomProvidedInput);
		ClusterInput.AddInput("MouseTestY", "testDevice2", "10.0.1.28", 0, ClusterInputType.CustomProvidedInput);
		if (ClusterNetwork.IsMasterOfCluster()) {
			Debug.Log ("I am the cluster master");
		} else {
			Debug.Log ("I am the cluster slave");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (ClusterNetwork.IsMasterOfCluster()) {
			Vector3 mousepos = Input.mousePosition;
			mousepos.x /= Screen.width;
			mousepos.y /= Screen.height;
			ClusterInput.SetAxis("MouseTestX", mousepos.x);
			ClusterInput.SetAxis("MouseTestY", mousepos.y);
			Debug.Log ("SetAxis to" + mousepos.ToString());

		} 

		Vector2 pos = new Vector2();
		pos.x = ClusterInput.GetAxis("MouseTestX");
		pos.y = ClusterInput.GetAxis("MouseTestY");
		this.transform.position =new Vector3 (pos.x,pos.y,5.0f);
		Debug.Log ("getAxis" + pos.ToString());

	}
}

#endif