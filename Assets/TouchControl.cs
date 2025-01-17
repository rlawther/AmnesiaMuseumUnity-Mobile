﻿using UnityEngine;
using System.Collections;
using Toolbelt;

public class TouchControl : MonoBehaviour {
	

	public float rotateSpeed = 10.0f;
	public float sensitivityMove = 10.0f;
	public float sensitivityRotate = 0.1f;
	public float sensitivityElevate = 0.1f;
	public float joystickDeadZone;
	public float camMaxHeight;
	public float camMinHeight;
	public float clickDelta;
	public float doubleClickDelta;
	public float doubleClickTimeDelta;
	private float ang, elevation, radius;
	public Transform orbitAround;
	private Vector3 offset;

	public Joystick jstick;
	
	public Camera camera;

	private float pinchDist;
	private Vector2 oldTouchPos;
	private float oldTouchAngle;
	private bool pinching = false;
	private Vector3 forwardDir;
	private Vector3 rightDir;

	private bool touchMoved = false;

	private TCPAsyncSender tcpSender;
	private BSONSender bsonSender;
	byte [] testMsg = {1, 2, 3, 4, 5, 6};

	private int tapCount = 0;
	private string debug = "";
	public GameObject playerDisplay;
	private bool touchStarted = false;

	private Vector2 touchStartPos;
	private Vector2 lastClickPos;
	private float lastClickTime;

	private BSONComms bsonComms;

	public bool debugMode;
	private bool sentJoystickZero;


	// Use this for initialization
	void Start () {
		radius = 1000.0f;
		ang = 0;
		elevation = 0.1f;
		offset = new Vector3(0, 0, 0);

		/*
		tcpSender = new TCPAsyncSender("192.168.1.103", 5555);
		tcpSender.StartThread();
		tcpSender.AddMessage(testMsg);
		*/
		/*
		Kernys.Bson.BSONObject bsonObj = new Kernys.Bson.BSONObject();
		bsonObj.Add ("test", "hello");

		bsonSender = new BSONSender(remoteHost, remotePort);
		bsonSender.SendUncompressed(bsonObj);
		*/

		bsonComms = GetComponent<BSONComms> ();
		sentJoystickZero = false;


	}

	private float getDist(Touch p1, Touch p2)
	{
		return 0;
	}

	// Update is called once per frame
	void Update () {
		float x, y, z;
		bool tapMove = false;
		Vector3 tapMovePos = new Vector3(0, 0, 0);


		if (Input.touchCount == 1)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
				touchMoved = false;
				touchStartPos = touch.position;
				/*
				 * We make some "dead zones" near the buttons at the top of the screen and around the joystick
				 * and on the left hand side of the screen to avoid thumb presses
				 * Touch events cannot start in these regions
				 */
				if ((touch.position.y > 1300) ||
				    ((touch.position.x > 1920) && (touch.position.y < 528)) ||
				    (touch.position.x < 300))
					touchStarted = false;
				else
					touchStarted = true;

				//Debug.Log ("Touch start " + touch.position.x + ", " + touch.position.y);
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				Vector3 cameraPos;
				float moveAmount;

				/*
				ang -= touch.deltaPosition.x * sensitivityRotate;
				elevation += touch.deltaPosition.y * sensitivityElevate;
				if (elevation < 0)
					elevation = 0;
				else if (elevation > (Mathf.PI / 2.0f))
					elevation = Mathf.PI / 2.0f;
				*/
				if (touchStarted)
				{
					moveAmount = sensitivityMove * (camera.transform.position.y / camMaxHeight);
					cameraPos = camera.transform.position;
					cameraPos -= (touch.deltaPosition.x * moveAmount) * Vector3.right;
					cameraPos -= (touch.deltaPosition.y * moveAmount) * Vector3.forward;
					camera.transform.position = cameraPos;
				}
				if (!touchMoved)
				{
					if (Vector2.Distance(touchStartPos, touch.position) > clickDelta)
						touchMoved = true;
				}
			}
			else if (touch.phase == TouchPhase.Ended)
			{
				// this is a click
				if (touchStarted && !touchMoved)
				{
					/* test for a double click */
					if ((Vector2.Distance(lastClickPos, touch.position) < doubleClickDelta) &&
					    ((Time.time - lastClickTime) < doubleClickTimeDelta))
					{
						RaycastHit hitInfo;
						debug = "x";
						tapCount++;
						if (Physics.Raycast(camera.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0)), out hitInfo))
						{
							debug = "hit";
							//playerDisplay.transform.position = hitInfo.point;
							tapMove = true;
							tapMovePos = hitInfo.point + new Vector3(0, 20, 0);

						}
						else
						{
							debug = "miss";
						}
					}

