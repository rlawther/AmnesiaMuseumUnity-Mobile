using UnityEngine;
using System.Collections;

namespace Toolbelt {
[ExecuteInEditMode]
public class AvieMovement : MonoBehaviour {
	public Vector3 AviePosition;
	private Vector3 _AviePosition;
	public bool calculateAviePosition;
	public bool faceCentre = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void faceCentreCheck() {
		if (this.faceCentre) {
			Vector3 direction = this.transform.position - new Vector3(0,0,0);
			this.transform.rotation = Quaternion.LookRotation(direction);
			
		}
	}
	void Update () {
		if (this.calculateAviePosition) {
			Euclidizer.EuclidToAvie(this.transform.localPosition,ref this.AviePosition);
			_AviePosition = AviePosition; //structs are passed by value;
			this.calculateAviePosition = false;
			this.faceCentreCheck();
		} else if (_AviePosition != AviePosition) {
			_AviePosition = AviePosition;
			this.transform.localPosition = Euclidizer.AvieToEuclid(this.AviePosition);
			this.faceCentreCheck();
		}
		
	}
}
}