using UnityEngine;
using System.Collections;

class LightState {
	public bool waiting;
	public float animationTime;
	public float randomDelay;
	
}

public class ProximityLightTrigger : MonoBehaviour {

	public Light[] lights;
	public AnimationCurve intensityOverTime;
	public float duration;
	public float randomDelayMin;
	public float randomDelayMax;
	private LightState[] lightStates;
	
	// Use this for initialization
	void Start () {
		int i;
		this.lightStates = new LightState[lights.Length];
		for (i = 0; i < this.lightStates.Length; i++)
		{
			this.lightStates[i] = new LightState();
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other) {
		int lightIndex = 0;
		if (other.name.Equals("First Person Controller"))
		{
			Debug.Log ("prox light enter");
			foreach (Light light in this.lights)
			{
				light.gameObject.SetActive(true);
				light.intensity = intensityOverTime.Evaluate(0.0f);
				lightStates[lightIndex].waiting = true;
				lightStates[lightIndex].animationTime = 0.0f;
				lightStates[lightIndex].randomDelay = Random.Range(randomDelayMin, randomDelayMax);
				lightIndex++;
			}
		}
			
	}
	
	void OnTriggerExit(Collider other) {
		if (other.name.Equals("First Person Controller"))
		{
		    Debug.Log ("prox light exit");
			foreach (Light light in this.lights)
			{
				light.gameObject.SetActive(false);
			}
		}
	}
	
	void OnTriggerStay(Collider other) {
		int lightIndex = 0;
		
		if (other.name.Equals("First Person Controller"))
		{
			Debug.Log ("prox light stay");
			foreach (Light light in this.lights)
			{
				if (lightStates[lightIndex].waiting)
				{
					lightStates[lightIndex].randomDelay -= Time.deltaTime;
					if (lightStates[lightIndex].randomDelay < 0)
					{
						lightStates[lightIndex].waiting = false;
						lightStates[lightIndex].animationTime = 0.0f;
					}
				} 
				else
				{
					lightStates[lightIndex].animationTime += Time.deltaTime;
					if (lightStates[lightIndex].animationTime > this.duration)
					{
						lightStates[lightIndex].waiting = true;
						lightStates[lightIndex].randomDelay = Random.Range(randomDelayMin, randomDelayMax);						
					}
					light.intensity = intensityOverTime.Evaluate(lightStates[lightIndex].animationTime);
				}
				lightIndex++;
			}
		}
	}
			    
}
