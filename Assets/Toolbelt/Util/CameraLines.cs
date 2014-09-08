using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class CameraLines : MonoBehaviour {

	void OnPreRender() {
		GL.wireframe = true;	
	}
	void OnPostRender() {
		GL.wireframe = false;
	}
}
}