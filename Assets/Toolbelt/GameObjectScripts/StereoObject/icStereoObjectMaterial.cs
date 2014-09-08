using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Toolbelt {
public class icStereoObjectMaterial : icMaterial 
{
	public GameObject leftObject;
	public GameObject rightObject;

	private icMaterial leftIcMat;
	private icMaterial rightIcMat;

	private StereoMediaReady leftReadyScript;
	private StereoMediaReady rightReadyScript;

	void Start()
	{
		SetStereoMode(leftObject, StereoMode.LEFT);
		SetStereoMode(rightObject, StereoMode.RIGHT);
		leftObject.transform.parent = this.transform;
		rightObject.transform.parent = this.transform;
		this.leftIcMat = this.leftObject.GetComponent<icMaterial>();
		this.rightIcMat = this.rightObject.GetComponent<icMaterial>();
		UpdateStereoChild(this.leftIcMat);
		UpdateStereoChild(this.rightIcMat);

		LinkMediaReadies();
	}

	protected void LinkMediaReadies()
	{
		if (mediaReadyCallbacks.Count > 0) {
			this.leftReadyScript = LinkSingleMediaReady(this.leftIcMat);
			this.rightReadyScript = LinkSingleMediaReady(this.rightIcMat);
		}
	}

	protected StereoMediaReady LinkSingleMediaReady(icMaterial stereoChildMat) 
	{
		if (stereoChildMat) {
			StereoMediaReady mrScript = this.gameObject.AddComponent<StereoMediaReady>();
			mrScript.enabled = false;
			stereoChildMat.mediaReadyCallbacks.Add(mrScript);
			return mrScript;
		} else {
			return null;
		}
	}

	protected override void DoUpdate ()
	{
		if (this.leftReadyScript && this.rightReadyScript) {
			if (this.leftReadyScript.ready && this.rightReadyScript.ready) {
				Destroy(this.leftReadyScript);
				Destroy(this.rightReadyScript);
				this.leftReadyScript = null;
				this.rightReadyScript = null;
				AllMediaReady();
			}
		}
	}

	void UpdateStereoChild(icMaterial icMat)
	{
		if (icMat) {
			icMat.baseScale = this.baseScale;
			icMat.autoAspectRatio = this.autoAspectRatio;
			icMat.keepStartingAspectRatio = this.keepStartingAspectRatio;
		}
	}

	void SetStereoMode(GameObject go, StereoMode sm)
	{
		icMaterial icMat = go.GetComponent<icMaterial>();
		if (icMat) {
			icMat.stereoMode = sm;

			// Always true so we can adjust colour from the stereo parent
			icMat.multiplyByParentColour = true;
		} else {
			int layerNum = ToolbeltManager.FirstInstance.GetStereoLayer(sm);
			go.layer = layerNum;
		}
	}

	public override void SetBaseScale (float bs)
	{
		// Should do nothing for a StereoObject
	}
	
}
}