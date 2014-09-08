using UnityEngine;
using System.Collections.Generic;
namespace Toolbelt {
/*** Our own abstract interface for sound. 
 * Override the abstract functions to complete implementation 
 * (e.g BellowsMessenger builds Bellows BSON messages and sends them)
 */
abstract public class icSoundMaster : MonoBehaviour {

	static private icSoundMaster instance;
	static public icSoundMaster Instance {
		get {
			return instance;
		}
	}
	
	public float masterVolume;
	
	float prevMasterVolume;	
	int curSoundId = 0;
	
	protected List<icSound> allSounds = new List<icSound>();
	
	
	public icSoundMaster()
	{
		instance = this;
	}
	
	public void SetMasterVolume(float volume)
	{
		// Message is made in Update()
		masterVolume = volume;
	}
	
	public int SoundCreated(icSound sound)
	{
		allSounds.Add(sound);
		
		this.SoundLoadMsg(sound);
		
		return (this.curSoundId)++;
	}
	
	void Start()
	{
		prevMasterVolume = masterVolume;
		this.StartMessenger();
	}
	
	void Reset()
	{
		masterVolume = 1.0f;
		this.ResetMessenger();
	}
	
	void OnApplicationQuit() {
		this.QuitMessenger();
		allSounds.Clear();
	}
	
	
	// Update is called once per frame
	void Update () {
		// Since Master Volume is exposed to the Unity editor,
		// we need to make sure it stays inside [0, 1]
		masterVolume = Mathf.Clamp(masterVolume, 0.0f, 1.0f);
		if (masterVolume != prevMasterVolume) {
			this.MasterVolumeMsg(masterVolume);
			prevMasterVolume = masterVolume;
		}
		
		if (transform.hasChanged) {
			ListenerPositionChanged();
			transform.hasChanged = false;
		}
		
		this.UpdateMessenger();
	}
	
	abstract protected void StartMessenger();
	abstract protected void ResetMessenger();
	abstract protected void QuitMessenger();
	abstract protected void UpdateMessenger();
	
	abstract protected void MasterVolumeMsg(float vol);
	
	/// Up to implementation how it deals with listener position.
	/// e.g Bellows has no concept of a listener, so we must do
	/// all the transforms of sound positions on this end.
	abstract protected void ListenerPositionChanged();
	
	abstract protected void SoundLoadMsg(icSound sound); 
	abstract public void SoundVolumeMsg(icSound sound);
	abstract public void SoundPlayMsg(icSound sound);
	abstract public void SoundPositionMsg(icSound sound);
	abstract public void SoundPlayRateMsg(icSound sound);
	abstract public void SoundSeekMsg(icSound sound, int milliseconds);

}
}