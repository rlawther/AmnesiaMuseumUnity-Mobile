using UnityEngine;
using System.Collections.Generic;

using Kernys.Bson;
namespace Toolbelt {
/***
 * Attach one of these to whatever object you want to be your listener (usually the camera)
 */
public class BellowsMessenger : icSoundMaster {

	public string remoteIP;
	public int remotePort;
	
	private BSONSender bsonSender;
	private BSONObject bsonRoot;
	private BSONObject bsonGlobals;	
	private Dictionary<string, BSONArray> bsonChildren = new Dictionary<string, BSONArray>();
	
	void Awake()
	{
		// Set up BSON objects before script Start()
		bsonRoot = new BSONObject();
		bsonGlobals = new BSONObject();
		
		bsonChildren["load"] = new BSONArray();
		bsonChildren["play"] = new BSONArray();
		bsonChildren["move"] = new BSONArray();
		bsonChildren["volume"] = new BSONArray();
		bsonChildren["playrate"] = new BSONArray();
		bsonChildren["time"] = new BSONArray();
	}
	
	// Called from Start()
	protected override void StartMessenger() 
	{
		bsonSender = new BSONSender(remoteIP, remotePort);
		MasterVolumeMsg(masterVolume);
	}
	
	// Called from Reset()
	protected override void ResetMessenger() 
	{
		remoteIP = "127.0.0.1";
		remotePort = 3341;
	}
	
	// Called from OnApplicationQuit()
	protected override void QuitMessenger() 
	{
		// Send a clearall right before disconnecting
		bsonGlobals["clearall"] = new BSONValue(true);
		bsonRoot["globals"] = bsonGlobals;
		bsonSender.Send(bsonRoot);
		
		// Disconnection handled by BSONSender destructor
		bsonSender = null;
	}
	
	// Update is called once per frame
	protected override void UpdateMessenger () 
	{		
		if (bsonGlobals.Count > 0) {
			bsonRoot["globals"] = bsonGlobals;
		}
		
		// Go through any individual sound volume, position, etc messages that
		// need to be added
		foreach (KeyValuePair<string, BSONArray> entry in bsonChildren) {
			if (entry.Value.Count > 0) {
				bsonRoot[entry.Key] = entry.Value;
			}
		}
		
		// Send a message this frame if we have anything to send
		if (bsonRoot.Count > 0)
		{
			bsonSender.Send(bsonRoot);
			bsonRoot.Clear();
			bsonGlobals.Clear();
			
			foreach (KeyValuePair<string, BSONArray> entry in bsonChildren) {
				entry.Value.Clear();
			}
		}
	}
	
	protected override void MasterVolumeMsg (float vol) {
		this.bsonGlobals["volume"] = vol;
	}
	
	/// We have to re-calculate and re-send all sound positions 
	/// if the listener moves
	protected override void ListenerPositionChanged ()
	{
		foreach (icSound s in this.allSounds) {
			SoundPositionMsg(s);
		}
	}
	
	protected override void SoundLoadMsg (icSound sound)
	{
		BSONObject b = new BSONObject();
		b["id"] = sound.GetId();
		b["streaming"] = new BSONValue(sound.IsStreaming());
		b["looping"] = new BSONValue(sound.IsLooping());
		b["videosync"] = new BSONValue(sound.IsVideoSync());	
		b["permanent"] = new BSONValue(true);
		b["path"] = sound.GetFilePath();
		b["speed"] = sound.PlayRate;
		b["volume"] = sound.Volume;
		
		b["position"] = WorldToBellowsPosition(sound.GetWorldPosition());
		
		bsonChildren["load"].Add (b);
	}
	
	public override void SoundPlayMsg (icSound sound)
	{
		BSONObject b = new BSONObject();
		b["id"] = sound.GetId();
		bsonChildren["play"].Add(b);
	}
	public override void SoundVolumeMsg(icSound sound)
	{
		BSONObject b = new BSONObject();
		b["id"] = sound.GetId();
		b["volume"] = sound.Volume;
		
		bsonChildren["volume"].Add(b);
	}
	
	public override void SoundPositionMsg (icSound sound)
	{
		BSONObject b = new BSONObject();
		b["id"] = sound.GetId();
		b["position"] = WorldToBellowsPosition(sound.GetWorldPosition());
		bsonChildren["move"].Add (b);
	}
	
	public override void SoundPlayRateMsg (icSound sound)
	{
		BSONObject b = new BSONObject();
		b["id"] = sound.GetId();
		b["speed"] = sound.PlayRate;
		bsonChildren["playrate"].Add (b);
	}
	
	public override void SoundSeekMsg (icSound sound, int milliseconds)
	{
		BSONObject b = new BSONObject();
		b["id"] = sound.GetId();
		b["t"] = milliseconds;
		
		bsonChildren["time"].Add (b);
	}
	
	/// Transforms a position in world space to Bellows polar coordinates
	public BSONArray WorldToBellowsPosition(Vector3 vec)
	{
		// Transform point to be relative to whatever this Sound Master is attached to
		// (usually the camera)
		vec = transform.InverseTransformPoint(vec);		
		
		// X value in bellows is simply polar direction
		float bellowsX = Mathf.Atan2(vec.z, vec.x) / (Mathf.PI * 2);
		
		// Y value in bellows is just the height
		float bellowsY = vec.y;
		
		// Z value in bellows is the distance to the sound, ignoring height
		float bellowsZ = Mathf.Sqrt( (vec.x * vec.x) + (vec.z * vec.z) );
		
		BSONArray a = new BSONArray();
		a.Add (bellowsX);
		a.Add (bellowsY);
		a.Add (bellowsZ);
		
		return a;		
	}
}
}