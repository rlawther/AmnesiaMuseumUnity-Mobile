using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Toolbelt {
public class icImageCacher {
	
	// Can't be a struct because they should be immutable and are copied by value
	public class CacheInfo {
		public Texture2D texture;
		public float staleTime;
		public int usageCount;
	}
	
	static icImageCacher firstInstance;
	
	static public icImageCacher FirstInstance {
		get {
			return firstInstance;
		}
	}
	
	protected int cacheSizeMB;
	protected float graceTime;
	protected Dictionary<string, CacheInfo> cacheDict = new Dictionary<string, CacheInfo>();
	
	public icImageCacher(int cacheSizeMB, float graceTime)
	{
		Debug.Log ("Starting image cacher with " + cacheSizeMB + "MB");
		if (firstInstance == null) {
			firstInstance = this;
		}
		
		this.cacheSizeMB = Mathf.Max(0, cacheSizeMB);
		this.graceTime = Mathf.Max(0, graceTime);
	}
	
	/// Coroutine to remove stale cache entries every cleanupInterval seconds
	public IEnumerator CleanupCoroutine(float cleanupInterval)
	{
		List<string> toRemoveList = new List<string>();
		while (true)
		{
			toRemoveList.Clear();
			
			foreach (KeyValuePair<string, CacheInfo> p in this.cacheDict) {
				CacheInfo info = p.Value;
				
				if (info.usageCount <= 0) {
					// Once nobody is using a texture, we keep track of how long it has been 'stale'
					info.staleTime += cleanupInterval;
				}
				
				if (info.staleTime > this.graceTime) {
					toRemoveList.Add(p.Key);
				}
			}
			
			foreach (string k in toRemoveList) {
				this.cacheDict.Remove(k);
				//Debug.Log("Removed " + k + " from cache");
			}
			
			yield return new UnityEngine.WaitForSeconds(cleanupInterval);
		}
	}
	
	public void AddToCache(string fileName, Texture2D tex)
	{
		CacheInfo info = new CacheInfo();
		info.texture = tex;
		info.staleTime = 0;
		info.usageCount = 1;
		
		try {
			this.cacheDict.Add (fileName, info);
		} catch (System.ArgumentException) {
			// The fileName is already in the dictionary. 
			// Can happen a lot if using multiple WWW threads
			// to load images
			this.cacheDict[fileName].usageCount += 1;
		}
	
		//Debug.Log (fileName + " usage count: " + this.cacheDict[fileName].usageCount);
	}
	
	public Texture2D GetFileFromCache(string fileName)
	{
		CacheInfo info;
		bool ok = cacheDict.TryGetValue(fileName, out info);
		
		if (ok) {
			info.staleTime = 0;	// reset the unused timer
			info.usageCount += 1;
			//Debug.Log (fileName + " usage count: " + info.usageCount);
			return info.texture;
		} else {
			return null;
		}
	}
	
	/// Once the usage count for an image reaches 0, we start the countdown
	/// for it to be removed from the cache
	public void DecrementUsageCount(string fileName)
	{
		CacheInfo info;
		bool ok = this.cacheDict.TryGetValue(fileName, out info);
		
		if (ok) {
			info.usageCount -= 1;
			//Debug.Log (fileName + " usage count: " + info.usageCount);
		}
	}
}
}