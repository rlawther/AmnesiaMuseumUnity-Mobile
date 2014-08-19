using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Toolbelt;
using Kernys.Bson;

public class BSONTestServer : MonoBehaviour {

	BSONListener bl;
	public FPSInputController ic;
	// Use this for initialization
	void Start () {
	
		bl = new BSONListener(5555);
	}
	
	// Update is called once per frame
	void Update () {
	
		BSONObject bo;
		//ic.directionVector.x = 1.0f;
		//Debug.Log ("set dir");

		bo = bl.Receive();

		while (bo != null)
		{
			/*
			foreach (string k in bo.Keys)
			{

				Debug.Log (k + "," + bo[k] + ",\n");
			}
			*/


			if (bo.ContainsKey("x"))
			{
				ic.directionVector.x = bo["x"];
			}
			if (bo.ContainsKey("y"))
			{
				ic.directionVector.z = bo["y"];
			}
			if (bo.ContainsKey("button"))
			{
				if (string.Equals (bo["button"], "artistic"))
					Debug.Log ("ARTISTIC\n");
				else
					Debug.Log ("not ARTISTIC\n");
			}

			bo = bl.Receive();
		}

	}
}
