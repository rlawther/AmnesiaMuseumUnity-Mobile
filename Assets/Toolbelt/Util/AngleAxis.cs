using UnityEngine;
using System.Collections;

public class AngleAxis {
	public float angle;
	public Vector3 axis;
	public AngleAxis() {
		this.angle = 0;
		this.axis = new Vector3();
	}
	public AngleAxis(float angle, Vector3 axis) {
		this.angle = angle;
		this.axis = axis;
	}
}
