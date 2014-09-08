using UnityEngine;
using System.Collections;
using icGUIHelpers;

namespace Toolbelt {
public class DebugItemViewer
{	
	private Transform toView;
	private bool GuiEnabled = true;
	private Rect GuiCanvas;
	
	private const string numberFormat = icGUIHelpers.FORMAT.THREE_DP;
		

	private ColourPicker colourPicker; 
	private Vector3Picker globalPosPicker;
	private Vector3Picker localPosPicker;
	private Vector3Picker scalePicker;

	private RotationPicker rotationPicker;
	private AngleAxis rotationState = new AngleAxis();
	//private bool editColour = false;

	private Vector2 ListScrollPos;


	public DebugItemViewer(Rect canvas) {
		this.GuiCanvas = canvas;		
		colourPicker = new ColourPicker("Colour");
		globalPosPicker = new Vector3Picker("Global Pos", -10f, 10f);
		localPosPicker = new Vector3Picker("Local Pos", -10f, 10f);
		scalePicker = new Vector3Picker("Scale", 0.001f, 5f);
		rotationPicker = new RotationPicker("Rotation");

	}

	public void setToView (Transform t)
	{
		this.toView = t;
		if (t != null)
			t.localRotation.ToAngleAxis (out this.rotationState.angle, out this.rotationState.axis);
	}

	public Transform getSelectedTransform ()
	{
		return this.toView;
	}

	public void drawPosition ()
	{
		this.toView.position = globalPosPicker.DrawGUI(this.toView.position);
		this.toView.localPosition = localPosPicker.DrawGUI (this.toView.localPosition);
	}

	public void drawRotation ()
	{		
		this.rotationState = this.rotationPicker.DrawGUI(this.rotationState);
		this.toView.localRotation = Quaternion.AngleAxis(this.rotationState.angle,this.rotationState.axis);			

	}
	
	public void drawScale() 
	{
		this.toView.localScale = this.scalePicker.DrawGUI(this.toView.localScale);
	}
	
	public void drawColour() 
	{
		if (toView.renderer && toView.renderer.material) //check if colour exists.			
		{
			if (!toView.renderer.material.HasProperty("_Color"))
				return;
			Color c;

			icMaterial icMat = toView.GetComponent<icMaterial>();
			if (icMat == null) {
				c = toView.renderer.material.GetColor ("_Color"); 				
			} else {
				c = icMat.icColour;
			}
			
			c = this.colourPicker.DrawGUI(c);
		
			if (icMat != null) {
				icMat.icColour = c;				
			} else {
				toView.renderer.material.SetColor("_Color", c);
			}

		}
	}

	public void DrawTransform() {
		GUILayout.BeginVertical(GUI.skin.box);
		bool isActive = this.toView.gameObject.activeSelf;
		bool isActive2 = icGUIHelpers.icGUI.BoolHeading(isActive,"Transform");
		
		if (isActive != isActive2) {
			this.toView.gameObject.SetActive(isActive2);
		}

		this.drawPosition ();
		this.drawRotation ();
		this.drawScale();
		this.drawColour();
		
		GUILayout.EndVertical();
		
	}

	public void DrawGUI ()
	{
		if (!this.GuiEnabled || this.toView == null) {			
			return;
		}
		
		GUILayout.BeginArea (this.GuiCanvas,GUI.skin.box);
		ListScrollPos = GUILayout.BeginScrollView(ListScrollPos, GUI.skin.scrollView);
		GUI.skin.scrollView.stretchWidth = true;

		GUILayout.BeginVertical ();
		this.DrawTransform();

		MonoBehaviour[] toDraw = toView.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour mb in toDraw) {
			if (mb is icDebugGUI) {
				GUILayout.BeginVertical (GUI.skin.box);			
				mb.enabled = icGUIHelpers.icGUI.BoolHeading(mb.enabled, mb.GetType ().Name);
				if (mb.enabled)
					(mb as icDebugGUI).DrawDebugGUI();
				GUILayout.EndVertical ();
			}
		}
		
		GUILayout.EndVertical ();
		GUILayout.EndScrollView();
		GUILayout.EndArea ();
	}
	
}
}