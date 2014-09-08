using UnityEngine;
using System.Collections;
namespace Toolbelt {
/*** Base class for anything that needs to run at the end of an icMaterial Update()
 * Can also run on it's own if no icMaterial present
 * See ImageShaderScript for example of how it is used
 */
abstract public class icRenderPass : MonoBehaviour {

	protected icMaterial icMat;
	//Call this whenever you change the icMaterial on this gameobject.
	//E.g. when changing from an icImageMaterial to a icBinkMaterial
	//Will pick the first enabled icMaterial. If all are disabled, will pick the first one.
	public void RefreshIcMaterial() {
		if (icMat != null) {
			icMat.RemoveRenderPass(this);
		}
		this.icMat = null;
		icMaterial[] icMats = this.GetComponents<icMaterial>();
		
		if (icMats.Length > 0) {
			for (int i = 0; i < icMats.Length; i++) {
				icMaterial mat = icMats[i];
				if (mat.enabled) {
					this.icMat = mat;
					break;
				}
				
			}
			if (this.icMat == null) {
				this.icMat = icMats[0];
			}
			
			this.icMat.AddRenderPass(this);
		} else {
			Debug.LogWarning ("icRenderPass was added to a GameObject with no icMaterial");
		}
	
	}
	
	public void SetIcMaterial(icMaterial icm) {
		this.icMat = icm;		
	}
	
	void Awake() 
	{
		this.RefreshIcMaterial();
		DoAwake();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (this.icMat == null) {
			this.RenderPass();
		}
	}
	void OnDestroy() {
		if (this.icMat != null) {
			this.icMat.RemoveRenderPass(this);
		}
	}
	protected abstract void DoAwake();
	public abstract void RenderPass();
}
}