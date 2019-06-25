
/* This is identical to the Flight Sim script, I just optimized it for realistic space flight
In space there is no gravity or lift, and no need to bank into a turn.
*/


// Componants of the flyer
var flyer : GameObject;

// Various control variables. These mostly control realism settings, change as you see fit.
var throttleDelta : float=1;			// This defines how fast the throttle value changes
var accelerateConst: float = 10;
var decelerateConst: float = 0;
var smoothRotation : float = 1.5;
var speedConst : int = 250;
var throttleConst = 30;
var maxSpeed : float = 100;
var liftConst : float;					// Another arbitrary constant, change it as you see fit.
var dragConst : float;				// Note that this is NOT the same as the rigidbody drag...
var gravityConst = 9.8;				// An arbitrary gravity constant, there is no particular reason it has to be 9.8...


// Rotation Variables, these change how your plane turns.
var lockedConst : boolean;		// If this is checked, it locks pitch roll and yaw const to the var rotationConst.
var rotationConst : int = 100;
var pitchConst = 80;
var rollConst = 80;
var yawConst = 80;
var pitchDelta : float = 1.05;
var rollDelta : float = 1.05;
var yawDelta : float = 1.05;


// Private variables that dont need to clutter the inspector panel.
private var trueSmooth : float;
private var truePitch : float;
private var trueRoll : float;
private var trueYaw : float;

// Airplane Aerodynamics - don't alter these... Anything that is preceded by "true" is directly plugged into a movement or rotation line.
private var thrust : float;
private var trueLift : float;
private var trueThrust : float;
private var trueDrag : float;
private var trueGrav : float;



// Let the games begin!
function Start ()
{
	flyer.rigidbody.drag = 1;
	if (lockedConst == true)
		{
		pitchConst = rotationConst;
		rollConst = rotationConst;
		yawConst = rotationConst;
		Screen.showCursor = false;
		}
}

function Update () 
{
	// * * This section of code handles the plane's rotation.
	var pitch = -Input.GetAxis ("Pitch") * pitchConst;
	var roll = Input.GetAxis ("Roll") * rollConst;
	var yaw = -Input.GetAxis ("Yaw") * yawConst;

	pitch *= pitchDelta * Time.deltaTime;
	roll *= -rollDelta * Time.deltaTime;
	yaw *= yawDelta * Time.deltaTime;
	
	// Smothing Rotations...
	trueSmooth = Mathf.Lerp (trueSmooth, smoothRotation, 5* Time.deltaTime);
	truePitch = Mathf.Lerp (truePitch, pitch, trueSmooth * Time.deltaTime);
	trueRoll = Mathf.Lerp (trueRoll, roll, trueSmooth * Time.deltaTime);
	trueYaw = Mathf.Lerp (trueYaw, yaw, trueSmooth * Time.deltaTime);


	// * * This next block handles the thrust and drag.
	// This block sets the value of the joystick throttle ( float value between 0 and 1)
	var throttle = (-(Input.GetAxis ("Throttle"))+1)/2 * throttleConst;
	throttle *= throttleDelta * Time.deltaTime;
	if ( throttle >= trueThrust)
	{
		trueThrust = Mathf.SmoothStep (trueThrust, throttle, accelerateConst * Time.deltaTime);
	}
	if (throttle < trueThrust)
	{
		trueThrust = Mathf.Lerp (trueThrust, throttle, decelerateConst * Time.deltaTime);
	}	

	// This is a airbrake, this increases drag, lowering your speed
	if (Input.GetButtonDown ("Airbrake"))
	{
		// Do such
	}
	
	// * * Now we are applying lift and gravity to airplane.
	
}	// End function Update( );


// Now we apply what we calculated...	
function FixedUpdate () 
{
	// Seperated addRelativeForce so we have better controll over when we want them to run.
	if (trueThrust <= maxSpeed)
	{
		// Horizontal Force
		flyer.rigidbody.AddRelativeForce (0,0,trueThrust*speedConst);
		//transform.Translate (0,0,trueThrust);
	}
	
	flyer.rigidbody.AddRelativeForce (0,trueLift,0);
	transform.Rotate (truePitch,-trueYaw,trueRoll);
}// End function FixedUpdateUpdate( );