using UnityEngine;
using System.Collections;

public class StutterTest : MonoBehaviour {

	public Camera mainCamera;
	public int delayAmount = 100000000;
	
	private bool isMaster = false;
	
	// Use this for initialization
	void Start () {
	
#if NO_CLUSTER
#elif
		isMaster = ClusterNetwork.IsMasterOfCluster();
#endif
	}
	
	// Update is called once per frame
	void Update () 
	{
		float dt = Time.deltaTime;
		
		if (!isMaster) {
			if (mainCamera) {
				Color bgColour = mainCamera.backgroundColor;
				bgColour.r = (bgColour.r + dt*0.25f) % 0.5f;
				bgColour.g = (bgColour.g + dt*0.05f) % 0.5f;
				
				mainCamera.backgroundColor = bgColour;
			}
			DoDelay();
		} else {

		}
		
	}
	
	private void DoDelay()
	{
		bool spaceDown = Input.GetKeyDown("space");
		if (spaceDown) 
		{
			Debug.Log ("Delaying...");
			int someInt = 0;
			for (int i = 0; i < delayAmount; ++i) {
				someInt = i / 2;
			}
			Debug.Log ("Done");
		}	
	}
	 
}
