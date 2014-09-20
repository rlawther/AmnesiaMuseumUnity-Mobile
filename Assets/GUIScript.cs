using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour 
{

	public GUIText ModeMem,ModeSC,Help,Reset;
	public GUIText [] guiPaths;
	public GUITexture [] guiPathRings;
	public GameObject helpScreen;
	public GameObject camera;

	public float alpha;

	public Color activeColor;
	
	public Font boldFont;

	private Vector3 cameraOriginalPosition;
	private Quaternion cameraOriginalRotation;

	//private bool helpActive;
	private bool gotParent = false;
	private GameObject photoParent;
	
	private GameObject [] paths;

	private BSONComms bsonComms;

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
			setPathActive(index, false);
		}
		else
		{
			setPathActive(index, true);
		}
	}

	void setPathActive(int index, bool active)
	{
		Debug.Log ("setActive path " + index + " to " + active);
		paths[index].SetActive(active);

		if (!active)
		{
			setAlpha (guiPaths[index], alpha);
			setAlphaTex (guiPathRings[index], 0.0f);
			bsonComms.addData ("path" + (index + 1), 0);
		}
		else
		{
			setAlpha (guiPaths[index], 1.0f);
			setAlphaTex (guiPathRings[index], 0.3f);
			bsonComms.addData ("path" + (index + 1), 1);
		}
	}

	void Start() 
	{
		setAlpha(ModeMem, alpha);
		setAlpha(Help, alpha);
		setAlpha(Reset, alpha);

		cameraOriginalPosition = camera.transform.position;
		cameraOriginalRotation = camera.transform.rotation;

		paths = new GameObject[6];
		bsonComms = GameObject.Find ("Scripts").GetComponent<BSONComms> ();

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
			for (int i = 0; i < 6; i++)
			{
				setPathActive(i, true);
			}
			bsonComms.addData("reset", 1);

		} 
		else 
		{
			/* Check each of the "path" buttons */
			for (int i = 0; i < 6; i++)
			{
				if (button == guiPaths [i]) {
					togglePathActive(i);
				}
			}
		}
		
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