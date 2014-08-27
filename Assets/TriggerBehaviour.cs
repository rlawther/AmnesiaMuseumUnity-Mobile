using UnityEngine;
using System.Collections;

public class TriggerBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider other) {
		//Debug.Log ("Collided with " + other.name);
		/* When we collide with the terrain, we stop the quad from moving */
		if (other.name.EndsWith("terrain_collider"))
		{
			gameObject.rigidbody.isKinematic = true;
			gameObject.collider.enabled = false;
		}

	}
}
