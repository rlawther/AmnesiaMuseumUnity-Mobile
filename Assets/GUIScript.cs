using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour 
{

	public GUIText ModeMem,ModeSC,ScenA,ScenB,Help,Reset;
	public GameObject helpScreen;

	public float alpha;

	public Color activeColor;
	
	public Font boldFont;

	//private bool helpActive;

	void Start() 
	{
		setAlpha(ModeMem, alpha);
		setAlpha(ScenB, alpha);
		//helpActive = false;
	}

	void setAlpha(GUIText text, float newAlpha)
	{
		Color col;
		col = text.color;
		col.a = newAlpha;
		text.color = col;

	}

	public void buttonPressed(GUIText button)
	{
		if (button == ModeMem)
		{
			Debug.Log ("mem");
			setAlpha(ModeMem, 1.0f);
			setAlpha(ModeSC, alpha);
		} 
		else if (button == ModeSC)
		{
			Debug.Log ("sc");
			setAlpha(ModeMem, alpha);
			setAlpha(ModeSC, 1.0f);
		} 
		else if (button == ScenA)
		{
			Debug.Log ("sc a");
			setAlpha(ScenB, alpha);
			setAlpha(ScenA, 1.0f);
			ScenA.fontSize = 80;
			ScenB.fontSize = 50;
		} 
		else if (button == ScenB)
		{
			Debug.Log ("sc b");
			setAlpha(ScenB, 1.0f);
			setAlpha(ScenA, alpha);
			ScenB.fontSize = 80;
			ScenA.fontSize = 50;
		} 
		else if (button == Help)
		{
			//helpActive = !helpActive;
			//Debug.Log ("help screen " + helpActive);
			helpScreen.SetActive(!helpScreen.activeSelf);
		} 

	}




	void Update() 
	{


	}
	
}