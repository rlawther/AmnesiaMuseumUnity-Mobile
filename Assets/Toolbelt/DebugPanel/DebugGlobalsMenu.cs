using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using icGUIHelpers;


namespace Toolbelt {
public class DebugGlobalsMenu : DebugTabMenu
{
	//private bool editColour = false;	
	private Rect canvas;
	private const string numberFormat = FORMAT.THREE_DP;
	private Vector2 ListScrollPos;

	
	private GetDebugCamera getDebugCamera;
	private GameObject[] myCameras;
	private List<icDebugGUI> otherGlobals = new List<icDebugGUI>();
	
	private ColourPicker ambientLightColour = new ColourPicker( "Ambient Light",false,true);
	private ColourPicker hsvColour = new ColourPicker("Hue, Saturation, Value",false,false);
	// Use this for initialization

	public DebugGlobalsMenu (Rect canvas, GetDebugCamera getDebugCamera, GameObject otherGlobalsGameObject)
	{		
		this.canvas = canvas;
		this.getDebugCamera = getDebugCamera;
		
		if (otherGlobalsGameObject != null)
		{
			MonoBehaviour[] potentials = otherGlobalsGameObject.GetComponents<MonoBehaviour>();			
			foreach (MonoBehaviour mb in potentials) 
			{						
				if (mb == null) continue;
				
				icDebugGUI toAdd = mb as icDebugGUI; //returns null if monobehaviour is not an icDebugGUI
				if (toAdd != null)				
				{
					otherGlobals.Add(toAdd);					
				}
			}
		}		
	}

	public void doDrawGUI() {
		if (myCameras == null) {
			myCameras = getDebugCamera.getDebugPanelCameras();			
		}
		GameObject mc;
		if (myCameras.Length == 0) {
			mc = null;
		} else {
			mc = myCameras[0];
		}
		
		FullScreenEffects fSFX = null;
		if (mc != null)
			fSFX = mc.GetComponent<FullScreenEffects>();

		//#########################################################
		//# Refresh Camera button                                 #
		//#########################################################
		
		if (GUILayout.Button("Refresh Camera Ref")){
			myCameras = getDebugCamera.getDebugPanelCameras();
		}
    	
    			
		//#########################################################
		//# Ambient Light Colour Picker                           #
		//#########################################################
		Color ambiColour = RenderSettings.ambientLight;
		ambiColour = ambientLightColour.DrawGUI(ambiColour);
		RenderSettings.ambientLight = ambiColour;
		
		
		//#####################################################
		//# Gamma, brightness, contrast                       #
		//#####################################################
    	GammaLUT glut = mc.GetComponent<GammaLUT>();
		if (glut != null && glut.enabled) {
			glut.DrawDebugGUI();
		} else {
			GUILayout.Label ("Main Camera does not have \nGammaLUT attached/enabled");
    	}
		
		//#####################################################
		//# Hue, Saturation, Value                            #
		//#####################################################
		ImageShaderScript iss = mc.GetComponent<ImageShaderScript>();
		if (iss != null) {
			iss.DrawDebugGUI();
		} else if (fSFX != null && fSFX.enabled) {

			if (fSFX.mat != null && fSFX.mat.HasProperty("_HsvAdjust")) {
				Color hsv = fSFX.mat.GetColor("_HsvAdjust");
				hsv = hsvColour.DrawGUI (hsv);
				fSFX.mat.SetColor("_HsvAdjust",hsv);

			} else {
				GUILayout.Label("Warning: No _HsvAdjust in camera's shader");
			}
		}
		//#########################################################
		//# Grid, blend                                           #
		//#########################################################
		foreach (icDebugGUI icdg in otherGlobals) {	
			try {		
				icdg.DrawDebugGUI();
			} catch (NullReferenceException) {
				//icdg script is disabled
			}
		}
		
		this.propagateCameraChanges();
	}
	
	private void propagateCameraChanges() {
		if (this.myCameras.Length <= 1) return;
		
		GameObject original = this.myCameras[0];
		GammaLUT glut = original.GetComponent<GammaLUT>();
		ImageShaderScript iss = original.GetComponent<ImageShaderScript>();
		FullScreenEffects fSFX = original.GetComponent<FullScreenEffects>();
		for (int i = 1; i < this.myCameras.Length; i++) {
			GameObject other = this.myCameras[i];
			if (glut != null) {
				GammaLUT glut2 = other.GetComponent<GammaLUT>();
				glut2.brightness = glut.brightness;
				glut2.contrast = glut.contrast;				
				glut2.gamma = glut.gamma;				
			}
			if (iss != null) {
				ImageShaderScript iss2 = other.GetComponent<ImageShaderScript>(); 
				iss2.HSL = iss.HSL;				
			} else if (fSFX != null) {
				Color hsv = fSFX.mat.GetColor("_HsvAdjust");
				FullScreenEffects fSFX2 = other.GetComponent<FullScreenEffects>();
				fSFX2.mat.SetColor("_HsvAdjust",hsv);
			}
		}
	}
	public override void DrawGUI ()
	{
		GUILayout.BeginArea (this.canvas,GUI.skin.box);
		ListScrollPos = GUILayout.BeginScrollView(ListScrollPos, GUI.skin.scrollView);
		
		GUILayout.BeginVertical ();
		
		this.doDrawGUI();
				
		GUILayout.EndVertical ();
		GUILayout.EndScrollView();
		GUILayout.EndArea ();

	}
	
}
}