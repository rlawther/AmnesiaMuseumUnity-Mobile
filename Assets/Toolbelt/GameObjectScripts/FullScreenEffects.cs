using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class FullScreenEffects : MonoBehaviour {
	public GammaLUT gammaLUTScript;
	public Material mat;
	// Use this for initialization
	void Awake() {
		this.mat = new Material(Shader.Find("iCinema/FullScreenCameraShader"));
		if (LevelSerializer.isBeingObjectTreeLoaded(this.gameObject)) {			
			return;
		}

		this.gammaLUTScript = gameObject.AddComponent<GammaLUT>();	
		this.gammaLUTScript.SetTargetMat(mat);
	}
	void Start () {
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDeserialized() {
		this.gammaLUTScript = this.GetComponent<GammaLUT>();
		this.gammaLUTScript.SetTargetMat(mat);
		
		//Hack to disable/re-enable this script after deserialization. 
		//Required because OnRenderImage won't get called unless you do this...
		this.enabled = false;
		this.enabled = true;
	}
	
	void OnRenderImage(RenderTexture src, RenderTexture dest) {		
		Graphics.Blit(src,dest,mat);		
	}
}
}