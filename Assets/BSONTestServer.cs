using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Toolbelt;
using Kernys.Bson;

public class BSONTestServer : MonoBehaviour {

	BSONListener bl;
	public FPSInputController ic;
	public bool yAxisTurn;
	// Use this for initialization
	void Start () {
	
		bl = new BSONListener(5558);
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
				ic.networkDirectionVector.x = bo["x"];
			}
			if (bo.ContainsKey("y"))
			{
				if (yAxisTurn)
					transform.Rotate(0, bo["y"] * 15.0f, 0);
				else
					ic.networkDirectionVector.z = bo["y"];
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
