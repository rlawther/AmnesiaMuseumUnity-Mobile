using UnityEngine;
using System.Collections;

namespace Toolbelt {
[ExecuteInEditMode]
public class SpreadItems : MonoBehaviour {
	public Transform[] items;
	public float _spread = 0f;
	public float spread = 0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (this.spread != this._spread) {
			this._spread = this.spread;
			this.calculateSpread();
		}
	}
	private void calculateSpread() {
		float centreIndex = (float)items.Length/2.0f;
		if (items.Length % 2 == 0) {
			centreIndex -= 0.5f;
		}
		Vector3 centre = this.getCentre();
		for (int i = 0; i < items.Length; i++) {
			Vector3 pos = this.items[i].localPosition;
			pos.x = centre.x + this.spread*(i - centreIndex);
			this.items[i].localPosition = pos;			
		}
	}
	private Vector3 getCentre() {
		if (this.items.Length % 2 == 0) { 
			//return average of two middle things.
			return Vector3.Lerp (this.items[this.items.Length/2-1].localPosition,
								 this.items[this.items.Length/2].localPosition,0.5f);
		} else {			
			return this.items[this.items.Length/2].localPosition;
		}
		
	}
}
}