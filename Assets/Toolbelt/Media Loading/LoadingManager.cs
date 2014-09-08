using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

namespace Toolbelt {
/*** Class for managing async loading of lots of media.
 */
public class LoadingManager {

	int maxConcurrentLoads = 1;
	
	protected int currentLoadingCount = 0;
	
	LinkedList<LoaderObject> workQueue = new LinkedList<LoaderObject>();
	HashSet<LoaderObject> activeLoaderObjects = new HashSet<LoaderObject>();
	
	public int MaxConcurrentLoads {
		get {
			return maxConcurrentLoads;
		}
		set {
			this.maxConcurrentLoads = Mathf.Clamp(value, 1, 1000);
			Debug.Log("ThreadedLoader set to " + this.maxConcurrentLoads + " concurrent loads");
		}
	}

	
	public void AddLoaderInfo(LoaderObject loaderObj, bool frontOfQueue = false) 
	{
		// Start loading straight away if we haven't reached our limit, or put it in the queue	
		bool loadOk = this.TryLoad(loaderObj);

		if (!loadOk) {
			if (!frontOfQueue) {
				workQueue.AddLast(loaderObj);
			} else {
				workQueue.AddFirst(loaderObj);
			}
		}
	}
	
	public void Update () 
	{		
		// Go through queue of stuff that still needs to be loaded
		if (workQueue.Count > 0) 
		{			
			LoaderObject loader = workQueue.First.Value;
			if (TryLoad(loader)) {			
				workQueue.RemoveFirst();
			}
		}
		
		foreach (LoaderObject lo in this.activeLoaderObjects) {
			lo.Update();
		}
		
		// Check loaders to see if they are done and remove them from the active set
		if (this.activeLoaderObjects.Count > 0) {			
			this.activeLoaderObjects.RemoveWhere(LoaderDone);
		}
	}
	
	/// Comparison function for removing finished LoaderObjects with .RemoveWhere()
	bool LoaderDone(LoaderObject lo) 
	{
		return (lo.IsLoaded() || lo.IsNotFound());		
	}
	
	bool TryLoad(LoaderObject lo)
	{
		if (this.activeLoaderObjects.Count < this.maxConcurrentLoads) {
			lo.Load();
			this.activeLoaderObjects.Add (lo);
			return true;
		} else {
			return false;
		}
		
	}
}

/*** Abstract class for anything you want to load through a ThreadedLoader.
 * Load() will always be called in a ThreadedLoaderWorker, so don't do things
 * there that must be in the main thread (e.g graphics card texture loading)
 */
abstract public class LoaderObject 
{
	protected string filePath;
	protected object loadLock = new Object();
	protected LoadingManager threadedLoader;
	
	public LoaderObject(string filePath, LoadingManager threadedLoader) {
		this.filePath = filePath;
		this.threadedLoader = threadedLoader;
	}
	
	public string GetFilePath() {
		return filePath;
	}
	
	public virtual void Update() {}
	
	abstract public void Load();
	abstract public bool IsLoaded();
	abstract public bool IsNotFound();
	

}
}