using UnityEngine;
using System.Collections;
using Toolbelt;
using Kernys.Bson;

public class BSONComms : MonoBehaviour {

	public string remoteHost;
	public int remotePort;

	public bool positionListener;
	public int listenPort;
	public GameObject playerDisplay;
	
	private BSONSender bsonSender;
	private Kernys.Bson.BSONObject bsonObj;
	private bool dataAdded;

	private BSONListener bsonListener;
	private Vector3 newPosition;
	private bool hasNewPosition;
	

	// Use this for initialization
	void Start () {
		bsonSender = new BSONSender(remoteHost, remotePort);
		bsonObj = new Kernys.Bson.BSONObject();
		dataAdded = false;

		if (positionListener)
		{
			bsonListener = new BSONListener(listenPort);

			Debug.Log ("Sending listener active message");
			Kernys.Bson.BSONObject laMsg = new Kernys.Bson.BSONObject();
			laMsg.Add ("positionListener", "active");
			bsonSender.SendUncompressed (laMsg);
		}

	}

	public void reconnect()
	{	
		bsonSender = new BSONSender(remoteHost, remotePort);
	}

	public void addData(string key, Kernys.Bson.BSONValue value)
	{
		//Debug.Log ("BSON Comms add msg " + remotePort);
		bsonObj.Add (key, value);
		dataAdded = true;
	}

	public bool isConnected()
	{
		return bsonSender.isConnected();
	}
	
	// Update is called once per frame
	void Update () {
		BSONObject bo;
		
		if (dataAdded)
		{
			bsonSender.SendUncompressed(bsonObj);
			bsonObj = new Kernys.Bson.BSONObject();
			dataAdded = false;
		}

		if (!positionListener)
			return;

		bo = bsonListener.Receive();
		hasNewPosition = false;
		newPosition = new Vector3 (0, 0, 0);
		
		while (bo != null)
		{
			Debug.Log ("Got new pos");
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
			bo = bsonListener.Receive();
		}
		
		if (hasNewPosition)
			playerDisplay.transform.position = newPosition;
	}
}
