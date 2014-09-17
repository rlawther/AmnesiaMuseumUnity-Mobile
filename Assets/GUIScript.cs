using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour 
{

	public GUIText ModeMem,ModeSC,Help,Reset;
	public GUIText path1,path2,path3,path4,path5,path6;
	public GUITexture path1Ring,path2Ring,path3Ring,path4Ring,path5Ring,path6Ring;
	public GameObject helpScreen;
	
	public bool path1on, path2on, path3on, path4on, path5on, path6on;

	public float alpha;

	public Color activeColor;
	
	public Font boldFont;

	//private bool helpActive;

	void Start() 
	{
		setAlpha(ModeMem, alpha);
		setAlpha(Help, alpha);
		setAlpha(Reset, alpha);
		//helpActive = false;
		path1on = true;
		path2on = true;
		path3on = true;
		path4on = true;
		path5on = true;
		path6on = true;
		setPathAlphas();
	}
	
	void setPathAlphas()
	{
				if (path1on) {
						setAlpha (path1, 1.0f);
						setAlphaTex (path1Ring, 0.3f);
				} else {
						setAlpha (path1, alpha);
						setAlphaTex (path1Ring, 0.0f);
				}
				if (path2on) {
						setAlpha (path2, 1.0f);
						setAlphaTex (path2Ring, 0.3f);
				} else {
						setAlpha (path2, alpha);
						setAlphaTex (path2Ring, 0.0f);
				}
				if (path3on) {
						setAlpha (path3, 1.0f);
						setAlphaTex (path3Ring, 0.3f);
				} else {
						setAlpha (path3, alpha);
						setAlphaTex (path3Ring, 0.0f);
				}

				if (path4on) {
						setAlpha (path4, 1.0f);
						setAlphaTex (path4Ring, 0.3f);
				} else {
						setAlpha (path4, alpha);
						setAlphaTex (path4Ring, 0.0f);
				}

				if (path5on) {
						setAlpha (path5, 1.0f);
						setAlphaTex (path5Ring, 0.3f);
				} else {
						setAlpha (path5, alpha);
						setAlphaTex (path5Ring, 0.0f);
				}

				if (path6on) {
						setAlpha (path6, 1.0f);
						setAlphaTex (path6Ring, 0.3f);
				} else {
						setAlpha (path6, alpha);
						setAlphaTex (path6Ring, 0.0f);
		}
			
	}

	void setAlpha(GUIText text, float newAlpha)
	{
		Color col;
		col = text.color;
		col.a = newAlpha;
		text.color = col;

	}

	void setAlphaTex(GUITexture tex, float newAlpha)
	{
		Color col;
		col = tex.color;
		col.a = newAlpha;
		tex.color = col;
		
	}

	public void buttonReleased(GUIText button)
	{
		if (button == Help) 
		{
			Debug.Log ("no help screen ");
			//helpScreen.SetActive(!helpScreen.activeSelf);
			setAlpha (Help, alpha);
		} 
		else if (button == Reset) 
		{
			Debug.Log ("no reset ");
			setAlpha (Reset, alpha);
		}

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

		else if (button == Help)
		{
			//helpActive = !helpActive;
			//Debug.Log ("help screen " + helpActive);
			Debug.Log ("help screen ");
			helpScreen.SetActive(!helpScreen.activeSelf);
			setAlpha(ModeSC, 1.0f);
		} 

		else if (button == Reset)
		{
			//helpActive = !helpActive;
			//Debug.Log ("help screen " + helpActive);
			Debug.Log ("reset ");
			setAlpha(ModeSC, 1.0f);

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