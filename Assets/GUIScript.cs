using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour 
{

	public GUIText ModeMem,ModeSC,ScenA,ScenB,Help,Reset;
	public GUIText path1,path2,path3,path4,path5,path6;
	public GameObject helpScreen;
	
	public bool path1on, path2on, path3on, path4on, path5on, path6on;

	public float alpha;

	public Color activeColor;
	
	public Font boldFont;

	//private bool helpActive;

	void Start() 
	{
		setAlpha(ModeMem, alpha);
		setAlpha(ScenB, alpha);
		//helpActive = false;
		path1on = true;
		path2on = false;
		path3on = false;
		path4on = false;
		path5on = false;
		path6on = false;
		setPathAlphas();
	}
	
	void setPathAlphas()
	{
		if (path1on)
			setAlpha (path1, 1.0f);
		else
			setAlpha (path1, alpha);
			
		if (path2on)
			setAlpha (path2, 1.0f);
		else
			setAlpha (path2, alpha);
			
		if (path3on)
			setAlpha (path3, 1.0f);
		else
			setAlpha (path3, alpha);
			
		if (path4on)
			setAlpha (path4, 1.0f);
		else
			setAlpha (path4, alpha);
			
		if (path5on)
			setAlpha (path5, 1.0f);
		else
			setAlpha (path5, alpha);
			
		if (path6on)
			setAlpha (path6, 1.0f);
		else
			setAlpha (path6, alpha);
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
		else if (button == path1)
		{
			path1on = !path1on;
			setPathAlphas();
		}
		else if (button == path2)
		{
			path2on = !path2on;
			setPathAlphas();
		}
		else if (button == path3)
		{
			path3on = !path3on;
			setPathAlphas();
		}
		else if (button == path4)
		{
			path4on = !path4on;
			setPathAlphas();
		}
		else if (button == path5)
		{
			path5on = !path5on;
			setPathAlphas();
		}
		else if (button == path6)
		{
			path6on = !path6on;
			setPathAlphas();
		}
		
	}




	void Update() 
	{


	}
	
}