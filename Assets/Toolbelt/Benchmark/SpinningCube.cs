using UnityEngine;
using System.Collections;
namespace Toolbelt {

public class SpinningCube : MonoBehaviour {
	public float speed;
	private AvieMovement am;
	void Start() {
		this.am = this.gameObject.GetComponent<AvieMovement>();
	}
	// Update is called once per frame
	void Update () {
		
		Vector3 pos = this.am.AviePosition;
		pos.x += Time.deltaTime * this.speed;
		this.am.AviePosition = pos;
	}
}
}