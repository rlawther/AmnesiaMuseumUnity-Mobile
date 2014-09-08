using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class OggLoaderObject : LoaderObject 
{
	protected Texture2D loadedTexture = null;
	WWW www = null;
	
	public OggLoaderObject(string filePath, LoadingManager threadedLoader)
	: base(filePath, threadedLoader)
	{

	}
	
	public override void Load ()
	{
		//Debug.Log ("Downloading " + filePath);
		
		// TODO: image caching
		www = new WWW(this.filePath);
	}
	
	public override bool IsLoaded ()
	{		
		return (this.www != null && www.isDone);
	}
	
	public override bool IsNotFound ()
	{		
		return !( string.IsNullOrEmpty(www.error) );
	}
	
	public MovieTexture GetTexture() {
		return www.movie;
	}
}
}