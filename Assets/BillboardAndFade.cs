using UnityEngine;
using System.Collections;

public class BillboardAndFade : MonoBehaviour {

	public bool applyBillboard;
	public float []billboardRadius;
	public bool applyFade;
	public AnimationCurve []fadeCurve;
	public AnimationCurve fadeCurveG1E1;
	public AnimationCurve fadeCurveG1E2;
	public AnimationCurve fadeCurveG1E3;
	public AnimationCurve fadeCurveG2E1;
	public AnimationCurve fadeCurveG2E2;
	public AnimationCurve fadeCurveG2E3;
	public AnimationCurve fadeCurveG2E4;
	public float fadeDistanceMultiplier;

	private VisualizerManager visManager;
	private GameObject player;
	
	private GameObject photoParent = null;
	
	// Use this for initialization
	void Start () {
		this.visManager = gameObject.GetComponent<VisualizerManager> ();
		player = GameObject.Find ("First Person Controller");
		fadeCurve = new AnimationCurve[7];
		fadeCurve[0] = fadeCurveG1E1;
		fadeCurve[1] = fadeCurveG1E2;
		fadeCurve[2] = fadeCurveG1E3;
		fadeCurve[3] = fadeCurveG2E1;
		fadeCurve[4] = fadeCurveG2E2;
		fadeCurve[5] = fadeCurveG2E3;
		fadeCurve[6] = fadeCurveG2E4;
	}
	
	// Update is called once per frame
	void Update () {
		int episodeNum = 0;
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
							new Color (1.0f, 1.0f, 1.0f, fadeCurve[episodeNum].Evaluate(distance / fadeDistanceMultiplier));
					}
					if (applyBillboard)
					{
						if (distance < billboardRadius[episodeNum])
						{
							photo.LookAt(new Vector3(player.transform.position.x,
							                         photo.position.y,
							                         player.transform.position.z));
						}
					}
					
				}
				episodeNum++;
			}
		}
		
	}
}
