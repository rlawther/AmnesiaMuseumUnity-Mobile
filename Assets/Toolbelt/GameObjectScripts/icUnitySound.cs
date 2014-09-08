using UnityEngine;
using System.Collections;
namespace Toolbelt {
public class icUnitySound : MonoBehaviour {
	public string filePath;
	public AudioClip oggSound = null;
	
	protected SoundLoaderObject oggLoader;
	public bool isLooping = false;
	
	public bool shouldLoadMyself = true;
	
	
	public void setLooping(bool loop) {
		this.isLooping = loop;
		this.audio.loop = loop;			

	}
	void Awake() {
		if (this.audio == null) {
			this.gameObject.AddComponent<AudioSource>();
		}
	}
	void Start()
	{
		if (this.shouldLoadMyself && this.filePath != null && this.filePath.Length > 0) {
			LoadMyself();
		}
	}
	void Update() {
		DoUpdate();
	}
	
	protected void DoUpdate() 
	{
		if (oggLoader != null) {
			if (oggLoader.IsLoaded()) {
				this.oggSound = oggLoader.GetSound();
				this.audio.clip = oggLoader.GetSound();				
				oggLoader = null;				
			}
		}
	}
	
	public void LoadMyself ()
	{		
		icMediaLoader mediaLoader = ToolbeltManager.FirstInstance.SoundLoader;
		mediaLoader.CreateOggSoundMaterial(this.filePath, this.gameObject);
	}
	
	public void SetLoaderObject (LoaderObject loader)
	{
		this.filePath = loader.GetFilePath();
		oggLoader = loader as SoundLoaderObject;
		if (oggLoader == null) {
			Debug.LogError("Loader for " + this.filePath + " is not an OggSoundLoaderObject");
		}

	}
	
	
	public void SetSound(AudioClip s) 
	{
		this.oggSound = s;		
		this.audio.clip = s;		
		this.audio.loop = this.isLooping;			
		this.audio.Play();
	}
	

	
}
}