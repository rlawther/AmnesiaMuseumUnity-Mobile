using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class CameraEyeCallback : MonoBehaviour {

	public StereoCameraInterface stereoCameraInterface;
	public StereoMode stereoMode;
	
	void OnPreRender() {
		this.stereoCameraInterface.SetCurrentEye(this.stereoMode);
	}
}
}