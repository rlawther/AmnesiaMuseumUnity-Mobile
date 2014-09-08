using UnityEngine;
using System.Collections;

public class PositionClamper : MonoBehaviour {
	public Vector3 topClamp = new Vector3(5f,50f,5f);
	public Vector3 bottomClamp = new Vector3(-5f,-50f,-5f);
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = this.transform.position;
		pos.x = Mathf.Clamp(pos.x,bottomClamp.x,topClamp.x);
		pos.y = Mathf.Clamp(pos.y,bottomClamp.y,topClamp.y);
		pos.z = Mathf.Clamp(pos.z,bottomClamp.z,topClamp.z);
		this.transform.position = pos;
	}
}
