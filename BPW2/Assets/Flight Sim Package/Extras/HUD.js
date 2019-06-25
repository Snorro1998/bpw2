var enableHeadsUpDisplay : boolean = false;
private var speedGUI : float;
private var headingGUI : float;
private var attitudeGUI : float;
private var bankGUI : float;

// FPS Counter Variables
var updateInterval = 0.5;

private var accum = 0.0; // FPS accumulated over the interval
private var frames = 0; // Frames drawn over the interval
private var timeleft : float; // Left time for current interval


function OnGUI () 
{
	
	if (GUI.Button (Rect (10,150,150,20), "Toggle Hud")) 
	{
		if (enableHeadsUpDisplay == false)
		{
			enableHeadsUpDisplay = true;
		}
		else
		{
			enableHeadsUpDisplay = false;
		}
	}
	GUI.Label (Rect (10,170,150,20),("FPS - " + (accum/frames).ToString("f2")));


	if (enableHeadsUpDisplay == true)
	{
		speedGUI = ((FlightSim.trueSpeed - FlightSim.trueDrag) + FlightSim.afterburnerConst);
		headingGUI = FlightSim.heading;
		attitudeGUI = FlightSim.attitude;
		bankGUI = FlightSim.bank;
		altitudeGUI = FlightSim.altitude;
		waypointAngle =WaypointGUI.angle2;
		
		GUI.Label (Rect (10,10,150,100), "Speed - " + parseInt(speedGUI));
		speedGUI = GUI.HorizontalSlider (Rect (100,20,150,100), speedGUI, 0, FlightSim.maxSpeed);
		GUI.Label (Rect (10,30,150,100), "Heading - " + headingGUI);	
		headingGUI = GUI.HorizontalSlider (Rect (100,40,150,100), headingGUI, 0, 360);
		GUI.Label (Rect (10,50,150,100), "Attitude- " + attitudeGUI);
		attitudeGUI = GUI.HorizontalSlider (Rect (100,60,150,100), attitudeGUI, -90, 90);
		GUI.Label (Rect (10,70,150,100), "Bank - " + bankGUI);
		bankGUI = GUI.HorizontalSlider (Rect (100,80,150,100), bankGUI, -180, 180);
		GUI.Label (Rect (10,90,150,100), "Altitude - " + altitudeGUI);
		
		GUI.Label (Rect (10,110,150,100), "Distance to waypoint - " + WaypointGUI.distance2);	
		waypointAngle = GUI.HorizontalSlider (Rect (10,130,150,100), waypointAngle, 180, -180);
	}
	
	
	// FPS Counter
	
    timeleft -= Time.deltaTime;
    accum += Time.timeScale/Time.deltaTime;
    ++frames;
    
    // Interval ended - update GUI text and start new interval
    if( timeleft <= 0.0 )
    {
        timeleft = updateInterval;
        accum = 0.0;
        frames = 0;
    }
}
