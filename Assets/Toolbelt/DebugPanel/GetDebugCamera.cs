using UnityEngine;
using System.Collections;

namespace Toolbelt {
public abstract class GetDebugCamera : MonoBehaviour {
	//returns a list of gameobjects that you can adjust hsv/gamma of for global effects.
	public abstract GameObject[] getDebugPanelCameras();
}
}