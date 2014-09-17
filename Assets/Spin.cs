using UnityEngine;
using System.Collections;
using Toolbelt;

public class Spin : MonoBehaviour {

	public string remoteHost;
	public int remotePort;

	public float rotateSpeed = 10.0f;
	public float sensitivityMove = 10.0f;
	public float sensitivityRotate = 0.1f;
	public float sensitivityElevate = 0.1f;
	public float camMaxHeight;
	public float camMinHeight;
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
		bool tapMove = false;
		Vector3 tapMovePos = new Vector3(0, 0, 0);

		if (Input.touchCount == 1)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
				touchMoved = false;
				/*
				 * We make some "dead zones" near the buttons at the top of the screen and around the joystick
				 * Touch events cannot start in these regions
				 */
				if ((touch.position.y > 1300) || ((touch.position.x > 1920) && (touch.position.y < 528)))
					touchStarted = false;
				else
					touchStarted = true;

				Debug.Log ("Touch start " + touch.position.x + ", " + touch.position.y);
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
				touchMoved = true;
			}
			else if (touch.phase == TouchPhase.Ended)
			{
				// this is a click
				if (touchStarted && !touchMoved)
				{
					RaycastHit hitInfo;
					debug = "x";
					tapCount++;
					if (Physics.Raycast(camera.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0)), out hitInfo))
					{
						debug = "hit";
						playerDisplay.transform.position = hitInfo.point;
						tapMove = true;
						tapMovePos = hitInfo.point + new Vector3(0, 20, 0);

					}
					else
					{
						debug = "miss";
					}
					/* We can use GUI stuff so probably don't need this */
					/*
					Kernys.Bson.BSONObject bsonObj = new Kernys.Bson.BSONObject();
					bsonObj.Add ("x", touch.position.x);
					bsonObj.Add ("y", touch.position.y);
					bsonSender.SendUncompressed(bsonObj);
					*/
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
		bsonObj.Add ("x", jstick.position.x);
		bsonObj.Add ("y", jstick.position.y);
		if (tapMove)
		{
			Debug.Log ("sending MOVE msg");
			bsonObj.Add ("movex", tapMovePos.x);
			bsonObj.Add ("movey", tapMovePos.y);
			bsonObj.Add ("movez", tapMovePos.z);
			Debug.Log ("sent MOVE msg");
		}
		bsonSender.SendUncompressed(bsonObj);

	}
	/*
	void OnGUI () {
		// Make a background box
		GUI.Box(new Rect(10,60,300,260), "Menu");
		
		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(20,90,280,100), "Realistic")) {
			Kernys.Bson.BSONObject bsonObj = new Kernys.Bson.BSONObject();
			bsonObj.Add ("button", "realistic");
			bsonSender.SendUncompressed(bsonObj);
			//Application.LoadLevel(1);
		}
		
		// Make the second button.
		if(GUI.Button(new Rect(20,200,280,100), "Artistic")) {
			Kernys.Bson.BSONObject bsonObj = new Kernys.Bson.BSONObject();
			bsonObj.Add ("button", "artistic");
			bsonSender.SendUncompressed(bsonObj);
			//Application.LoadLevel(2);
		}

		if(GUI.Button(new Rect(20,310,280,100), "Reconnect")) {
			bsonSender = new BSONSender(remoteHost, remotePort);
		}

		if(GUI.Button(new Rect(20,420,280,100), "RemotePort++")) {
			remotePort++;
		}

		int i = 50;
		GUI.Box(new Rect(Screen.width - 220,i + 10,200,260), "Info");
		GUI.Label(new Rect(Screen.width - 200,i + 30,280,20), "Host : " + remoteHost);
		GUI.Label(new Rect(Screen.width - 200,i + 60,280,20), "Port : " + remotePort);
		GUI.Label(new Rect(Screen.width - 200,i + 90,280,20), "Cam height : " + camera.transform.position.y);
		GUI.Label(new Rect(Screen.width - 200,i + 120,280,20), "Cam angle x: " + camera.transform.rotation.eulerAngles.x);
		GUI.Label(new Rect(Screen.width - 200,i + 150,280,20), "Tap count: " + tapCount);
		GUI.Label(new Rect(Screen.width - 200,i + 180,280,20), "D: " + debug);
		GUI.Label(new Rect(Screen.width - 200,i + 210,280,20), playerDisplay.transform.position.x + ", " + playerDisplay.transform.position.z);

	}
	*/

}
