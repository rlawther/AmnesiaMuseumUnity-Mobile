using UnityEngine;
using System.Collections;
#if NO_CLUSTER
#else
public class iPadCluster : MonoBehaviour {
	icClusterInputFloat x;
	icClusterInputFloat y;
	// Use this for initialization
	void Start () {
		this.x = new icClusterInputFloat("iPadX");
		this.y = new icClusterInputFloat("iPadY");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = this.transform.localPosition;
		if (icCluster.isMaster()) {


			this.x.SetValue(pos.x);
			this.y.SetValue(pos.y);
		}
		pos.x = this.x.GetValue();
		pos.y = this.y.GetValue();
		this.transform.localPosition = pos;
	}
}
#endif