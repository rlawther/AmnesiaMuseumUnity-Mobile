using UnityEngine;
using System.Collections;

namespace Toolbelt {
/***
 * When placed on a icStereoObject, this will copy the ImageShaderScript parameters set here
 * to the ImageShaderScripts of the left and right eye objects.
 * 
 * Subclassing ImageShaderScript allows this GameObject to be treated like any other
 * GameObject with an ImageShaderScript
 */
public class StereoImageShaderScript : ImageShaderScript 
{
	public icStereoObjectMaterial stereoObject;

	void Awake() {

	}

	void Start() {
		this.stereoObject = this.GetComponent<icStereoObjectMaterial>();
	}

	void Update()
	{
		if (this.stereoObject) {
			if (this.stereoObject.leftObject) {
				UpdateChild(this.stereoObject.leftObject);
			}

			if (this.stereoObject.rightObject) {
				UpdateChild(this.stereoObject.rightObject);
			}
		}
	}

	void UpdateChild(GameObject go)
	{
		ImageShaderScript s = go.GetComponent<ImageShaderScript>();
		if (s)
		{
			s.HSL = this.HSL;
		}
	}

}
}