using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class ImageLoaderObject : LoaderObject 
{
	protected Texture2D loadedTexture = null;
	
	WWW www;
	icImageCacher imageCacher;
	bool loadedFromCache = false;
	Texture2D readyTexture;
	
	public ImageLoaderObject(string filePath, LoadingManager threadedLoader)
	: base(filePath, threadedLoader)
	{
		imageCacher = icImageCacher.FirstInstance;
	}
	
	public override void Update ()
	{
		if (www != null && www.isDone && this.readyTexture == null) {
			this.readyTexture = www.texture;
			
			if (imageCacher != null) {
				imageCacher.AddToCache(this.filePath, this.readyTexture);
			}
		}
	}
	
	public override void Load ()
	{
		// Check if this file is already in the image cache
		if (imageCacher != null) 
		{
			Texture2D tex = imageCacher.GetFileFromCache(this.filePath);
			if (tex != null) {
				this.loadedFromCache = true;
				this.readyTexture = tex;
			}
		}
		
		if (!this.loadedFromCache)
			www = new WWW(this.filePath);
	}
	
	public override bool IsLoaded ()
	{
		return this.readyTexture != null;
	}
	
	public override bool IsNotFound ()
	{
		return !( string.IsNullOrEmpty(www.error) );
	}
	
	public Texture2D GetTexture() {
		return readyTexture;
	}
}
}