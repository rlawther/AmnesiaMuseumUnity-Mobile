using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class PlayOgg : MonoBehaviour {
	public bool pause = false;
	public float whenToPause = 3.0f;
	// Use this for initialization
	IEnumerator Start () {
		MovieTexture mt = this.renderer.material.mainTexture as MovieTexture;
		mt.Play();
		if (this.pause) {
			yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(this.whenToPause);
			
			if (mt.isPlaying && this.pause) { //check this.pause in case it got disabled during the x second wait.
				mt.Pause();		
				this.pause = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
}