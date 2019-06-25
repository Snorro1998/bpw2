/*
	This script is a simplified version of Flight Sim. This can be used to create
	semi-realistic flight. Rotation is directly proportional to User Input, there
	is no wind, no air and no force proportional to angle here. This is idealy suited 
	for uses in simplified jet simulators, or any applications that require smoothed 
	3d positioning.
*/


// Componants of the flyer
var flyer : Transform;
var flyerRigidbody : Rigidbody;

// Assorted control variables. These mostly handle realism settings, change as you see fit.
//var throttleDelta : float=0.08;			// This defines how fast the throttle value changes
var accelerateConst: float;
var decelerateConst: float = 0.065;	// I found this value gives semi-realistic deceleration, change as you see fit.
var maxSpeed : float = 100;
var speedConst : float = 100;
var throttleConst = 30;
var airbrakeConst : float = 30;

var liftConst : float;					// Another arbitrary constant, change it as you see fit.
var dragConst : float;				// Note that this is NOT the same as the rigidbody drag. This is used for the airbrake
var angleOfAttack : float;			// Effective range: 0 <= angleOfAttack <= 20
var gravityConst = -9.8;				// An arbitrary gravity constant, there is no particular reason it has to be 9.8...
var levelFlightPercent : int;
var maxDiveForce : float;
var noseDiveConst : float;
var minSmooth : float;
var maxSmooth : float;
var maxControlSpeedPercent : float = 75;		// When your speed is withen the range defined by these two variables, your ship's rotation sensitivity fluxuates.
var minControlSpeedPercent : float = 25;		// If you reach the speed defined by either of these, your ship has reached it's max or min sensitivity.


// Rotation Variables, these change how your plane handles. 
// Change these to give the effect of flying anything from a cargo plane to a fighter jet.

	// If this is checked, it locks pitch roll and yaw constants to the var rotationConst.
var lockedConst : boolean;		
var rotationConst : int = 120;
var pitchConst = 50;
var rollConst = 50;
var yawConst = 15;

	// These set the sensitivity of each individual axis. If you want higher sensitivity on pitch for example, change this value.
var pitchDelta : float = 1.05;		
var rollDelta : float = 1.05;
var yawDelta : float = 1.05;

// Airplane Aerodynamics - I strongly reccomend not touching these...
private var trueSmooth : float;
private var smoothRotation : float;
private var truePitch : float;
private var trueRoll : float;
private var trueYaw : float;
private var thrust : float;
private var trueThrust : float;
private var trueDrag : float;
private var nosePitch : float;
private var attitude : float;


// Let the games begin!
function Start ()
{
	smoothRotation = minSmooth + 0.01;
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
	
	// Calculating Angle of Attack
	attitude = -((Vector3.Angle(Vector3.up, flyer.forward))-90);
	angleOfIncidence = attitude - angleOfAttack;
	
	
	var pitch = -Input.GetAxis ("Pitch") * pitchConst;
	var roll = Input.GetAxis ("Roll") * rollConst;
	var yaw = -Input.GetAxis ("Yaw") * yawConst;

	pitch *= pitchDelta * Time.deltaTime;
	roll *= -rollDelta * Time.deltaTime;
	yaw *= yawDelta * Time.deltaTime;
	
	// Smothing Rotations...	
	if ((smoothRotation > minSmooth)&&(smoothRotation < maxSmooth))
	{
		smoothRotation = Mathf.Lerp (smoothRotation, trueThrust, Time.deltaTime);
	}
	if (smoothRotation <= minSmooth)
	{
		smoothRotation = smoothRotation +0.01;
	}
	if ((smoothRotation >= maxSmooth) &&(trueThrust < (maxSpeed*(maxControlSpeedPercent/100))))
	{
		smoothRotation = smoothRotation -0.1;
	}
	trueSmooth = Mathf.Lerp (trueSmooth, smoothRotation, 5* Time.deltaTime);
	truePitch = Mathf.Lerp (truePitch, pitch, trueSmooth * Time.deltaTime);
	trueRoll = Mathf.Lerp (trueRoll, roll, trueSmooth * Time.deltaTime);
	trueYaw = Mathf.Lerp (trueYaw, yaw, trueSmooth * Time.deltaTime);



	
	// * * This next block handles the thrust and drag.
	var throttle = (((-(Input.GetAxis ("Throttle"))+1)/2)*maxSpeed);
	
	//throttle *= (throttleDelta * Time.deltaTime);
	if ( throttle/speedConst >= trueThrust)
	{
		trueThrust = Mathf.SmoothStep (trueThrust, throttle/speedConst, accelerateConst * Time.deltaTime);
	}
	if (throttle/speedConst < trueThrust)
	{
		trueThrust = Mathf.Lerp (trueThrust, throttle/speedConst, decelerateConst * Time.deltaTime);
	}	


	
	// * * Now we are applying lift, gravity and level flight to airplane.
	rigidbody.drag = liftConst*((trueThrust)*(trueThrust));
	//trueDrag = dragConst*((trueThrust)*(trueThrust));
	
	if (trueThrust <= (maxSpeed/levelFlightPercent))
	{
		
		nosePitch = Mathf.Lerp (nosePitch, maxDiveForce, noseDiveConst * Time.deltaTime);
	}
	else
	{
		
		nosePitch = Mathf.Lerp (nosePitch, 0, 2* noseDiveConst * Time.deltaTime);
	}
	Debug.Log (angleOfIncidence);
}	// End function Update( );


function FixedUpdate () 
{
	// This is a airbrake, this increases drag, lowering your speed
	if (Input.GetButton ("Airbrake"))
	{
		dragConst = Mathf.Lerp (trueThrust, 0, airbrakeConst * Time.deltaTime);
	}
	
	// Setting airbrake back to 0 when not used. These are flaps, not car brakes
	// We need to lerp these back to 0 so they dont "snap" off.
	if ((!Input.GetButtonDown ("Airbrake"))&&(dragConst !=0))
	{
		dragConst = Mathf.Lerp (dragConst, 0, 25 * Time.deltaTime);
	}
	
	// Seperated addRelativeForce so we have better control over when we want them to run.
	if (trueThrust <= maxSpeed)
	{
		// Horizontal Force
		//flyerRigidbody.AddRelativeForce (0,0, (trueThrust - trueDrag)*5);
		transform.Translate(0,0,(trueThrust-trueDrag));
	}
	
	flyerRigidbody.AddForce (0,(rigidbody.drag - gravityConst),0);
	transform.Rotate (truePitch,-trueYaw,trueRoll);
	//flyerRigidbody.AddTorque(nosePitch/200,0,0);

}// End function FixedUpdateUpdate( );
	