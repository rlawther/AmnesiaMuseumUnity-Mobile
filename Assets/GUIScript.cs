using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour 
{

	public GUIText ModeMem,ModeSC,Help,Reset;
	public GUIText [] guiPaths;
	public GUITexture [] guiPathRings;
	public GameObject helpScreen;
	public GameObject camera;
	
	public bool path1on, path2on, path3on, path4on, path5on, path6on;

	public float alpha;

	public Color activeColor;
	
	public Font boldFont;

	private Vector3 cameraOriginalPosition;
	private Quaternion cameraOriginalRotation;

	//private bool helpActive;
	private bool gotParent = false;
	private GameObject photoParent;
	
	private GameObject [] paths;
	
	void findPaths(GameObject parent)
	{
		int i = 0;
		foreach (Transform scenario in parent.transform)
		{
			foreach (Transform episode in scenario.transform)
			{
				paths[i] = episode.gameObject;
				i++;
			}
		}
	}
	
	void togglePathActive(int index)
	{
		Debug.Log ("toggling path " + index);
		if (paths[index].activeSelf)
		{
			paths[index].SetActive(false);
			setAlpha (guiPaths[index], alpha);
			setAlphaTex (guiPathRings[index], 0.0f);
		}
		else
		{
			paths[index].SetActive(true);
			setAlpha (guiPaths[index], 1.0f);
			setAlphaTex (guiPathRings[index], 0.3f);
		}
	}
	
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

		cameraOriginalPosition = camera.transform.position;
		cameraOriginalRotation = camera.transform.rotation;

		paths = new GameObject[6];
	}
	
	void setPathAlphas()
	{
		/*
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
		*/
			
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
		if (button == ModeMem) {
			Debug.Log ("mem");
			setAlpha (ModeMem, 1.0f);
			setAlpha (ModeSC, alpha);
		} else if (button == ModeSC) {
			Debug.Log ("sc");
			setAlpha (ModeMem, alpha);
			setAlpha (ModeSC, 1.0f);
		} else if (button == Help) {
			//helpActive = !helpActive;
			//Debug.Log ("help screen " + helpActive);
			Debug.Log ("help screen ");
			helpScreen.SetActive (!helpScreen.activeSelf);
			setAlpha (ModeSC, 1.0f);
		} else if (button == Reset) {
			//helpActive = !helpActive;
			//Debug.Log ("help screen " + helpActive);
			Debug.Log ("reset ");
			setAlpha (ModeSC, 1.0f);
			camera.transform.position = cameraOriginalPosition;
			camera.transform.rotation = cameraOriginalRotation;
		} else 
		{
			for (int i = 0; i < 6; i++)
			{
				if (button == guiPaths [i]) {
					togglePathActive(i);
				}
			}
		}
		
		/*
		else if (button == path1)
		{
			path1on = !path1on;
			setPathAlphas();
			togglePathActive(0);
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
		*/
		
	}




	void Update() 
	{
		if (!gotParent && (Time.time > 1.0))
		{
			GameObject photoParent = GameObject.Find ("Photos");
			findPaths (photoParent);
			
			gotParent = true;
		}
		
	}
	
}