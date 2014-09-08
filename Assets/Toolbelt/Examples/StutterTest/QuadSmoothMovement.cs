using UnityEngine;
using System.Collections;

public class QuadSmoothMovement : MonoBehaviour {
	
	public float radius = 2.0f;
	protected Vector3 center;
	
	// Use this for initialization
	void Start () {
		this.center = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		float curTime = Time.time;
		
		float x = this.radius * Mathf.Sin(curTime);
		float y = this.radius * Mathf.Cos(curTime);
		
		transform.localPosition = new Vector3(x, y, 0) + center;
	}
}
