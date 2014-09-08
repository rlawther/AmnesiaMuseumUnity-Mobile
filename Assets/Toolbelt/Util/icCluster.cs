using UnityEngine;
using System.Collections;

public static class icCluster {
	public static bool isMaster() {
#if UNITY_EDITOR || NO_CLUSTER
		return true;
#else
		return	ClusterNetwork.IsMasterOfCluster();
#endif
	}
}
