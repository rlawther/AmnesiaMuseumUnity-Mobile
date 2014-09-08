using UnityEngine;
using System.Collections;

using Toolbelt;

public class RotateAroundAvie : MonoBehaviour {
	public float speed = 1f;
	public bool random = false;
	// Use this for initialization
	void Start () {
		if (random) {
			this.speed *= Random.value;
		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 currentPos = Euclidizer.EuclidToAvie(this.transform.position);
		currentPos.x += speed * Time.deltaTime;
		this.transform.position = Euclidizer.AvieToEuclid(currentPos);
	}
}
