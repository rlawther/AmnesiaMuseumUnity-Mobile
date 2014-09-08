using UnityEngine;
using System.Collections;
using icExtensionMethods;
using BinkInterface;

namespace Toolbelt {
public class icBinkMaterial : icMaterial {
	
	protected BinkPlugin binkPlugin;
	public bool enableLighting = true;
	public bool isGameSpeedAgnostic = false;
	public bool IsMovieLoaded() {
		return (this.binkPlugin != null && this.binkPlugin.MovieLoaded);
	}
	
	public BinkPlayOptions getBinkPlayOptions() {
		return this.gameObject.GetOrAddComponent<BinkPlayOptions>();
	}	
	public override void LoadMyself ()
	{
		icMediaLoader mediaLoader = ToolbeltManager.FirstInstance.MediaLoader;
		mediaLoader.CreateBinkMaterial(this.filePath, this.gameObject);
	}
	
	public void LoadMovie(string url)
	{
		this.filePath = url.Replace("\\","/");
		this.binkPlugin = this.gameObject.GetOrAddComponent<BinkPlugin>();
		this._LoadMovie();
	}
	
	public void LoadMovie(string url, BinkPlugin bp) 
	{
		this.filePath = url;
		this.binkPlugin = bp;
		
		this._LoadMovie();

	}

	private void _LoadMovie()
	{
		BinkPlayOptions bpo = this.getBinkPlayOptions();
		BinkPlugin bp = this.binkPlugin;
		string url = this.filePath;
		
		if (bp.enabled && bp.MovieLoaded) {
			bp.Cleanup();
			//undo textureAspectRatio...
			this.UndoAspectRatios();
		}
		
		bp.SetAlpha(bpo.alphaChannel);	
		bp.isGameSpeedAgnostic = this.isGameSpeedAgnostic;	
		bp.enabled = true;
		
		bool ok = bp.LoadMovie(url);
		
		if (ok) {
			this.textureAspectRatio = bp.getAspectRatio();
		} else {
			Debug.LogError ("Problem loading Bink file " + url);
		}
		
		LocalMediaReady();
	}
	

	public override void SetLoaderObject (LoaderObject loader)
	{
		throw new System.NotImplementedException ();
	}

	protected override Texture GetRawTexture ()
	{
		if (this.binkPlugin != null) {
			return this.binkPlugin.GetFinalTexture();
		} else {
			return null;
		}
	}
	protected override void SubclassPreStart ()
	{
		base.SubclassPreStart ();
		gameObject.GetOrAddComponent<BinkPlayOptions>(); //ensure there are binkplayoptions
	}
	
}
}