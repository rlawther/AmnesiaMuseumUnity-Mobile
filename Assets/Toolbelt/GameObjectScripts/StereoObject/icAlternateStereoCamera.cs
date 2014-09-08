using UnityEngine;
using System.Collections;

namespace Toolbelt {
/***
 * Attach to camera to flick the culling mask once per frame between 
 * only rendering the left and right eyes
 */
public class icAlternateStereoCamera : MonoBehaviour 
{
	public StereoMode curStereo = StereoMode.LEFT;
	
	void FixedUpdate () 
	{
		this.SwitchEyes();
	}

	void OnPreRender()
	{
		//this.SwitchEyes();
	}

	void SwitchEyes()
	{
		ToolbeltManager tm = ToolbeltManager.FirstInstance;
		
		// Remove previous eye layer from culling mask
		this.camera.cullingMask = this.camera.cullingMask & ~(1 << tm.GetStereoLayer(this.curStereo));
		
		this.curStereo = (this.curStereo == StereoMode.LEFT) ? StereoMode.RIGHT : StereoMode.LEFT;
		
		this.camera.cullingMask = this.camera.cullingMask | (1 << tm.GetStereoLayer(this.curStereo));
		//Debug.Log(Time.frameCount.ToString() + " " + this.camera.cullingMask.ToString());
	}
}
}