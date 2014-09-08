using UnityEngine;
using System.Collections;
namespace Toolbelt {
/** Generic sound component - attach to a GameObject to use.
 * Relies on the icSoundMaster for actual implementation (e.g to send BSON messages to Bellows)
 */
public class icSound : MonoBehaviour {
	
	public string filePath;
	public float volume;
	public bool streaming;
	public bool looping;
	public bool videoSync;
	
	float playRate;
	
	int uniqueId;
	icSoundMaster soundMaster;
	
	Vector3 prevPosition = new Vector3();
	float prevVolume;
	
	void Start()
	{
		this.soundMaster = icSoundMaster.Instance;
		
		// Ensures volume is in range [0, 1]
		this.Volume = volume;
		this.uniqueId = this.soundMaster.SoundCreated(this);
	}
	
	void Reset()
	{
		this.volume = 1.0f;
		this.prevVolume = this.volume;
		this.streaming = false;
		this.looping = false;
		this.videoSync = false;
		this.playRate = 1.0f;
	}
	
	void Update()
	{		
		// Update sound engine if position has changed.
		UnityEngine.Vector3 p = transform.position;
		if (p.x != prevPosition.x || p.y != prevPosition.y || p.z != prevPosition.z) {
			prevPosition = p;
			soundMaster.SoundPositionMsg(this);
		}
		
		// This allows adjusting the volume in the editor in real time.
		if (this.prevVolume != this.volume) {
			this.Volume = this.volume;
		}
	}
	
	public int GetId() {
		return this.uniqueId;
	}
	
	public Vector3 GetWorldPosition() {
		return transform.position;
	}
	
	public string GetFilePath() {
		return this.filePath;
	}
	
	public void Play() {
		this.soundMaster.SoundPlayMsg(this);
	}
	
	public void Pause() {
		this.PlayRate = 0.0f;
	}
	
	public void UnPause() {
		this.PlayRate = 1.0f;
	}
	
	public void Seek(int milliseconds) {
		this.soundMaster.SoundSeekMsg(this, milliseconds);
	}
		
	
	public float Volume {
		get {
			return this.volume;
		}
		set {
			this.volume = Mathf.Clamp01(value);
			this.prevVolume = this.volume;
			soundMaster.SoundVolumeMsg(this);
		}
	}
	
	public bool IsStreaming() {
		return this.streaming;
	}
	
	public bool IsLooping() {
		return this.looping;
	}
	
	public bool IsVideoSync() {
		return this.videoSync;
	}
	
	public float PlayRate {
		get {
			return this.playRate;
		}
		set {
			this.playRate = Mathf.Clamp01(value);
			soundMaster.SoundPlayRateMsg(this);
		}
	}
		
}
}