using UnityEngine;
using System.Collections;

public class BillboardAndFade : MonoBehaviour {

	public bool applyBillboard;
	public float billboardRadius;
	public bool applyFade;
	public AnimationCurve fadeCurve;
	public float fadeDistanceMultiplier;

	private VisualizerManager visManager;
	private GameObject player;
	
	private GameObject photoParent = null;
	
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
		if (photoParent == null)
		{
			photoParent = GameObject.Find ("Photos");
		}
		
		foreach (Transform narrativeScenario in photoParent.transform)
		{
			foreach (Transform episode in narrativeScenario)
			{
				foreach (Transform photo in episode)
				{
					var distance = Vector3.Distance (photo.position, player.transform.position);
					//var opacity = distance / divider - offset;
					if (applyFade)
					{
						photo.gameObject.renderer.material.color = 
							new Color (1.0f, 1.0f, 1.0f, fadeCurve.Evaluate(distance / fadeDistanceMultiplier));
					}
					if (applyBillboard)
					{
						if (distance < billboardRadius)
						{
							photo.LookAt(new Vector3(player.transform.position.x,
							                         photo.position.y,
							                         player.transform.position.z));
						}
					}
				}
			}
			
		}
		
	}
}
