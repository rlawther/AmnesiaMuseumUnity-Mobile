using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Assets.AVIE.Scripts.Graphics;

namespace Toolbelt {

[RequireComponent(typeof(InitialisationScript))]

public class AvieDepthOfField34 : MonoBehaviour, icDebugGUI {
	public Camera sampleCamera;
	private bool initialized = false;
	// Use this for initialization
	private Transform clusterManager = null;
	
	public Shader depthToColourShader;
	protected Material depthToColourMat;
	
	
	public Material defaultMat;
	
	public icDepthOfField34.Dof34QualitySetting quality = icDepthOfField34.Dof34QualitySetting.OnlyBackground;
	private icDepthOfField34.Dof34QualitySetting _quality;
	
	
	public icDepthOfField34.DofResolution resolution = icDepthOfField34.DofResolution.Low;
	private icDepthOfField34.DofResolution _resolution;	
	
	public float focalDistance01 = 5f;
	private float _focalDistance01;
	
	public float focalSize;
	private float _focalSize;
	
	public bool visualize = false;
	private bool _visualize = false;
	
	private icDepthOfField34[] myDof;
	
	protected InitialisationScript aviePrefabInitialisationScript;
	
	public List<RenderTexture> projectorDepthBuffersList = new List<RenderTexture>();
	protected Dictionary<ProjectorDescription, RenderTexture> projectorDepthBuffers = new Dictionary<ProjectorDescription, RenderTexture>();

	//Debug Gui stuff
	icGUIHelpers.FloatPicker distancePicker;
	icGUIHelpers.FloatPicker focalSizePicker;
	
    void Start ()
	{
		this.clusterManager = this.transform.FindChild ("Cluster Manager");
		if (this.sampleCamera == null) {
			this.sampleCamera = this.transform.FindChild ("Master Camera").camera;
		}
		this.distancePicker = new icGUIHelpers.FloatPicker("Focal Distance", 0.0f, 30.0f);
		this.focalSizePicker = new icGUIHelpers.FloatPicker("Focal Size", 0.0f, 10.0f);
		//this.visualizePicker = new icGUIHelpers.icGUIBoolPicker("Visualize");
		
		if (!this.depthToColourShader) {
			this.depthToColourShader = Shader.Find("Custom/DepthToColour");
		}		
		
		this.depthToColourMat = new Material(this.depthToColourShader);
		this.depthToColourMat.SetFloat("_nearClipMetres", this.sampleCamera.nearClipPlane);
		this.depthToColourMat.SetFloat("_farClipMetres", this.sampleCamera.farClipPlane);
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (!this.initialized) {
			this.tryInitialize ();
		} else {
			this.checkChanges();
		}
		
	}
	
	protected void SetupSliceDepthBuffers()
	{
		if (!this.aviePrefabInitialisationScript) {
			InitialisationScript apis = this.GetComponent<InitialisationScript>();
			this.aviePrefabInitialisationScript = apis;
							
			//apis.AddAllCameraRenderCallback(AllSlicesRendered);
			apis.AddSliceCameraRenderCallback(SliceCameraRenderDepthBuffer);

			foreach (ProjectorDescription p in apis.GetProjectorDescriptions()) 
			{
				RenderTexture projectorDepthBuffer = new RenderTexture(p._targetRenderTexture.width,
																		p._targetRenderTexture.height,
																		0
																		);
				this.projectorDepthBuffers.Add(p, projectorDepthBuffer);
				this.projectorDepthBuffersList.Add(projectorDepthBuffer);
			}
		}			
	}
	
