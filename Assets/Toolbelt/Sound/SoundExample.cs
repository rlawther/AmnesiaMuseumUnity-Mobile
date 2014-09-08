using UnityEngine;
using System.Collections;

namespace Toolbelt {
/*** Adds an icSound to object and plays it. 
 * Sound pauses if it goes belows a certain distance
 * 
 */
public class SoundExample : MonoBehaviour {
	
	public string soundFile = "S:\\Media\\Blockworld\\beethoven9thChunk.wav";
	public float pauseDistance = 4.0f;
		
	icSound testSound = null;
	bool started = false;
	
	// Use this for initialization
	void Start () {
		testSound = gameObject.AddComponent("icSound") as icSound;
		
		testSound.filePath = soundFile;
		testSound.looping = true;
		testSound.streaming = true;
		testSound.volume = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (!started) {
			started = true;
			testSound.Play();
		}
		
		// Stop sound if it goes below a certain distance from 0
		float sqrDistance = pauseDistance * pauseDistance;
		if (testSound.PlayRate > 0.0f) {
			if ( transform.position.sqrMagnitude < sqrDistance ) {
				testSound.Pause ();
			}
		}
		else if ( transform.position.sqrMagnitude >= sqrDistance ) {
				testSound.UnPause();
		}
		
	}
}
}