using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class icMediaLoader {
	
	static icMediaLoader firstInstance;
	
	protected LoadingManager threadedLoader;
	
	static public icMediaLoader FirstInstance {
		get {
			return firstInstance;
		}
	}
	
	public icMediaLoader(int concurrentLoads, bool useBink) 
	{
		this.threadedLoader = new LoadingManager();
		this.threadedLoader.MaxConcurrentLoads = concurrentLoads;
		
		if (firstInstance == null) 
			firstInstance = this;
	}
	
	public void Update()
	{
		this.threadedLoader.Update();
	}
	
	/// Give an existing GameObject an icImageMaterial with a given image file.
	public icImageMaterial CreateImageMaterial(string filePath, GameObject go)
	{
		icImageMaterial mat = go.GetComponent<icImageMaterial>();
			
		if (mat == null)
			mat = go.AddComponent<icImageMaterial>();
		
		ImageLoaderObject lo = new ImageLoaderObject(filePath, this.threadedLoader);
		mat.SetLoaderObject(lo);
		this.threadedLoader.AddLoaderInfo(lo);
		
		return mat;
	}
	
	public icOggMaterial CreateOggMaterial(string filePath, GameObject go)
	{
		icOggMaterial mat = go.GetComponent<icOggMaterial>();
		
		if (mat == null) 
			mat = go.AddComponent<icOggMaterial>();
		
		OggLoaderObject lo = new OggLoaderObject(filePath, this.threadedLoader);
		mat.SetLoaderObject(lo);
		this.threadedLoader.AddLoaderInfo(lo);
		
		return mat;
	}
	
	public icUnitySound CreateOggSoundMaterial(string filePath, GameObject go)
	{
		icUnitySound so = go.GetComponent<icUnitySound>();
		
		if (so == null) 
			so = go.AddComponent<icUnitySound>();
		
		SoundLoaderObject lo = new SoundLoaderObject(filePath, this.threadedLoader);
		so.SetLoaderObject(lo);
		this.threadedLoader.AddLoaderInfo(lo);
		
		return so;
	}
	
	/// Give an existing GameObject an icBinkMaterial with a given local Bink file.
	public icBinkMaterial CreateBinkMaterial(string filePath, GameObject go)
	{
		if (ToolbeltManager.FirstInstance.useBink)
		{
			icBinkMaterial mat = go.GetComponent<icBinkMaterial>();
				
			if (mat == null)
				mat = go.AddComponent<icBinkMaterial>();
			
			BinkPlugin bp = go.GetOrAddComponent<BinkPlugin>();
			mat.LoadMovie(filePath, bp);
			
			return mat;
		}
		else {
			Debug.LogWarning("Tried to CreateBinkMaterial() when ToolbeltManager::useBink is false. Returning null. \"" + filePath + "\"");
			return null;
		}
	}

}
}