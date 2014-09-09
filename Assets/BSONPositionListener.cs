using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Toolbelt;
using Kernys.Bson;

public class BSONPositionListener : MonoBehaviour {

	BSONListener bl;
	public int listenPort;
	public GameObject playerDisplay;
	private Vector3 newPosition;
	private bool hasNewPosition;
	// Use this for initialization
	void Start () {
	
		Debug.Log ("Starting BSON Pos Listener on " + listenPort);
		bl = new BSONListener(listenPort);
	}
	
	// Update is called once per frame
	void Update () 
	{
		BSONObject bo;

		bo = bl.Receive();
		hasNewPosition = false;
		newPosition = new Vector3 (0, 0, 0);

		while (bo != null)
		{
			Debug.Log ("Got msg");
			if (bo.ContainsKey("newx"))
			{
				hasNewPosition = true;
				newPosition.x = bo["newx"];
			}
			if (bo.ContainsKey("newy"))
			{
				hasNewPosition = true;
				newPosition.y = bo["newy"];
			}
			if (bo.ContainsKey("newz"))
			{
				hasNewPosition = true;
				newPosition.z = bo["newz"];
			}
			bo = bl.Receive();
		}

		if (hasNewPosition)
			playerDisplay.transform.position = newPosition;

	}
}
