using UnityEngine;
using System.Collections;

public class RenderSettingsSetter : MonoBehaviour {

	public bool fog;
	public Color fogColour;
	public float fogDensity;
	public Color ambientLight;
	public Material skyboxMaterial;
	

	// Use this for initialization
	void Start () {
		set();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void set()
	{
		RenderSettings.fog = fog;
		RenderSettings.fogColor = fogColour;
		RenderSettings.fogDensity = fogDensity;
		RenderSettings.ambientLight = ambientLight;
		RenderSettings.skybox = skyboxMaterial;
	}
}
