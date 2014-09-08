using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class icOggMaterial : icMaterial {
	
	public MovieTexture oggTexture = null;

	protected OggLoaderObject oggLoader;
	public bool isLooping = false;
	
	public void setLooping(bool loop) {
		this.isLooping = loop;
		if (this.renderer) {
			MovieTexture mt = this.renderer.material.GetTexture("_MainTex") as MovieTexture;
			if (mt != null) {
				mt.loop = loop;
			}
		}
	}
	protected override void DoUpdate() 
	{
		if (oggLoader != null) {
			if (oggLoader.IsLoaded()) {
				this.SetTexture2D(oggLoader.GetTexture());
				oggLoader = null;
				LocalMediaReady();
			}
		}
	}
	
	public override void LoadMyself ()
	{
		
		icMediaLoader mediaLoader = ToolbeltManager.FirstInstance.MediaLoader;
		mediaLoader.CreateOggMaterial(this.filePath, this.gameObject);
	}
	
	public override void SetLoaderObject (LoaderObject loader)
	{
		this.filePath = loader.GetFilePath();
		oggLoader = loader as OggLoaderObject;
		if (oggLoader == null) {
			Debug.LogError("Loader for " + this.filePath + " is not an OggLoaderObject");
		}

	}
	
	
	public void SetTexture2D(MovieTexture tex) 
	{
		this.oggTexture = tex;		
		
		renderer.material.SetTexture("_MainTex", tex);
		
		// In order to get texture size, we have to play the texture for one frame.
		// this is a known bug in unity.
		// http://forum.unity3d.com/threads/71220-get-MovieTexture-resolution
		tex.Play();		
		tex.Pause();		
		tex.loop = this.isLooping;
		this.textureAspectRatio = (float)tex.width / tex.height;
	}
	

	
}
}