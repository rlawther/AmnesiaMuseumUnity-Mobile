private var motor : CharacterMotor;
public var networkDirectionVector : Vector3;

// Use this for initialization
function Awake () {
	motor = GetComponent(CharacterMotor);
	networkDirectionVector = new Vector3(0, 0, 0);
}

// Update is called once per frame
function Update () {
	var directionVector : Vector3;
	// Get the input vector from keyboard or analog stick
	var inputDirectionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	//var directionVector = new Vector3(1.0f, 1.0f, 0);
	//directionVector.x = 1.0f;
	
	
	directionVector = inputDirectionVector + networkDirectionVector;
	

	if (directionVector != Vector3.zero) {
		// Get the length of the directon vector and then normalize it
		// Dividing by the length is cheaper than normalizing when we already have the length anyway
		var directionLength = directionVector.magnitude;
		directionVector = directionVector / directionLength;
		
		// Make sure the length is no bigger than 1
		directionLength = Mathf.Min(1, directionLength);
		
		// Make the input vector more sensitive towards the extremes and less sensitive in the middle
		// This makes it easier to control slow speeds when using analog sticks
		directionLength = directionLength * directionLength;
		
		// Multiply the normalized direction vector by the modified length
		directionVector = directionVector * directionLength;
	}
	
	// Apply the direction to the CharacterMotor
	motor.inputMoveDirection = transform.rotation * directionVector;
	motor.inputJump = Input.GetButton("Jump");
}

// Require a character controller to be attached to the same game object
@script RequireComponent (CharacterMotor)
@script AddComponentMenu ("Character/FPS Input Controller")
