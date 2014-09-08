using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Toolbelt {
public class GammaLUT : MonoBehaviour, icDebugGUI {
	
	Material targetMat;

	public float brightness = 0.5f;
	protected float _brightness = 0.0f;
	
	public float contrast = 0.5f;
	protected float _contrast = 0.0f;
	
	public float gamma = 1.0f;
	protected float _gamma = 0.0f;

	private icGUIHelpers.Vector3Picker debugBCG;	
	[SerializeField]
	protected bool rebuildImmediately = true;
	[SerializeField][DoNotSerialize]
	protected Texture2D lutTexture;
	[SerializeField]
	protected bool needRebuild = true;
	[SerializeField]
	protected bool realtimeUpdate = true;
	
	private const int GAMMA_LUT_SIZE = 256;


	public void Reset()
	{
		needRebuild = true;
		brightness = 0.5f;
		contrast = 0.5f;
		gamma = 1.0f;
		realtimeUpdate = true;
	}
	public void setRealtimeUpdate(bool enabled) {
		this.realtimeUpdate = enabled;
	}
	
	public void Start() 
	{
		this.debugBCG = new icGUIHelpers.Vector3Picker("Bright/Contrast/Gamma",0f,3f);
		this.debugBCG.setLabels(new string[] { "Brightness", "Contrast", "Gamma"});
		
		if (LevelSerializer.IsDeserializing) return;
				
		if (this.targetMat == null && this.renderer != null)
			this.targetMat = this.renderer.material;
		
	}

	public void Update()
	{
		if (this.realtimeUpdate) {
			if (this.brightness != this._brightness) {
				this._brightness = this.brightness;
				this.needRebuild = true;
			}
			if (this.contrast != this._contrast) {
				this._contrast = this.contrast;
				this.needRebuild = true;
			}
			if (this.gamma != this._gamma) {
				this._gamma = this.gamma;
				this.needRebuild = true;
			}
		}
		if (this.needRebuild)
			RebuildGammaLUT();
	}

	private void RefreshAttachedGamma() {
		//refreshes the attached materials Gamma LUT
		if (this.lutTexture != null && this.targetMat != null)			
			this.targetMat.SetTexture("_uGammaLUT",this.lutTexture);		
	}
	
	void RebuildGammaLUT()
	{
		if (this.lutTexture == null) {			
			if (this.targetMat && this.targetMat.HasProperty("_uGammaLUT")) { //it won't if we are chaining shaders...
				this.lutTexture = this.targetMat.GetTexture("_uGammaLUT") as Texture2D;
			}
			if (this.lutTexture == null) //if _uGammaLUT is STILL null even after pulling from the shader properties 
				this.lutTexture = new Texture2D(GAMMA_LUT_SIZE,1,TextureFormat.Alpha8,false);
						
			//use this otherwise it "jumps" to wrong colour at index 0 and 255, since it bilinearly interpolates
			// the edges, giving you gray.
			this.lutTexture.wrapMode = TextureWrapMode.Clamp;
			this.RefreshAttachedGamma();
		}
		Color32[] cols = this.lutTexture.GetPixels32();
		if (this.gamma == 0.0f)
		{
			// at 0 gamma, inverse gamma should be infinity, so fraction (fi) to the power of gamma is 0
			for (int i = 0; i < GAMMA_LUT_SIZE; i++)
			{
				float x = 0.5f * ((this.brightness - 0.5f) - this.contrast) + 0.25f;
	
				byte val = (byte)(int)(Mathf.Clamp(x * 255.0f, 0, 255));
				cols[i].r = val;
				cols[i].g = val;
				cols[i].b = val;
				cols[i].a = val; //only need to set alpha since its an Alpha8 texture.
				
			}
		}
		else
		{
			float invgamma = 1.0f / this.gamma;
	
			for (int i = 0; i < GAMMA_LUT_SIZE; i++)
			{
				float fi = (float)(i) / GAMMA_LUT_SIZE;
				float x = ((this.contrast + 0.5f) * Mathf.Pow(fi, invgamma)) + (0.5f * ((this.brightness - 0.5f) - this.contrast)) + 0.25f;
	
				byte val = (byte)(int)(Mathf.Clamp((x * 255.0f), 0, 255));
								
				cols[i].r = val;
				cols[i].g = val;
				cols[i].b = val;
				cols[i].a = val; //only need to set alpha since its an Alpha8 texture.
			}
		}
		this.lutTexture.SetPixels32(cols);
		this.lutTexture.Apply();		
		this.needRebuild = false;
	}

	public void SetTargetMat(Material mat) {
		this.targetMat = mat;
		this.RefreshAttachedGamma();
	}
	
	public Texture2D GetTexture()
	{
		if (!this.lutTexture)
			RebuildGammaLUT();
	
		return this.lutTexture;
	}

	public void FlagRebuild()
	{
		if (this.rebuildImmediately)
			RebuildGammaLUT();
		else
			this.needRebuild = true;
	}
	


	public void DrawDebugGUI() {
		GUILayout.BeginVertical();
		Vector3 vals = new Vector3(this.brightness,this.contrast,this.gamma);
		vals = this.debugBCG.DrawGUI(vals);
		this.brightness = vals.x;
		this.contrast = vals.y;
		this.gamma = vals.z;
		if (this.debugBCG.enabled) {
			this.realtimeUpdate = GUILayout.Toggle (this.realtimeUpdate, "Realtime Update");
			if (!this.realtimeUpdate)
				this.needRebuild = GUILayout.Toggle(this.needRebuild, "Rebuild GammaLUT");
		}
		GUILayout.EndVertical();
	}

}




}