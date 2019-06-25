/*
This is a 3D First Person Controller script that can be used to simulate anything from 
high speed aerial combat, to submarine warfare. I have tried to make my variable names
as descriptive as possible so you can immediately see what they affect. The variables are
sorted by type; speed with speed, rotation with rotation ect... I have also included generic
ranges on variables that need them; relative upper and lower variable limits. This is because 
some of the variables have a greater effect as they approach 1, while others have greater
impact as they approach infinity. If for some reason the ship is not doing what it is supposed to,
check the ranges, as some variables create problems when they are set to 0 or very large values.

Also note the separate script titled Space Flight Script. This script has been
optimized to better suit space combat. The effects of gravity, drag and lift are removed
to better simulate flight in zero-gravity space.

This script uses controls based off 4 axis. I found these parameters worked well...
	Name : (Roll, Pitch, Yaw or Throttle)
	Gravity : 20
	Dead : 0.001
	Sensitivity : 1

Axis for each control (Axis based off a standard flight joystick).
	Pitch: Y- Axis
	Roll: X - Axis
	Yaw: 3'rd Axis
	Throttle: 4'th - Axis

How to use this script: 

	Drag and Drop the Transform and its Rigidbody onto the variables Flyer and
	FlyerRigidbody in the inspector panel. Remember to change the rigidbody's Drag
	value. If you dont change this, gravity will be unrealistic... (I set drag to 500)

	Change the variables to simulate the flight style you desire.

	Create a prefab of your GameObject, and back it up to a secure location.

	*Note: This is important because none of the variables are stored in the 
	script. If for some reason Unity crashes during testing, the variables
	are not stored when you save the javascript, but when you save the game
	project.

	Save often and enjoy!

	~Mirage~
*/


// Componants
var flyer : Transform;
var flyerRigidbody : Rigidbody;
var seaLevelTransform : Transform;


// Assorted control variables. These mostly handle realism settings, change as you see fit.
	var accelerateConst: float = 5;				// Set these close to 0 to smooth out acceleration. Don't set it TO zero or you will have a division by zero error.
	var decelerateConst: float = 0.065;	// I found this value gives semi-realistic deceleration, change as you see fit.
	
	/*	The ratio of MaxSpeed to Speed Const determines your true max speed. The formula is maxSpeed/SpeedConst = True Speed. 
		This way you wont have to scale your objects to make them seem like they are going fast or slow.
		MaxSpeed is what you will want to use for a GUI though.
	*/ 
	static var maxSpeed : float = 100;		
	var speedConst : float = 50;
	
	var throttleConst : int = 50;
	var raiseFlapRate : float = 1;						// Smoother when close to zero
	var lowerFlapRate : float = 1;						// Smoother when close to zero
	var maxAfterburner : float = 5;				// The maximum thrust your afterburner will produce
	var afterburnerAccelerate : float = 0.5;
	var afterburnerDecelerate : float = 1;
	var liftConst : float = 7.5;					// Another arbitrary constant, change it as you see fit.
	var angleOfAttack : float = 15;			// Effective range: 0 <= angleOfAttack <= 20
	var gravityConst = 9.8;				// An arbitrary gravity constant, there is no particular reason it has to be 9.8...
	var levelFlightPercent : int = 25;
	var maxDiveForce : float = 0.1;
	var noseDiveConst : float = 0.01;
	var minSmooth : float = 0.5;
	var maxSmooth : float = 500;
	var maxControlSpeedPercent : float = 75;		// When your speed is withen the range defined by these two variables, your ship's rotation sensitivity fluxuates.
	var minControlSpeedPercent : float = 25;		// If you reach the speed defined by either of these, your ship has reached it's max or min sensitivity.


// Rotation Variables, change these to give the effect of flying anything from a cargo plane to a fighter jet.
	var lockRotation : boolean;		// If this is checked, it locks pitch roll and yaw constants to the var rotationConst.
	var lockedRotationValue : int = 120;
	var pitchConst = 100;
	var rollConst = 100;
	var yawConst = 100;

// Airplane Aerodynamics - I strongly reccomend not touching these...
	private var nosePitch : float;
	private var trueSmooth : float;
	private var smoothRotation : float;
	private var truePitch : float;
	private var trueRoll : float;
	private var trueYaw : float;
	private var trueThrust : float;
	static var trueDrag : float;
	
// Misc. Variables
	static var afterburnerConst : float;
	static var altitude : int;

	
// HUD and Heading Variables. Use these to create your insturments.
	static var trueSpeed : int;
	static var attitude : int;
	static var incidence : int;
	static var bank : int;
	static var heading : int;


// Let the games begin!
function Start ()
{
	trueDrag = 0;
	afterburnerConst = 0;
	smoothRotation = minSmooth + 0.01;
	if (lockRotation == true)
		{
		pitchConst = lockedRotationValue;
		rollConst = lockedRotationValue;
		yawConst = lockedRotationValue;
		Screen.showCursor = false;
		}
}


