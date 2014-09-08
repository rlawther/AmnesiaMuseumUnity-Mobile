using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class PlayBink : MonoBehaviour, icDebugGUI {
	public float speed = 1.0f;
	public bool spaceActivate = false;
	// Use this for initialization
	void Start () {
		if (!spaceActivate) {
			icBinkMaterial bm = this.GetComponent<icBinkMaterial>();
			if (bm != null)
				bm.getBinkPlayOptions().movieSpeed = speed;
			else
				Debug.Log ("null");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space") && this.spaceActivate) {
			icBinkMaterial bm = this.GetComponent<icBinkMaterial>();
			if (bm != null)	{
				if (bm.getBinkPlayOptions().movieSpeed == speed) {
					bm.getBinkPlayOptions().movieSpeed = 0.0f;
				} else {
					bm.getBinkPlayOptions().movieSpeed = speed;
				}
			}
		}
	}
	
	public void DrawDebugGUI() {
		GUILayout.BeginVertical();
		GUILayout.Label("MovieSpeed on pressing spacebar: " + this.speed.ToString(icGUIHelpers.FORMAT.THREE_DP));
		this.speed = GUILayout.HorizontalSlider(this.speed, 0f,2f);
		GUILayout.EndVertical();
	}
	
}
}