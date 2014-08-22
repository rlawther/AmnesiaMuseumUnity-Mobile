using UnityEngine;
using System.Collections;

public class BillboardAndFade : MonoBehaviour {

	public float billboardRadius;

	private VisualizerManager visManager;
	private GameObject player;
	
	// Use this for initialization
	void Start () {
		this.visManager = gameObject.GetComponent<VisualizerManager> ();
		player = GameObject.Find ("First Person Controller");
	}
	
	// Update is called once per frame
	void Update () {
		if (player == null)
		{
			player = GameObject.Find ("First Person Controller");
			return;
		}
		foreach (var currentVis in this.visManager.visualizations) {
			if(currentVis.targetMetadataParser.output == null)
				continue;
			
			foreach (var imageItem in currentVis.targetMetadataParser.output) {
				//If the item doesn't have a transform, go to the next item. 
				if (imageItem.transform == null)
					continue;
				
				var distance = Vector3.Distance (imageItem.transform.position, player.transform.position);
				//var opacity = distance / divider - offset;
				imageItem.material.color = new Color (1.0f, 1.0f, 1.0f, 0.5f);
				
				if (distance < billboardRadius)
				{
					imageItem.transform.LookAt(player.transform.position);
					//Debug.Log ("billboard");
				}
			}
		}
	}
}
