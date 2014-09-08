using UnityEngine;
using System.Collections;
#if NO_CLUSTER
#else
public class icClusterInput {
	public  static string masterIP = "10.0.1.28";
	protected static int numInputs = 0;
	protected string inputName;

	public icClusterInput(string s) {
		this.inputName = s;
		ClusterInput.AddInput(this.inputName,this.inputName,icClusterInput.masterIP,0,ClusterInputType.CustomProvidedInput);
		
	}

}

public class icClusterInputBool: icClusterInput {
	public icClusterInputBool(string s) :base(s) {
		
	}
	public void SetValue(bool f) {
		ClusterInput.SetButton(this.inputName,f);
	}
	public bool GetValue() {
		return ClusterInput.GetButton(this.inputName);
	}
}

public class icClusterInputFloat: icClusterInput {
	public icClusterInputFloat(string s) :base(s) {

	}
	public void SetValue(float f) {
		ClusterInput.SetAxis(inputName,f);
	}
	public float GetValue() {
		return ClusterInput.GetAxis(this.inputName);
	}
}
#endif