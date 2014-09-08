using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class MasterCameraControl : MonoBehaviour {

    public float _rotationSpeed = 1.0f;
    public float _moveSpeed = 0.4f; 
	public bool isGameSpeedAgnostic = false;
	protected float prevTick;
	// Use this for initialization
	protected virtual void Start () {
		this.prevTick = Time.realtimeSinceStartup;
	}

	protected virtual float determineTimeMultiplier() {
		float timeMultiplier = Time.deltaTime;
		
		float t = Time.realtimeSinceStartup;	
		if (isGameSpeedAgnostic) {
			timeMultiplier =  t - this.prevTick;
		}
		this.prevTick = t;	
		return timeMultiplier;
	}
	
	// Update is called once per frame
	protected void Update () {
	
		float timeMultiplier = this.determineTimeMultiplier();

		
		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			timeMultiplier *= 3.0f;
		}
		
		
        if (Input.GetKey(KeyCode.Keypad4))
        {
            Vector3 mainCameraDirection = Vector3.right;
            mainCameraDirection = mainCameraDirection * -_moveSpeed*timeMultiplier;
            this.transform.Translate(mainCameraDirection);
        }
        else if (Input.GetKey(KeyCode.Keypad6))
        {
			Vector3 mainCameraDirection = Vector3.right;
            mainCameraDirection = mainCameraDirection * _moveSpeed*timeMultiplier;
            this.transform.Translate(mainCameraDirection);
        }
        else if (Input.GetKey(KeyCode.Keypad0))
        {
			Vector3 mainCameraDirection = Vector3.up;
            mainCameraDirection = mainCameraDirection * -_moveSpeed*timeMultiplier;
            this.transform.Translate(mainCameraDirection);
        }
        else if (Input.GetKey(KeyCode.KeypadPeriod))
        {
			Vector3 mainCameraDirection = Vector3.up;
            mainCameraDirection = mainCameraDirection * _moveSpeed*timeMultiplier;
            this.transform.Translate(mainCameraDirection);
        }
        else if (Input.GetKey(KeyCode.Keypad5))
        {
			Vector3 mainCameraDirection = Vector3.forward;
            mainCameraDirection = mainCameraDirection * -_moveSpeed*timeMultiplier;
            this.transform.Translate(mainCameraDirection);
        }
        else if (Input.GetKey(KeyCode.Keypad8))
        {            
			Vector3 mainCameraDirection = Vector3.forward;
            mainCameraDirection = mainCameraDirection * _moveSpeed*timeMultiplier; 
            this.transform.Translate(mainCameraDirection);
        }
        else if (Input.GetKey(KeyCode.Keypad7))
        {
            this.transform.Rotate(Vector3.up, -_rotationSpeed*timeMultiplier);
        }
        else if (Input.GetKey(KeyCode.Keypad9))
        {
            this.transform.Rotate(Vector3.up, _rotationSpeed*timeMultiplier);
        }
        else if (Input.GetKey(KeyCode.KeypadMinus))
        {
            this.transform.Rotate(Vector3.right, -_rotationSpeed*timeMultiplier);
        }
        else if (Input.GetKey(KeyCode.KeypadPlus))
        {
            this.transform.Rotate(Vector3.right, _rotationSpeed*timeMultiplier);
        }
		else if (Input.GetKey(KeyCode.KeypadDivide))
		{
			this.transform.Rotate (Vector3.forward, -_rotationSpeed*timeMultiplier);
		}
		else if (Input.GetKey (KeyCode.KeypadMultiply))
		{
			this.transform.Rotate (Vector3.forward, _rotationSpeed*timeMultiplier);
		}
		else if (Input.GetKey(KeyCode.Numlock))
		{
			this.transform.rotation = Quaternion.identity;
		}
		else if (Input.GetKey(KeyCode.KeypadEnter)) 
		{
			this.transform.position = Vector3.zero;
		}
		
	}
}
}