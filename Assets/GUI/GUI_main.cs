
// C#
using UnityEngine;
using System.Collections;

public class GUI_main : MonoBehaviour {
	
	public GUIStyle StyleLabel, StyleMode, StyleSceneA, StyleSceneB, StyleReset;
	public Texture2D black_75percent_2k;

	private int sw = Screen.width/64;
	private int sh = Screen.height/64;

	private bool memToggle = true; //memory mode
	private bool scToggle = false; //sc-senseCam mode
	private bool sceneAToggle = true; //scenario A
	private bool sceneBToggle = false; //scenario B
	private bool helpMode = false;

	void OnGUI () {

		//l = GameObject.Find("Directional light");

		GUI.Label (new Rect (0,0, Screen.width , sw*2), black_75percent_2k);
		//GUI.Box (new Rect (0,0, sw*64 , sw*2.5), black_75percent_2k);

		GUI.Label (new Rect (sw/2,sw/2, sw*4, sw*2), "Mode: ", StyleLabel);

		if (memToggle = GUI.Toggle (new Rect (sw*8,sw/2,sw*8,sw*2), memToggle, "Memory", StyleMode)) {
			memToggle = true;
			scToggle = false;
			print ("Memory Mode");
		}
		if (scToggle = GUI.Toggle (new Rect (sw*16,sw/2,sw*8,sw*2), scToggle, "SenseCam", StyleMode)) {
			memToggle = true;
			scToggle = false;
			print ("SenseCam Mode");
		}

		GUI.Label (new Rect (sw*32,sw/2,sw*8,sw*2), "Scenario: ", StyleLabel);

		if (sceneAToggle = GUI.Toggle (new Rect (sw*40,sw/2,sw*4,sw*2), sceneAToggle, "A", StyleSceneA)) {
			sceneAToggle = true;
			sceneBToggle = false;
			print ("Scenario A");
		}
		if (sceneBToggle = GUI.Toggle (new Rect (sw*44,sw/2,sw*4,sw*2), sceneBToggle, "B", StyleSceneB)) {
			sceneAToggle = false;
			sceneBToggle = true;
			print ("Scenario B");
		}

		if (GUI.Button (new Rect (sw*62,sw/2,sw*6,sw*2), "Help", StyleMode)) {
			print ("Help");
		}

		if (GUI.Button (new Rect (sw*62,sh*60,sw*6,sw*2), "reset view", StyleReset)) {
			print ("Help");
		}
	}
	
}

