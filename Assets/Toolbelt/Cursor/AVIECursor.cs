using UnityEngine;
using System.Collections;

namespace Toolbelt
{
	public class AVIECursor : MonoBehaviour
	{
		public bool usePointerDevice = false;
		public GameObject cursorPosition;
		public float avieRadius = 5.0f;
		public float avieHeight = 4.0f;
		public float rayDistance = 20.0f;
		public float minDistance = 2.0f;
		public LayerMask SELECTABLE_LAYER_MASK = 1 << 8;
		public RaycastHit? lastHit;
		public float cursorFloat = 0.0f; //move cursor towards 0,0,0 by this many units.
		public Transform centreAround;
		public Vector3 wandOffset;
		// Use this for initialization
		void Start ()
		{
	
		}

		private Vector3 getAdjustmentPos ()
		{
			if (centreAround) {
				return centreAround.position + wandOffset;
			} else {
				return Vector3.zero;
			}
		}

		void calcCursorPositionFrom2D()
		{
			float cursorX, cursorY, cursorZ;
			RaycastHit hitInfo;
			Vector3 adjust = this.getAdjustmentPos ();
			
			/*
	         * Determine the postion of the cursor against the screen
	         */
			cursorX = Mathf.Sin (cursorPosition.transform.position.x * 2.0f * Mathf.PI) * avieRadius;
			cursorZ = Mathf.Cos (cursorPosition.transform.position.x * 2.0f * Mathf.PI) * avieRadius;
			cursorY = cursorPosition.transform.position.y * avieHeight - (0.5f * avieHeight);
			
			gameObject.transform.position = new Vector3 (cursorX , cursorY , cursorZ ) + adjust;
			//gameObject.transform.LookAt(Vector3.up * cursorY, Vector3.up);
			
			gameObject.transform.LookAt (adjust, Vector3.up);
			
			
			/*
	         * See if it's on top on another object
	         */ 
			Vector3 startPoint = Vector3.MoveTowards(adjust,gameObject.transform.position,minDistance);
			
			if (Physics.Raycast (startPoint, gameObject.transform.position, out hitInfo, this.rayDistance,
			                     SELECTABLE_LAYER_MASK)) {
				
				gameObject.transform.position = hitInfo.point;
				gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, adjust, this.cursorFloat);
				this.lastHit = hitInfo;
				
				
			} else {
				this.lastHit = null;
			}
		}

		void calcCursorPositionFromPointerDevice()
		{
			float cursorX, cursorY, cursorZ;
			float theta;
			RaycastHit hitInfo;
			Vector3 pointingDir;
			Vector3 adjust = this.getAdjustmentPos();
			pointingDir = cursorPosition.transform.rotation * Vector3.forward;
			/*
	         * Determine the postion of the cursor against the screen
	         */
			theta = Mathf.Atan2 (pointingDir.z, pointingDir.x);
			cursorX = Mathf.Cos (theta) * avieRadius;
			cursorZ = Mathf.Sin (theta) * avieRadius;
			cursorY = ((pointingDir.y / 
			           Mathf.Sqrt(pointingDir.x*pointingDir.x + pointingDir.z*pointingDir.z))
						* avieRadius) + cursorPosition.transform.position.y;
			cursorY = Mathf.Clamp (cursorY, 0, avieHeight);
			
			gameObject.transform.position = new Vector3 (cursorX , cursorY , cursorZ )  + adjust;
			//gameObject.transform.LookAt(Vector3.up * cursorY, Vector3.up);
			
			gameObject.transform.LookAt (adjust, Vector3.up);

			/*
	         * See if it's on top on another object
	         */ 
			Vector3 startPoint = new Vector3(0.0f, cursorPosition.transform.position.y, 0.0f) + adjust;
			
			if (Physics.Raycast (startPoint, pointingDir, out hitInfo, this.rayDistance,
			                     SELECTABLE_LAYER_MASK)) {
				
				gameObject.transform.position = hitInfo.point;
				gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, adjust, this.cursorFloat);
				this.lastHit = hitInfo;
				
			} else {
				this.lastHit = null;
			}

		}

		// Update is called once per frame
		void Update ()
		{
			if (usePointerDevice)
				calcCursorPositionFromPointerDevice();
			else
				calcCursorPositionFrom2D();
		}
	}
}