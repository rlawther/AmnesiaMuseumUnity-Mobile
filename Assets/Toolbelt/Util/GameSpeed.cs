using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class GameSpeed : MonoBehaviour, icDebugGUI {
	public float gameSpeed = 1f;
	private float _gameSpeed = 1f;
	private icGUIHelpers.FloatPicker picker;
	// Use this for initialization
	void Start () {
		this.picker = new icGUIHelpers.FloatPicker("GameSpeed", 0f, 3f);
	}
	
	// Update is called once per frame
	void Update () {
		if (this._gameSpeed != this.gameSpeed) {
			this._gameSpeed = this.gameSpeed;
			Time.timeScale = this.gameSpeed;
		}
	}
	
	public void DrawDebugGUI() {
		GUILayout.BeginVertical();
		this.gameSpeed = this.picker.DrawGUI(this.gameSpeed);
		GUILayout.EndVertical();
	}
}
}