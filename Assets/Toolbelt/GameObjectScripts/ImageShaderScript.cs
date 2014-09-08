using UnityEngine;
using System.Collections;
using System;
namespace Toolbelt {
/***
 * Place this on any gameObject to give it an ImageShader material.
 * Creates GammaLUT script automatically if it does not exist.
 * Note that this uses the Material's current colour and texture as input to the shader,
 * so make sure this Update() is run after any icMaterials set the colour and texture
 * (e.g after icBinkMaterial renders a frame to the Material)
 */
public class ImageShaderScript : icRenderPass, icDebugGUI
{
	public enum StereoTypes {
		none,
		leftRight,
		topBottom,
		rightLeft,
		bottomTop
	};

	public Vector3 HSL;
	private icGUIHelpers.Vector3Picker HSLDebugPicker;
	// Will do a separate render to texture pass if this box is ticked
	// Useful if you want to apply another shader on the final Material
	// This will only work if the GameObject has an icMaterial
	public bool separatePass;
	public bool transparent = false;
	[DoNotSerialize]
	public RenderTexture renderPassTex;

	public StereoTypes stereoType;
	
	GammaLUT gammaLut;	
	Material imageShaderMat;

	protected ImageShaderScript()
	{
		this.Reset ();
	}

	void Reset() 
	{
		this.HSL.Set(0, 1, 1);
		this.separatePass = false;
		this.renderPassTex = null;
		this.stereoType = StereoTypes.none;
	}

	protected override void DoAwake ()
	{
		if (LevelSerializer.isBeingObjectTreeLoaded(this.gameObject)) {	
			return;	//Assign gammalut after loading
		}
		// ImageShader needs a GammaLUT for Brightness/Contrast/Gamma to work
		// Use existing GammaLUT if it exists...
		this.gammaLut = this.gameObject.GetComponent<GammaLUT>();

		// ...or create one
		if (this.gammaLut == null) {
			this.gammaLut = this.gameObject.AddComponent<GammaLUT>();
		}
	}
	
	void CreateImageShaderFromPass() {
		if (this.separatePass) {
			// Create an intermediate Material for separate pass
			// We have to use the non-transparent version or things like specular will
			// not work in the final pass
			this.imageShaderMat = new Material(Shader.Find ("iCinema/ImageShader"));
		} else {
			if (this.renderer == null) {
				this.gameObject.AddComponent<MeshRenderer>();
			}
			Texture mainTex = null;
			try {
				mainTex = this.renderer.material.mainTexture;
			} catch (NullReferenceException) {
			}
			// Set the shader directly on the final Material
			if (this.transparent)
				this.renderer.material = new Material(Shader.Find ("iCinema/ImageShaderTransparent"));
			else
				this.renderer.material = new Material(Shader.Find ("iCinema/ImageShader"));
			this.imageShaderMat = this.renderer.material;
			this.renderer.material.mainTexture = mainTex;
		}
	}
	
	void Start () 
	{
		HSLDebugPicker = new icGUIHelpers.Vector3Picker("HSL",0f,1f);
		HSLDebugPicker.setLabels(new string[] { "Hue", "Saturation", "Luminance"});
		
		if (LevelSerializer.isBeingObjectTreeLoaded(this.gameObject)){
			return;						
		}
		this.CreateImageShaderFromPass();
		this.gammaLut.SetTargetMat(this.imageShaderMat);
		
		
	}
	void OnDeserialized() {		
		this.CreateImageShaderFromPass();
		//instead of creating a gammaLUT we get the one that was saved.
		this.gammaLut = this.GetComponent<GammaLUT>();		
		this.gammaLut.SetTargetMat(this.imageShaderMat);
	}
	
	void OnWillRenderObject() {
		if (this.imageShaderMat) {
			CameraEyeCallback cameraEyeScript = Camera.current.GetComponent<CameraEyeCallback>();
			if (cameraEyeScript) {
				this.imageShaderMat.SetInt("_uEye", (int)cameraEyeScript.stereoMode - 1);
			}
			//this.imageShaderMat.SetInt ("_uEye", (int)ToolbeltManager.FirstInstance.GetCurrentEye() - 1);
		}
	}

	/// Take the texture on the gameObject's Material and apply ImageShader to it
	public override void RenderPass ()
	{	

		if (this.renderer.material.mainTexture != null)
		{
				
			Texture inputTex = this.renderer.material.mainTexture;
			if (inputTex == null) return;
			Material mat = this.imageShaderMat;
			if (mat == null) return;

			mat.SetTexture("_MainTex", inputTex);
			mat.SetVector("_HsvAdjust", this.HSL);
			mat.SetInt ("_StereoType", (int)this.stereoType);

			// Because ImageShader is a surface shader, we have to explicitly tell it
			// not to use lighting when doing a render-to-texture (Graphics.Blit)
			mat.SetInt("_UseLighting", this.separatePass ? 0 : 1);

			mat.SetInt ("_uEye", (int)ToolbeltManager.FirstInstance.GetCurrentEye());
			
			if (this.separatePass) 
			{
				if (this.renderPassTex == null) {
					this.renderPassTex = new RenderTexture(inputTex.width, inputTex.height, 0, RenderTextureFormat.Default);
					this.renderPassTex.wrapMode = TextureWrapMode.Repeat;					
				}
				if (this.icMat != null) {
					mat.SetColor("_Color", this.icMat.getFinalColour());
				} else {
					mat.SetColor ("_Color", Color.white);
				}
								
				Graphics.Blit(null, this.renderPassTex, mat);

				this.renderer.material.mainTexture = this.renderPassTex;

				// Colour has already been dealt with in the ImageShader, 
				// so just set final colour to white
				this.renderer.material.color = Color.white;
			} 
			else
			{
				mat.SetColor("_Color", this.renderer.material.color);
			}
		}
	}
	

	
	
	public void DrawDebugGUI() {
		GUILayout.BeginVertical();
		this.HSL = HSLDebugPicker.DrawGUI(this.HSL);
		
		GUILayout.EndVertical();
		
	}
}
}