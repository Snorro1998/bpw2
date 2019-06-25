var waypoint : Transform;
static var distance2 : int;
static var angle2 : int;
private var heading : Vector3;
private var dirNum : float;

function Update () 
{    
	if ( waypoint != null )
	{
   heading.z = (Vector3.Angle(waypoint.position, transform.forward)); 
   dirNum = AngleDir(transform.forward, heading, transform.up); 



	distance2 = Vector3.Distance(transform.position, waypoint.position);
	angle2 = Vector3.Angle((waypoint.position - transform.position), transform.forward);
	if (dirNum <=0 )
	{
	angle2 *= 1;
	}
	else
	{
		angle2 *= -1;
	}
	}
	//Debug.Log (distance2 + "  " + angle2);
}

function AngleDir(fwd: Vector3, targetDir: Vector3, up: Vector3) 
{ 
   var perp: Vector3 = Vector3.Cross(fwd, targetDir); 
   var dir: float = Vector3.Dot(perp, up); 
    
   if (dir >= 0.0) 
   { 
      return 1.0; 
   } 
   
   else if (dir < 0.0) 
   { 
      return -1.0; 
   } 
} 