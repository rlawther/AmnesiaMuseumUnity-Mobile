using UnityEngine;
using System.Collections;
using Toolbelt;

public class BSONComms : MonoBehaviour {

	public string remoteHost;
	public int remotePort;

	private BSONSender bsonSender;
	private Kernys.Bson.BSONObject bsonObj;
	private bool dataAdded;

	// Use this for initialization
	void Start () {
		bsonSender = new BSONSender(remoteHost, remotePort);
		bsonObj = new Kernys.Bson.BSONObject();
		dataAdded = false;

		Debug.Log ("Sending test message");
		Kernys.Bson.BSONObject testObj = new Kernys.Bson.BSONObject();
		testObj.Add ("test", "hello");
		bsonSender.SendUncompressed (testObj);
	}

	public void reconnect()
	{	
		bsonSender = new BSONSender(remoteHost, remotePort);
	}

	public void addData(string key, Kernys.Bson.BSONValue value)
	{
		Debug.Log ("BSON Comms add msg " + remotePort);
		bsonObj.Add (key, value);
		dataAdded = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (dataAdded)
		{
			bsonSender.SendUncompressed(bsonObj);
			bsonObj = new Kernys.Bson.BSONObject();
			dataAdded = false;
		}
	}
}