function Update () 
{

	// * * This section of code handles the plane's rotation.
	
	var pitch = -Input.GetAxis ("Pitch") * pitchConst;
	var roll = Input.GetAxis ("Roll") * rollConst;
	var yaw = -Input.GetAxis ("Yaw") * yawConst;

	pitch *=  Time.deltaTime;
	roll *= -Time.deltaTime;
	yaw *= Time.deltaTime;
	
	// Smothing Rotations...	
	if ((smoothRotation > minSmooth)&&(smoothRotation < maxSmooth))
	{
		smoothRotation = Mathf.Lerp (smoothRotation, trueThrust, (maxSpeed-(maxSpeed/minControlSpeedPercent))* Time.deltaTime);
	}
	if (smoothRotation <= minSmooth)
	{
		smoothRotation = smoothRotation +0.01;
	}
	if ((smoothRotation >= maxSmooth) &&(trueThrust < (maxSpeed*(minControlSpeedPercent/100))))
	{
		smoothRotation = smoothRotation -0.1;
	}
	trueSmooth = Mathf.Lerp (trueSmooth, smoothRotation, 5* Time.deltaTime);
	truePitch = Mathf.Lerp (truePitch, pitch, trueSmooth * Time.deltaTime);
	trueRoll = Mathf.Lerp (trueRoll, roll, trueSmooth * Time.deltaTime);
	trueYaw = Mathf.Lerp (trueYaw, yaw, trueSmooth * Time.deltaTime);



	
// * * This next block handles the thrust and drag.
	var throttle = (((-(Input.GetAxis ("Throttle"))+1)/2)*100);
	

	if ( throttle/speedConst >= trueThrust)
	{
		trueThrust = Mathf.SmoothStep (trueThrust, throttle/speedConst, accelerateConst * Time.deltaTime);
	}
	if (throttle/speedConst < trueThrust)
	{
		trueThrust = Mathf.Lerp (trueThrust, throttle/speedConst, decelerateConst * Time.deltaTime);
	}	

	rigidbody.drag = liftConst*((trueThrust)*(trueThrust));
	
	if (trueThrust <= (maxSpeed/levelFlightPercent))
	{
		
		nosePitch = Mathf.Lerp (nosePitch, maxDiveForce, noseDiveConst * Time.deltaTime);
	}
	else
	{
		
		nosePitch = Mathf.Lerp (nosePitch, 0, 2* noseDiveConst * Time.deltaTime);
	}
	
	trueSpeed = ((trueThrust/2)*maxSpeed);
	
// ** Additional Input

	// Airbrake
	if (Input.GetButton ("Airbrake"))
	{
		trueDrag = Mathf.Lerp (trueDrag, trueSpeed, raiseFlapRate * Time.deltaTime);		
		
	}
	
	if ((!Input.GetButton ("Airbrake"))&&(trueDrag !=0))
	{
		trueDrag = Mathf.Lerp (trueDrag, 0, lowerFlapRate * Time.deltaTime);
	}
	
	
	// Afterburner
	if (Input.GetButton ("Afterburner"))
	{
		afterburnerConst = Mathf.Lerp (afterburnerConst, maxAfterburner, afterburnerAccelerate * Time.deltaTime);		
	}
	
	if ((!Input.GetButton ("Afterburner"))&&(afterburnerConst !=0))
	{
		afterburnerConst = Mathf.Lerp (afterburnerConst, 0, afterburnerDecelerate * Time.deltaTime);
	}
	
	
	// Adding nose dive when speed gets below a percent of your max speed	
	if ( ((trueSpeed - trueDrag) + afterburnerConst) <= (maxSpeed*levelFlightPercent/100))
	{
		noseDiveConst = Mathf.Lerp (noseDiveConst,maxDiveForce, (((trueSpeed - trueDrag) + afterburnerConst) - (maxSpeed * levelFlightPercent/100)) *5 *Time.deltaTime);
		flyer.Rotate(noseDiveConst,0,0,Space.World);		
	}
	
	
	// Calculating Flight Mechanics. Used mostly for the HUD.
	attitude = parseInt(-((Vector3.Angle(Vector3.up, flyer.forward))-90));
	bank = parseInt(((Vector3.Angle(Vector3.up, flyer.up))));	
	incidence = attitude + angleOfAttack;
	heading = parseInt(flyer.eulerAngles.y);
	
	if ( seaLevelTransform != null )
	{
	altitude = (flyer.transform.position.y - seaLevelTransform.transform.position.y);
	}
	//Debug.Log ((((trueSpeed - trueDrag) + afterburnerConst) - (maxSpeed * levelFlightPercent/100)));
}	// End function Update( );


function FixedUpdate () 
{
	if (trueThrust <= maxSpeed)
	{
		// Horizontal Force
		transform.Translate(0,0,((trueSpeed - trueDrag)/100 + afterburnerConst));
	}
	
	flyerRigidbody.AddForce (0,(rigidbody.drag - gravityConst),0);
	transform.Rotate (truePitch,-trueYaw,trueRoll);

}// End function FixedUpdateUpdate( )