	protected void SliceCameraRenderDepthBuffer(SliceCameraDescription sliceCam) 
	{
		RenderTexture projDepthBuffer = this.projectorDepthBuffers[sliceCam.Projector];
		RenderTexture old = RenderTexture.active;
		RenderTexture.active = projDepthBuffer;
		
		GL.PushMatrix();
		GL.LoadPixelMatrix(0, projDepthBuffer.width, projDepthBuffer.height, 0);
		
		Rect drawRect = sliceCam.cameraObject.camera.rect;
		drawRect.xMin *= projDepthBuffer.width;
		drawRect.xMax *= projDepthBuffer.width;
		drawRect.yMin *= projDepthBuffer.height;
		drawRect.yMax *= projDepthBuffer.height;

		Graphics.DrawTexture(drawRect, new Texture2D(1, 1), 
							new Rect(0, 0, 1, 1), 
							0, 0, 0, 0,
							this.depthToColourMat);
							
		GL.PopMatrix();
		RenderTexture.active = old;
	}
	
	
	protected void AllSlicesRendered(ProjectorDescription pd)
	{
		// Render a picture-in-picture of the depth buffer

		RenderTexture.active = pd._targetRenderTexture;
		
		GL.PushMatrix();
		GL.LoadPixelMatrix(0, pd._targetRenderTexture.width, pd._targetRenderTexture.height, 0);
				
		Graphics.DrawTexture(new Rect(100, 100, 500, 400), this.projectorDepthBuffers[pd]);
		RenderTexture.active = null;
		
		GL.PopMatrix();	
	}

	
	private void checkChanges() {
		if (this.myDof == null) {
			return;
		}
		
		if (this._quality != this.quality) {
			this._quality = this.quality;
			this.refreshQuality();			
		}
		if (this._resolution != this.resolution) {
			this._resolution = this.resolution;
			this.refreshResolution();
		}
		if (this._focalDistance01 != this.focalDistance01) {
			this._focalDistance01 = this.focalDistance01;
			this.refreshFocalDistance01();
		}
		if (this._focalSize != this.focalSize) {
			this._focalSize = this.focalSize;
			this.refreshFocalSize();
			
		}
		if (this._visualize != this.visualize) {
			this._visualize = this.visualize;
			this.refreshVisualize();
		}
	}
	
	private delegate void updateDof(icDepthOfField34 dof);
	
	private void propagateChange(updateDof change) {
		foreach (icDepthOfField34 dof in this.myDof) {
			change(dof);			
		}
	}
	private void refreshVisualize() {
		updateDof refVis = (x) => { x.visualize = this.visualize; };
		this.propagateChange(refVis);		
	}
	private void refreshQuality() {
		updateDof refQual = (x) => { x.quality = this.quality; };
		this.propagateChange(refQual);		
	}
	
	
	private void refreshResolution() {
		updateDof refRes = (x) => { x.resolution = this.resolution; } ;
		this.propagateChange (refRes);
	}
	
	private void refreshFocalDistance01() {
		
		float result = ZBufferConversion.metresToZBufferValue(this.sampleCamera, this.focalDistance01);
		updateDof focdis = (x) => { x.focalDistance01 = result; } ;
		this.propagateChange (focdis);
	}
	private void refreshFocalSize() {
	float result = ZBufferConversion.metresToZBufferValue(this.sampleCamera, this.focalSize);
		updateDof focsize = (x) => {x.focalSize = result; };
		this.propagateChange(focsize);
	}
	
	private void tryInitialize ()
	{
		
		if (this.clusterManager.childCount == 0)
			return;
		this.initialized = true;
		StartCoroutine(this._initialize());
	}
	
	private IEnumerator _initialize ()
	{
		yield return new WaitForEndOfFrame();
		this.SetupSliceDepthBuffers();
		this.myDof = new icDepthOfField34[clusterManager.childCount];
		int index = 0;
		foreach (Transform child in this.clusterManager) {		
			foreach (Transform child2 in child) {
				if (child2.name.Contains (").Projector(")) {
					icDepthOfField34 dof = child2.gameObject.AddComponent<icDepthOfField34> ();					
					dof.dofBlurShader = Shader.Find ("Hidden/Dof/icSeparableWeightedBlurDof34");
					dof.dofShader = Shader.Find ("Hidden/Dof/icDepthOfField34");
					dof.depthTexture = this.projectorDepthBuffersList[index];
					dof.bokehShader = Shader.Find ("Hidden/Dof/icBokeh34");
					//dof.bokehTexture = //@TODO: Set bokeh texture correctly...
					this.myDof[index] = dof;
					index++;
				}
			}
			
			
		}
	}
	
	public void DrawDebugGUI() {
		GUILayout.BeginVertical();
		this.focalDistance01 = this.distancePicker.DrawGUI(this.focalDistance01);
		this.focalSize = this.focalSizePicker.DrawGUI(this.focalSize);
		this.visualize = GUILayout.Toggle(this.visualize, "Visualize Depth of Field");
		GUILayout.EndVertical();
	}
}
}