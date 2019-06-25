var projectile : Rigidbody;
var initialSpeed = 20.0;
var reloadTime = 0.5;
var ammoCount = 20;
var leftPod : Transform;
var rightPod : Transform;

private var leftRight : boolean;
private var instantiatedProjectile : Rigidbody;
private var lastShot = -10.0;

// Make sure to turn the projectile's rigidbody gravity to 0!

function Fire () {
	if (Time.time > reloadTime + lastShot && ammoCount > 0) 
	{
		if (leftRight == false)
		{
			instantiatedProjectile = Instantiate (projectile, leftPod.position, transform.rotation);
			instantiatedProjectile.velocity = leftPod.TransformDirection(Vector3 (0, 0, initialSpeed));
			leftRight = true;
		}
		else
		{
			instantiatedProjectile = Instantiate (projectile, rightPod.position, transform.rotation);
			instantiatedProjectile.velocity = rightPod.TransformDirection(Vector3 (0, 0, initialSpeed));
			leftRight = false;
		}
		Physics.IgnoreCollision(instantiatedProjectile.collider, transform.root.collider);		
		lastShot = Time.time;
		ammoCount--;
	}
}