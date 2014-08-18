using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Toolbelt;
using Kernys.Bson;

public class BSONTestServer : MonoBehaviour {

	BSONListener bl;
	// Use this for initialization
	void Start () {
	
		bl = new BSONListener(5555);
	}
	
	// Update is called once per frame
	void Update () {
	
		BSONObject bo;

		bo = bl.Receive();

		while (bo != null)
		{
			foreach (string k in bo.Keys)
			{
				Debug.Log (k + "," + bo[k] + ",\n");
			}

			if (bo.ContainsKey("touch"))
			{
				if (string.Equals (bo["touch"], "artistic"))
					Debug.Log ("ARTISTIC\n");
				else
					Debug.Log ("not ARTISTIC\n");
			}

			bo = bl.Receive();
		}

	}
}
