using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class icImageMaterial : icMaterial {
	[DoNotSerialize]
	public Texture2D imageTexture = null;

	protected ImageLoaderObject imageLoader;
	
	protected override void DoUpdate ()
	{
		if (imageLoader != null) {
			if (imageLoader.IsLoaded()) {
				this.SetTexture2D(imageLoader.GetTexture());
				imageLoader = null;
				LocalMediaReady();
			}
		}
	}

	override protected void OnDestroy() {
		base.OnDestroy();
		// Tell the image cacher that there is now 1 less icMaterial using this texture
		if (this.imageTexture != null && icImageCacher.FirstInstance != null) {
			icImageCacher.FirstInstance.DecrementUsageCount(this.filePath);
		}
	
	}
	
	
	public override void LoadMyself ()
	{
		icMediaLoader mediaLoader = ToolbeltManager.FirstInstance.MediaLoader;
		mediaLoader.CreateImageMaterial(this.filePath, this.gameObject);
	}
	
	public override void SetLoaderObject (LoaderObject loader)
	{
		this.filePath = loader.GetFilePath();
		imageLoader = loader as ImageLoaderObject;
		if (imageLoader == null) {
			Debug.LogError("Loader for " + this.filePath + " is not an ImageLoaderObject");
		}
	}
	
	
	public void SetTexture2D(Texture2D tex) 
	{
		this.imageTexture = tex;
		
		this.textureAspectRatio = (float)tex.width / tex.height;
	}
	
	protected override Texture GetRawTexture ()
	{
		return this.imageTexture;
	}
	
}
}