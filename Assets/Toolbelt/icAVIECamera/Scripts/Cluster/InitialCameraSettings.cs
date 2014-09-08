using UnityEngine;
using System.Collections;
using Toolbelt;

public class InitialCameraSettings : MonoBehaviour {

	// Sets HSL/BCG of all children. Useful for warpmeshes that are children of this transform.
	public Vector3 HSL = new Vector3(0f,1f,1f);
	public Vector3 BrightnessContrastGamma = new Vector3(0.5f,0.5f,1.0f);
	public void ExecuteInitialCameraSettings() {
		foreach (ImageShaderScript iss in this.GetComponentsInChildren<ImageShaderScript>()) {
			iss.HSL = this.HSL;
		}
		foreach (GammaLUT glut in this.GetComponentsInChildren<GammaLUT>()) {
			glut.brightness= BrightnessContrastGamma.x;
			glut.contrast= BrightnessContrastGamma.y;
			glut.gamma= BrightnessContrastGamma.z;
		}
	}
}
