using UnityEngine;
using System.Collections;
using Toolbelt;

public class Spin : MonoBehaviour {

	public string remoteHost;
	public int remotePort;

	public float rotateSpeed = 10.0f;
	public float sensitivityRotate = 0.1f;
	public float sensitivityElevate = 0.1f;
	private float ang, elevation, radius;
	public Transform orbitAround;
	private Vector3 offset;

	public Joystick jstick;

	private float pinchDist;
	private Vector2 oldTouchPos;
	private bool pinching = false;
	private Vector3 forwardDir;
	private Vector3 rightDir;

	private bool touchMoved = false;

	private TCPAsyncSender tcpSender;
	private BSONSender bsonSender;
	byte [] testMsg = {1, 2, 3, 4, 5, 6};

	// Use this for initialization
	void Start () {
		radius = 1000.0f;
		ang = 0;
		elevation = Mathf.PI / 2.0f;
		offset = new Vector3(0, 0, 0);

		/*
		tcpSender = new TCPAsyncSender("192.168.1.103", 5555);
		tcpSender.StartThread();
		tcpSender.AddMessage(testMsg);
		*/

		Kernys.Bson.BSONObject bsonObj = new Kernys.Bson.BSONObject();
		bsonObj.Add ("test", "hello");

		bsonSender = new BSONSender(remoteHost, remotePort);
		bsonSender.SendUncompressed(bsonObj);

	}

	private float getDist(Touch p1, Touch p2)
	{
		return 0;
	}

	// Update is called once per frame
	void Update () {
		float x, y, z;

		if (Input.touchCount == 1)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
				touchMoved = false;
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				ang -= touch.deltaPosition.x * sensitivityRotate;
				elevation += touch.deltaPosition.y * sensitivityElevate;
				if (elevation < 0)
					elevation = 0;
				else if (elevation > (Mathf.PI / 2.0f))
					elevation = Mathf.PI / 2.0f;
				touchMoved = true;
			}
			else if (touch.phase == TouchPhase.Ended)
			{
				// this is a click
				if (!touchMoved)
				{
					/* We can use GUI stuff so probably don't need this */
					/*
					Kernys.Bson.BSONObject bsonObj = new Kernys.Bson.BSONObject();
					bsonObj.Add ("x", touch.position.x);
					bsonObj.Add ("y", touch.position.y);
					bsonSender.SendUncompressed(bsonObj);
					*/
				}
			}
		} 
		else if (Input.touchCount == 2)
		{
			Touch t1, t2;
			t1 = Input.GetTouch(0);
			t2 = Input.GetTouch(1);
			if (!pinching)
			{
				pinching = true;
				pinchDist = Vector2.Distance(t1.position, t2.position);
				oldTouchPos = t1.position;
				forwardDir = this.gameObject.transform.forward;
				forwardDir.y = 0;
				rightDir = this.gameObject.transform.right;
				rightDir.y = 0;
			} else {
				float oldPinchdist = pinchDist;
				pinchDist = Vector2.Distance(t1.position, t2.position);
				radius += oldPinchdist - pinchDist;
				offset -= forwardDir * (oldTouchPos.y - t1.position.y);
				offset += rightDir * (oldTouchPos.x - t1.position.x);
				oldTouchPos = t1.position;
			}
		}
		else
		{
			pinching = false;
		}


		if (Input.GetKey(KeyCode.A))
			ang += 0.1f;
		if (Input.GetKey(KeyCode.D))
			ang -= 0.1f;
		if (Input.GetKey(KeyCode.Q))
			if (elevation < (Mathf.PI / 2.0f))
				elevation += 0.1f;
		if (Input.GetKey(KeyCode.E))
			if (elevation > 0.15f)
				elevation -= 0.1f;
		if (Input.GetKey(KeyCode.W))
			radius -= 10.0f;
		if (Input.GetKey(KeyCode.S))
			radius += 10.0f;
		if (Input.GetKey(KeyCode.Z))
			offset += forwardDir;
		if (Input.GetKey(KeyCode.X))
			offset -= forwardDir;

		x = radius * Mathf.Cos(ang) * Mathf.Sin(elevation);
		z = radius * Mathf.Sin(ang) * Mathf.Sin(elevation);
		y = radius * Mathf.Cos(elevation);

		this.gameObject.transform.localPosition = new Vector3(x, y, z) + orbitAround.position + offset;
		this.gameObject.transform.LookAt(orbitAround.position + offset);

		Kernys.Bson.BSONObject bsonObj = new Kernys.Bson.BSONObject();
		bsonObj.Add ("x", jstick.position.x);
		bsonObj.Add ("y", jstick.position.y);
		bsonSender.SendUncompressed(bsonObj);

	}

	void OnGUI () {
		// Make a background box
		GUI.Box(new Rect(10,10,300,260), "Menu");
		
		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(20,40,280,100), "Realistic")) {
			Kernys.Bson.BSONObject bsonObj = new Kernys.Bson.BSONObject();
			bsonObj.Add ("button", "realistic");
			bsonSender.SendUncompressed(bsonObj);
			//Application.LoadLevel(1);
		}
		
		// Make the second button.
		if(GUI.Button(new Rect(20,150,280,100), "Artistic")) {
			Kernys.Bson.BSONObject bsonObj = new Kernys.Bson.BSONObject();
			bsonObj.Add ("button", "artistic");
			bsonSender.SendUncompressed(bsonObj);
			//Application.LoadLevel(2);
		}
		
		GUI.Box(new Rect(Screen.width - 220,10,200,260), "Info");
		GUI.Label(new Rect(Screen.width - 200,30,280,20), "Host : " + remoteHost);
		GUI.Label(new Rect(Screen.width - 200,60,280,20), "Port : " + remotePort);
		
	}

}
