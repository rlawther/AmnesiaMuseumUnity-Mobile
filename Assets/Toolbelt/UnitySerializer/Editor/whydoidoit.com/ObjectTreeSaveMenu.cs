using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
public class ObjectTreeSaveMenu : EditorWindow {
	private string filePath = "";
	private string currentError = "";
	private Vector2 scrollPos = new Vector2(0f,0f);
	
	[MenuItem("Window/icSaveGame")]
	static void Init ()
	{
		ObjectTreeSaveMenu window = EditorWindow.GetWindow<ObjectTreeSaveMenu> (false, "icSaveGame Loader");
		window.autoRepaintOnSceneChange = true;
		window.filePath = Path.GetFullPath(Application.dataPath + "./../");
		window.Show ();
	}
	private string getFolder(string path) {
		return Path.GetDirectoryName(path);
		
	}
	private string getFile(string path) {
			return Path.GetFileName(path);
	}

	void OnGUI() {
	
		EditorGUILayout.BeginVertical();
		this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos);
		GUILayout.Label ("Load game", EditorStyles.boldLabel);
		
		this.filePath = GUILayout.TextField(this.filePath);
		if (GUILayout.Button("Reset File Path")){		
			this.filePath = Path.GetFullPath(Application.dataPath + "./../");
		}		
		
		GUILayout.Label(this.currentError);
		if (GUILayout.Button ("Load object tree!")) {
			try {
				
				LevelSerializer.LoadObjectTreeFromFile(this.filePath,this.LoadedObjectTreeCallback);					
				
				this.currentError = "Game Loaded";				
			} catch (Exception e) {
				this.currentError = e.Message;
			}
		}
		GUILayout.Space(10);
		
		string folder = "";
		string currentFile = "";
		string[] allFiles = new string[] {};
		
		try {
			folder = this.getFolder (this.filePath);
			currentFile = this.getFile(this.filePath);			
			allFiles = Directory.GetFiles (folder);
			this.currentError = "";
		} catch (Exception e) { //TODO FIX
			this.currentError = e.Message;			
		}
		
		
		foreach (string fullFilename in allFiles) {
			string filename = System.IO.Path.GetFileName(fullFilename);			
			
			if (!filename.Contains(currentFile)) { //skip files that don't match search criteria
				continue;
			}
			if (GUILayout.Button (filename)) {
				this.filePath = folder + Path.DirectorySeparatorChar + filename;
			}
		}
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		
	}
	
	public void LoadedObjectTreeCallback(LevelLoader ll) {
		//remove all the uniqueidentifiers and Storematerials that are in the saved objects.
		GameObject newRoot = ll.rootObject;		
		UniqueIdentifier[] uis = newRoot.GetAllComponentsInChildren<UniqueIdentifier>();
		foreach (UniqueIdentifier ui in uis) {
			GameObject.DestroyImmediate(ui);
		}
		
		StoreMaterials[] sms = newRoot.GetAllComponentsInChildren<StoreMaterials>();
		foreach (StoreMaterials sm in sms) {
			GameObject.DestroyImmediate (sm);
		}		
		GameObject.DestroyImmediate(ll.gameObject);
		
		//also destroy the save game manager
		GameObject sgm = GameObject.Find("Save Game Manager");
		if (sgm != null) {
			if (sgm.GetComponent<SaveGameManager>() != null) {
				GameObject.DestroyImmediate(sgm);
			}
		}
	}
	
	
}

#endif