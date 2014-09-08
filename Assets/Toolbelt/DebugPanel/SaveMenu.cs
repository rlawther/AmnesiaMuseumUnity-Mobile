using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Toolbelt {
public class SaveMenu : DebugTabMenu {
	protected bool paused = false;
	public string gameName = "Your Game";	
	protected float oldTimeScale = 1.0f;
	public GameObject toSave;
	public string filePath = "";
	protected string currentError = "";
	
	protected string lastLoaded = "";
	public Rect canvas;
	protected Action<GameObject> refreshRoot;
	private Vector2 scrollPos = new Vector2(0,0);
	// Use this for initialization
	public SaveMenu(Rect canvas, GameObject root,Action<GameObject> refreshRoot) {
		this.canvas = canvas;
		this.toSave = root;
		this.refreshRoot = refreshRoot;
		this.filePath = Path.GetFullPath(Application.dataPath + "./../");
		
	}
	
	void DrawPauseMenu() 
	{		
		
		GUILayout.BeginArea(new Rect(this.canvas.xMin, 
		                             this.canvas.yMin,
		                             this.canvas.width,
		                             this.canvas.height),
		                    GUI.skin.box);
		GUILayout.BeginVertical ();	
		this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);
		this.filePath = Path.GetFullPath(GUILayout.TextField(this.filePath));
		GUILayout.Label(this.currentError);
		if (GUILayout.Button ("Save Game")) 
		{
			try {
				LevelSerializer.SaveObjectTreeToFile(this.filePath,this.toSave);
				this.currentError = "Game Saved";
			} catch (UnauthorizedAccessException e) {
				this.currentError = e.Message;
			} finally { 
				this.DestroyMetadata(this.toSave);
			}
		}
		
		
		GUILayout.Space(60);
		if (GUILayout.Button ("Load Game"))
		{
			if (this.lastLoaded == this.filePath) {
				this.currentError = "Cannot load same game twice in a row.";
			} else {
				try {
					
					LevelSerializer.LoadObjectTreeFromFile(this.filePath,this.LoadedObjectTreeCallback);					

					this.currentError = "Game Loaded";
					this.lastLoaded = this.filePath;
				} catch (Exception e) {
					this.currentError = e.Message;
				}
			}
		}
				
		GUILayout.FlexibleSpace();
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	public void LoadedObjectTreeCallback(LevelLoader ll) {
		GameObject newRoot = ll.rootObject;		
		this.DestroyMetadata(newRoot);
		if (this.refreshRoot != null) {
			this.refreshRoot(newRoot);
		}
	}
	
	private void DestroyMetadata(GameObject newRoot) {
		//also destroy the save game manager
		GameObject sgm = GameObject.Find("Save Game Manager");
		if (sgm != null) {
			if (sgm.GetComponent<SaveGameManager>() != null) {
				GameObject.DestroyImmediate(sgm);
	    	}
	    }
		UniqueIdentifier[] uis = newRoot.GetAllComponentsInChildren<UniqueIdentifier>();
		foreach (UniqueIdentifier ui in uis) {
			GameObject.Destroy(ui);
		}
		
		StoreMaterials[] sms = newRoot.GetAllComponentsInChildren<StoreMaterials>();
		foreach (StoreMaterials sm in sms) {
			GameObject.Destroy (sm);
		}
	}
	public override void OnSelect() {
		//call this when this tab is selected.
		this.oldTimeScale = Time.timeScale;
		Time.timeScale = 0.0f;
	}
	public override void OnUnselect() {
		//call this when tab is unselected...
		Time.timeScale = this.oldTimeScale;
	}
	public override void DrawGUI() {
		
		this.DrawPauseMenu();

	}
	
	
	
}
}