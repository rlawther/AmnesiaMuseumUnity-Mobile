using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


// TODO: Fix for NO_CLUSTER and CLUSTER so Cluster only has one 3338 port open.
namespace Toolbelt {

public class BasicCursor : MonoBehaviour {
	TCPAsyncListener listener;
	BSONListener bsonListener;
	Kernys.Bson.BSONObject bsonObj;
	iPadBSONParser bsonParser;
	
	[SerializeField]
	public iPadEventHandler eh;
	
	public float xSensitivity = 0.01f;
	public float ySensitivity = 0.01f;
    public bool constrainX = true;
    public bool wrapX = true;
    public bool constrainY = true;
    public bool wrapY = false;

	//public GameObject target;
	#if NO_CLUSTER
	#else
	private icClusterInputBool iPadTapped;
	#endif
	// Use this for initialization
	void Start () {
		Application.runInBackground = true;		
		#if NO_CLUSTER
		#else
		this.iPadTapped = new icClusterInputBool("iPadTapped");
		#endif
		
		

		this.bsonParser = new iPadBSONParser();		
		gameObject.renderer.material.color = Color.blue;
		bsonListener = new BSONListener(3338);
		
	}

    /*
     * A proper modulo that works with negative numbers
     */
    private float nfmod(float a,float b)
    {
        return a - b * Mathf.Floor(a / b);
    }
	
	// Update is called once per frame
	void Update () {
		//byte[] msg;

		bsonObj = null;

		/* Get the first incoming message */
		if (bsonListener != null)
			bsonObj = bsonListener.Receive();	

		while (bsonObj != null)
		{

			this.bsonParser.ParseBSON(bsonObj);
			try {
				//Debug.Log (bsonObj["ipad"]["type"].stringValue);
				if (bsonObj["ipad"]["type"].stringValue == "up")
					gameObject.renderer.material.color = Color.red;
				else if (bsonObj["ipad"]["type"].stringValue == "down")
					gameObject.renderer.material.color = Color.green;
				else if (bsonObj["ipad"]["type"].stringValue == "move")
				{
					//Debug.Log ("MOVE " + bsonObj["ipad"]["dist"][0]);
	                float dx = (float)bsonObj["ipad"]["dist"][0];
	                float dy = (float)bsonObj["ipad"]["dist"][1] * -1.0f;
	                float newX;
	                float newY;
	                newX = gameObject.transform.position.x + (dx * xSensitivity);
	                newY = gameObject.transform.position.y + (dy * ySensitivity);
	                if (constrainX)
	                {
	                    if (wrapX)
	                        newX = nfmod(newX, 1.0f);
	                    else
	                        newX = Mathf.Clamp(newX, 0.0f, 1.0f);
	                }
	
	                if (constrainY)
	                {
	                    if (wrapY)
	                        newY = nfmod(newY, 1.0f);
	                    else
	                        newY = Mathf.Clamp(newY, 0.0f, 1.0f);
	                }
	                gameObject.transform.position = new Vector3(newX, newY, 0);
				}
			} catch (KeyNotFoundException) {
				//unimplemented message.
			}
			
			/* Get the next message */
			bsonObj = bsonListener.Receive();	
		}

		#if NO_CLUSTER
			while (this.bsonParser.messageQueue.Count > 0) {
				IiPadInput ipi = this.bsonParser.messageQueue.Dequeue();
				if (ipi.inputType == InputType.Tap && this.eh != null) {				
					this.eh.handleTap();
				}
			}
		#else
			if (icCluster.isMaster()) {
				this.iPadTapped.SetValue(false); //always set to false at end of frame.
			}
			if (icCluster.isMaster()) {
				while (this.bsonParser.messageQueue.Count > 0) {
					IiPadInput ipi = this.bsonParser.messageQueue.Dequeue();
					if (ipi.inputType == InputType.Tap && this.eh != null) {				
						this.iPadTapped.SetValue(true);
					}
				}
			}

			bool tapped = this.iPadTapped.GetValue();
			if (tapped) {
				this.eh.handleTap();
			}


		#endif
		
			

	}

	void OnApplicationQuit() {
		//Debug.Log ("quit!");
		//listener.StopThread();
	}
}
}