					lastClickTime = Time.time;
					lastClickPos = touch.position;
				}
				touchStarted = false;
			}

		} 
		else if (Input.touchCount == 2)
		{
			Touch t1, t2;
			Vector2 touchPos;
			Vector2 touchDiff;
			float touchAngle;
			Vector3 oldCamRotation;
			t1 = Input.GetTouch(0);
			t2 = Input.GetTouch(1);
			if (!pinching)
			{
				pinching = true;
				pinchDist = Vector2.Distance(t1.position, t2.position);
				oldTouchPos = (t1.position + t2.position) * 0.5f; // midpoint
				forwardDir = camera.transform.forward;
				forwardDir.y = 0;
				rightDir = camera.transform.right;
				rightDir.y = 0;
				touchDiff = t2.position - t1.position;
				oldTouchAngle = Mathf.Atan2(touchDiff.y, touchDiff.x);
			} else {
				float oldPinchdist = pinchDist;
				pinchDist = Vector2.Distance(t1.position, t2.position);
				camera.transform.position += new Vector3(0, oldPinchdist - pinchDist, 0);
				offset -= forwardDir * (oldTouchPos.y - t1.position.y);
				offset += rightDir * (oldTouchPos.x - t1.position.x);
				touchPos = (t1.position + t2.position) * 0.5f; // midpoint
				//if (camera.transform.rotation.eulerAngles.x < 90)
				oldCamRotation = camera.transform.eulerAngles;
				camera.transform.Rotate((oldTouchPos.y - touchPos.y) * -0.05f, 0, 0);
				if (camera.transform.rotation.eulerAngles.y > 170)
				{
					camera.transform.eulerAngles = oldCamRotation;
				}

				touchDiff = t2.position - t1.position;
				touchAngle = Mathf.Atan2(touchDiff.y, touchDiff.x);
				//camera.transform.Rotate(0, 0, (touchAngle - oldTouchAngle) * -60.0f);

				oldTouchPos = touchPos;
				oldTouchAngle = touchAngle;
			}
		}
		else
		{
			pinching = false;
		}

		if (camera.transform.position.y > camMaxHeight)
		{
			Vector3 pos;
			pos = camera.transform.position;
			pos.y = camMaxHeight;
			camera.transform.position = pos;
		}
		else if (camera.transform.position.y < camMinHeight)
		{
			Vector3 pos;
			pos = camera.transform.position;
			pos.y = camMinHeight;
			camera.transform.position = pos;
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
			camera.transform.position += new Vector3(100, 0, 0);
		if (Input.GetKey(KeyCode.S))
			radius += 10.0f;
		if (Input.GetKey(KeyCode.Z))
			offset += forwardDir;
		if (Input.GetKey(KeyCode.X))
			offset -= forwardDir;

		x = radius * Mathf.Cos(ang) * Mathf.Sin(elevation);
		z = radius * Mathf.Sin(ang) * Mathf.Sin(elevation);
		y = radius * Mathf.Cos(elevation);

		//camera.transform.localPosition = new Vector3(x, y, z) + orbitAround.position + offset;
		//camera.transform.LookAt(orbitAround.position + offset);

		Kernys.Bson.BSONObject bsonObj = new Kernys.Bson.BSONObject();
		if ((jstick.position.x >= joystickDeadZone) || (jstick.position.x <= -1 * joystickDeadZone) ||
		    (jstick.position.y >= joystickDeadZone) || (jstick.position.y <= -1 * joystickDeadZone))
		{

			bsonComms.addData ("x", jstick.position.x);
			bsonComms.addData ("y", jstick.position.y);
			sentJoystickZero = false;
		}
		else if (!sentJoystickZero)
		{			
			bsonComms.addData ("x", 0.0f);
			bsonComms.addData ("y", 0.0f);
			sentJoystickZero = true;
		}
		if (tapMove)
		{
			Debug.Log ("sending MOVE msg");
			bsonComms.addData ("movex", tapMovePos.x);
			bsonComms.addData ("movey", tapMovePos.y);
			bsonComms.addData ("movez", tapMovePos.z);
			Debug.Log ("sent MOVE msg");
		}
		//bsonSender.SendUncompressed(bsonObj);

	}

	void OnGUI () {

		if (!debugMode)
			return;

		int ypos = 80;
		// Make a background box
		GUI.Box(new Rect(10,ypos,300,260), "Menu");
		
		if(GUI.Button(new Rect(20,ypos + 30,280,100), "Reconnect")) {
			bsonComms.reconnect ();
		}

		if(GUI.Button(new Rect(20,ypos + 140,280,100), "RemotePort++")) {
			bsonComms.remotePort++;
		}

		ypos = 80;
		GUI.Box(new Rect(Screen.width - 220, ypos + 10,200,260), "Info");
		GUI.Label(new Rect(Screen.width - 200, ypos + 30,280,20), "Host : " + bsonComms.remoteHost);
		GUI.Label(new Rect(Screen.width - 200, ypos + 60,280,20), "Port : " + bsonComms.remotePort);
		GUI.Label(new Rect(Screen.width - 200, ypos + 90,280,20), "Connected : " + bsonComms.isConnected());
		GUI.Label(new Rect(Screen.width - 200, ypos + 120,280,20), "Cam angle x: " + camera.transform.rotation.eulerAngles.x);
		GUI.Label(new Rect(Screen.width - 200, ypos + 150,280,20), "Tap count: " + tapCount);
		GUI.Label(new Rect(Screen.width - 200, ypos + 180,280,20), "D: " + debug);
		GUI.Label(new Rect(Screen.width - 200, ypos + 210,280,20), playerDisplay.transform.position.x + ", " + playerDisplay.transform.position.z);

	}


}
