using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Toolbelt {
/*** Handles many of the 'nice' features of AVIEGL Materials, such as abstracting out threaded loading.
 * 
 * */
public class icMaterial : MonoBehaviour 
{
	public string filePath;

	public StereoMode stereoMode = StereoMode.BOTH;
	private StereoMode prevStereoMode = StereoMode.BOTH;
	
	/// Automatically rescale the object to fit the aspect ratio of the media
	public bool autoAspectRatio = true;
	
	/// Use the x, y transform scale as the aspect ratio,
	/// or as an aspect ratio multiplier if autoAspectRatio is on
	public bool keepStartingAspectRatio = true;
	
	public float baseScale = 1.0f;
	public List<MonoBehaviour> mediaReadyCallbacks = new List<MonoBehaviour>();

	protected List<icRenderPass> renderPasses = new List<icRenderPass>();

	protected Vector3 aspectRatios = new Vector3(1.0f, 1.0f, 1.0f);
	
	protected float textureAspectRatio = 1.0f;
	
	protected bool bLocalMediaReady = false;
	protected bool bLocalMediaReadyReported = false;
	
	// 'Raw' colour seperate from the unity material used for colour parenting
	public Color icColour = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	public bool multiplyByParentColour = true;
	private Color finalColour = new Color(1.0f,1.0f,1.0f,1.0f);
	
	private Color prevIcColour = new Color(0.0f, 0.0f, 0.0f, 0.0f);
	private bool prevMultiplyByParentColour = false;

	private Renderer prevRenderer = null;
	
	/// If this is true on Start(), then the icMaterial will try to handle
	/// loading the media itself.
	public bool shouldLoadMyself = true;
	
	public Color getFinalColour() {
		return this.finalColour;
	}
	
	void Start()
	{
		this.SubclassPreStart();
		if (this.shouldLoadMyself && this.filePath != null && this.filePath.Length > 0) {
			LoadMyself();
		}
		this.SubclassPostStart();
	}
	
	protected virtual void SubclassPostStart() {
	}
	protected virtual void SubclassPreStart() {
	}
	
	void Update()
	{
		if (this.stereoMode != this.prevStereoMode) {
			this.gameObject.layer = ToolbeltManager.FirstInstance.GetStereoLayer(this.stereoMode);
			this.prevStereoMode = this.stereoMode;
		}

		if (prevIcColour != icColour || prevMultiplyByParentColour != multiplyByParentColour) {
			UpdateEffectiveColour();
		}

		if (this.renderer != this.prevRenderer) { //when renderer loads, make sure to pass colour in.
			this.prevRenderer = this.renderer;
			UpdateEffectiveColour();
		}
		DoUpdate();

		// We don't always want to create a MeshRenderer if it doesn't exist,
		// as icMaterial on empty GameObject can be used purely for colour parenting
		if (this.renderer)
			this.renderer.material.mainTexture = this.GetRawTexture();

		// Do any render passes after the icMaterial has updates it's own stuff
		foreach (icRenderPass rp in this.renderPasses) {
			rp.RenderPass();
		}
	}

	/// Render passes are anything that needs to happen right after the icMaterial updates
	public void AddRenderPass(icRenderPass rp)
	{
		this.renderPasses.Add (rp);
	}
	
	public bool RemoveRenderPass(icRenderPass rp) {
		return this.renderPasses.Remove (rp);		
	}
	/// Update the Unity Material colour based on the current icColour,
	/// and propagate the change down to the children
	protected virtual void UpdateEffectiveColour()
	{
		prevIcColour = icColour;
		prevMultiplyByParentColour = multiplyByParentColour;
		
		if (this.multiplyByParentColour && this.transform.parent != null)
		{
			Color parentColour;
			//check if it has an icMaterial
			if (this.transform.parent.renderer != null) {
				parentColour = this.transform.parent.renderer.material.color;
			} else { 
				icMaterial parentIcMat = this.transform.parent.GetComponent<icMaterial>();
				if (parentIcMat != null) {
					parentColour = parentIcMat.getFinalColour();
				} else {
					parentColour = new Color(1.0f,1.0f,1.0f,1.0f);
				}
			}
			
			this.finalColour = parentColour * icColour;
			if (this.renderer)
				this.renderer.material.SetColor("_Color", finalColour);
		}
		else {
			this.finalColour = icColour;
			if (this.renderer)
				this.renderer.material.SetColor("_Color", icColour);			
		}
		
		PropagateColourChange();
	}
	
	protected void PropagateColourChange()
	{
		foreach (Transform child in this.transform) {
			icMaterial[] icMats = child.GetComponents<icMaterial>();
			foreach (icMaterial icMat in icMats) {
				if (icMat && icMat.multiplyByParentColour) {
					icMat.UpdateEffectiveColour();
				}
			}
		}
	}
	
	/// Set the scale based on the baseScale and aspectRatio
	/// The longest side will always be of scale baseScale
	virtual public void SetBaseScale(float bs) 
	{
		this.baseScale = bs;
		Vector3 localScale = transform.localScale;
		Vector3 asp = this.aspectRatios;
		
		localScale.Set( 
			bs * asp.x,
			bs * asp.y,
			bs * asp.z
		);

		this.transform.localScale = localScale;
	}
	
	/// Shortcut for putting a callback script onto this object that starts off disabled.
	public T CreateMediaReadyCallback<T>() where T : MonoBehaviour
	{
		T beh = this.gameObject.AddComponent<T>();
		beh.enabled = false;
		mediaReadyCallbacks.Add (beh);
		
		return beh;
	}
	
	public void AllMediaReady()
	{
		if (this.keepStartingAspectRatio) 
		{
			// We want to keep the scale already set on the object,
			// so just set the aspect ratio to the scale and we'll
			// normalise it a bit later
			Vector3 scale = this.transform.localScale;
			this.aspectRatios.Set(scale.x, scale.y, scale.z);
			this.baseScale = Mathf.Max(scale.x, scale.y, scale.z);
		}
		
		if (this.autoAspectRatio) {
			// Adjust the aspect ratio to the ratio of the texture
			// Will be normalised below
			this.aspectRatios.y /= this.textureAspectRatio;
		}
		
		if (this.keepStartingAspectRatio || this.autoAspectRatio) {
			NormaliseAspectRatios();	
			this.SetBaseScale(this.baseScale);
		}
		
		// Simply enable callback scripts to make them start next frame
		foreach (MonoBehaviour beh in this.mediaReadyCallbacks) {
			beh.enabled = true;
		}
		//this.mediaReadyCallbacks.Clear(); dont clear, since we need it for loading scenes
	}
	
	protected void NormaliseAspectRatios()
	{
		Vector3 asp = this.aspectRatios;
		float max = Mathf.Max( asp.x, asp.y, asp.z );
		this.aspectRatios /= max;
	}
	
	/// Call this when the material's media has finished loading on this machine
	protected void LocalMediaReady()
	{
		this.bLocalMediaReady = true;
		// TODO: report to the cluster. Pseudocode below
//		if (clusteringMaster != null)
//		{
//			clusteringMaster.fireEvent(new ClusterEvent(this.id, "mediaReady"));
//		}
		
		// TODO: Just assume we're all ready for now
		AllMediaReady();
	}
	
	void OnSerializing() {
		//need to put the transforms scale back to its original scale, before it
		//was modified by aspect ratios.
		this.UndoAspectRatios();
		
	}
	public void UndoAspectRatios() {
		Vector3 ls = this.transform.localScale;
		ls.x /= this.aspectRatios.x;
		ls.y /= this.aspectRatios.y;
		ls.z /= this.aspectRatios.z;
		this.transform.localScale = ls;
	}
	
	void OnSerialized() {
		Vector3 ls = this.transform.localScale;
		ls.x *= this.aspectRatios.x;
		ls.y *= this.aspectRatios.y;
		ls.z *= this.aspectRatios.z;
		this.transform.localScale = ls;
	}
	void OnDeserialized() {
		
		foreach (MonoBehaviour mb in this.mediaReadyCallbacks) {
			mb.enabled = false;
		}
		this.UpdateEffectiveColour();
	}
	virtual protected void OnDestroy() {
		this.UndoAspectRatios();
	}
	protected virtual void DoUpdate() {}
	
	virtual public void SetLoaderObject(LoaderObject loader) { }
	
	/// This will be called if the script existed in the editor with a filePath
	virtual public void LoadMyself() { }

	virtual protected Texture GetRawTexture() { return null; }

	
}
}