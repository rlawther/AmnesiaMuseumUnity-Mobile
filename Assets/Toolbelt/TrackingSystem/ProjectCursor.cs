using UnityEngine;
using System.Collections;

public class ProjectCursor : MonoBehaviour {

	public GameObject cursorObject;
	public GameObject cursorDisplay;
	public bool projectFromCentre = true;
	public Vector3 armCentre = new Vector3(0.0f, 1.0f, 0.0f);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hitInfo;
		Vector3 cursorPos;

		if (projectFromCentre)
			cursorPos = armCentre;
		else
			cursorPos = cursorObject.transform.position;

		Vector3 cursorVec = cursorObject.transform.rotation * Vector3.forward;

		//Vector3 fwd = transform.TransformDirection(Vector3.forward);
		if (Physics.Raycast (cursorPos, cursorVec, out hitInfo, 10))
		{
			print ("There is something in front of the object!" + hitInfo.point);
			cursorDisplay.transform.position = hitInfo.point;
		}

	}
}
