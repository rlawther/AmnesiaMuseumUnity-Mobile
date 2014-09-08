using UnityEngine;
using System.Collections;

namespace Toolbelt {
namespace CameraInterfaceExample {

	public class CameraInterfaceExample : MonoBehaviour {

		GameObject _viewpointGhost;
		GameObject _avieCylinder;

		/** ToolbeltManager will call .SendMessage("MsgShowGhost", true/false)
		 * 
		 */
		void MsgShowGhost(bool show) {
			if (this._viewpointGhost == null) 
			{
				GameObject vg = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				vg.transform.parent = this.gameObject.transform;
				vg.renderer.material.shader = Shader.Find ("Transparent/Diffuse");
				vg.renderer.material.SetColor("", new Color(1.0f, 1.0f, 1.0f, 0.5f));
				this._viewpointGhost = vg;
			}

			if (this._avieCylinder == null) 
			{
				GameObject ac = new GameObject("Ghost Cylinder");
				ac.AddComponent<MeshRenderer>();
				ac.renderer.material.shader = Shader.Find("Transparent/Diffuse");
				ac.renderer.material.SetColor( "_Emission", new Color(1.0f, 1.0f, 1.0f, 0.2f) );

				icScreenSpaceCyli cyliScript = ac.AddComponent<icScreenSpaceCyli>();
				cyliScript.parentToCamera = false;
				cyliScript.screenSpacePosition.z = 5.0f;
				cyliScript.screenSpaceScale.x = 1.0f;
				cyliScript.screenSpaceScale.y = 1.0f;
				cyliScript.tessellationPerCircle = 60;
				cyliScript.tessellationPerY = 3;
				ac.transform.parent = this.gameObject.transform;

				this._avieCylinder = ac;
			}

			this._viewpointGhost.renderer.enabled = show;
			this._avieCylinder.renderer.enabled = show;
		}
	}

}
}