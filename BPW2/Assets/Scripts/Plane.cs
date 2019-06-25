using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    [Header("Plane Parts")]
    [SerializeField]
    private Transform AilLeft;
    [SerializeField]
    private Transform AilRight;
    [SerializeField]
    private Transform ElevLeft, ElevRight;
    [SerializeField]
    private Transform LeftProp, RightProp;
    [SerializeField]
    private Transform SmokeLeft, SmokeRight;
    [SerializeField]
    private Transform Rudder;

    [SerializeField]
    private Transform WaterSpawner;
    [SerializeField]
    private Transform Arrow;

    private Rigidbody rb;

    [Header("Variables")]
    public bool EngineRunning = false;
    private bool isMissingParts = false;

    private bool leftEngineRunning, rightEngineRunning = false;

    public float thrust, speed = 0;

    public float pitch, roll, yaw = 0;
    float ElevAngleSmooth, YawAngleSmooth, RollAngleSmooth = 0;
    public float pitchSmooth = 0;

    public float lift = 0;
    private Vector3 gravityVector, liftForce = new Vector3(0, 0, 0);

    public float propSpeedLeft, propSpeedRight = 0;
    private float propMinSpeed = 1200f;
    private float propMaxSpeed = 2400f;

    private float maxSpeed = 250f;

    float rotX, rotY, rotZ = 0;

    float maxAngle = 20;

    bool isGrounded = false;

    private float enginePitch, enginePitchOld;

    private float vSpeed = 0;

    public bool frozen = false;

    private Coroutine _coroutine;

    private IEnumerator startEngines() {
        propSpeedLeft = 200;
        AudioManager.Instance.playSound("engineStart");
        yield return new WaitForSeconds(1.8f);
        propSpeedLeft = propMinSpeed;
        leftEngineRunning = true;
        SmokeLeft.gameObject.SetActive(true);
        //waarom werkt dit niet?
        //Instantiate(SmokeCloud, LeftProp.position, Quaternion.Euler(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z)/*transform.rotation + new Quaternion Quaternion.Euler(1,1,1)*/);//SmokeCloud.transform.rotation);
        //GameObject smokePuff = Instantiate(SmokeCloud, LeftProp.position, Quaternion.Euler(transform.rotation.x, transform.rotation.y /*+ 180*/, transform.rotation.z));
        //smokePuff.transform.Rotate(0, 180, 0);
        AudioManager.Instance.playSound("engineRunning");
        yield return new WaitForSeconds(1.5f);
        SmokeLeft.gameObject.SetActive(false);
        propSpeedRight = 200;
        AudioManager.Instance.playSound("engineStart");
        yield return new WaitForSeconds(1.8f);
        propSpeedRight = propMinSpeed;
        rightEngineRunning = true;
        SmokeRight.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SmokeRight.gameObject.SetActive(false);
    }
    

// Start is called before the first frame update
void Start()
    {
        rb = GetComponent<Rigidbody>();
        isMissingParts = (!AilLeft || !AilRight || !ElevLeft || !ElevRight || !LeftProp || !RightProp || !Rudder);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //print(transform.position.y - vSpeed);

        vSpeed = transform.position.y;
        if (Input.GetKeyDown("m"))
        {
            if (!EngineRunning)
            {
                EngineRunning = true;
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }
                _coroutine = StartCoroutine(startEngines());
            }
        }

        if (WaterSpawner != null)
        {
            WaterSpawner.gameObject.SetActive(Input.GetKey("q"));
        }

        if (leftEngineRunning && rightEngineRunning)
        {
            thrust = /*Input.GetAxis("Throttle") * */12;
            speed = Mathf.Clamp(speed + thrust * Time.deltaTime, 0, maxSpeed);
        }
        

        pitch = Input.GetAxis("Pitch");
        roll = Input.GetAxis("Roll") * maxAngle;
        yaw = Input.GetAxis("Yaw");

        float ElevAngle = -pitch * maxAngle;
        float YawAngle = yaw * maxAngle;
        float RollAngle = -YawAngle;
        
        if (transform.position.y < 30)
        {
            RollAngle = 0;
        }

        ElevAngleSmooth = Mathf.Lerp(ElevAngleSmooth, ElevAngle, 2 * Time.deltaTime);
        YawAngleSmooth = Mathf.Lerp(YawAngleSmooth, YawAngle, 2 * Time.deltaTime);
        //RollAngleSmooth = -YawAngleSmooth;
        RollAngleSmooth = Mathf.Lerp(RollAngleSmooth, RollAngle, 2 * Time.deltaTime);

        applyLift();
        setPropSpeeds();
        rotateParts();

        rotX = Mathf.Clamp(ElevAngleSmooth, -20, 20);
        rotY = transform.rotation.eulerAngles.y + speed * YawAngleSmooth * Time.deltaTime / 50;
        rotZ = Mathf.Clamp(RollAngleSmooth, -20, 20);

        

        if (Mathf.Abs(rotX) < .1 || rotX + 0.1f >= 360 /*|| lift < 8000*/)
        {
            rotX = 0;
        }
        if (Mathf.Abs(rotY) < .1 || rotY + 0.1f >= 360)
        {
            rotY = 0;
        }

        float verm = MathMap(0, 1, 0, maxSpeed, speed);
        rotX *= verm;

        if (!frozen)
        {
            transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
        

        float rotPitch = Mathf.Clamp(ElevAngleSmooth * Time.deltaTime, -20, 20);

        //transform.Rotate(rotPitch, speed * YawAngleSmooth * Time.deltaTime / 50, 0);

        
        rotX = transform.rotation.eulerAngles.x;

        setEnginePitch();
    }

    public void disableArrow()
    {
        if (Arrow != null)
        {
            Arrow.gameObject.SetActive(false);
        }
    }

    void applyLift()
    {
        gravityVector = Physics.gravity * rb.mass;
        lift = Mathf.Clamp(rb.mass * -Physics.gravity.y * (1.5f * speed / maxSpeed), 0, Mathf.Abs(gravityVector.y)) + pitchSmooth * 500;
        liftForce = new Vector3(0, lift, 0);
        rb.AddForce(liftForce);
    }

    void setEnginePitch()
    {
        enginePitch = MathMap(1, 2, 0, maxSpeed, speed);
        if (enginePitchOld != enginePitch)
        {
            enginePitchOld = enginePitch;
            AudioManager.Instance.setPitch("engineRunning", enginePitch);
        } 
    }

    void rotateParts()
    {
        if (!isMissingParts)
        {
            LeftProp.transform.Rotate(new Vector3(0, 0, propSpeedLeft * Time.deltaTime));
            RightProp.transform.Rotate(new Vector3(0, 0, propSpeedRight * Time.deltaTime));

            ElevLeft.localRotation = ElevRight.localRotation = Quaternion.Euler(-ElevAngleSmooth, 0, 0);
            AilLeft.localRotation = Quaternion.Euler(RollAngleSmooth, 0, 0);
            AilRight.localRotation = Quaternion.Euler(-RollAngleSmooth, 0, 0);
            Rudder.localRotation = Quaternion.Euler(0, YawAngleSmooth, 0);
        }
    }

    void setPropSpeeds()
    {
        if (leftEngineRunning)
        {
            propSpeedLeft = MathMap(propMinSpeed, propMaxSpeed, 0, maxSpeed, speed);
        }
        if (rightEngineRunning)
        {
            propSpeedRight = MathMap(propMinSpeed, propMaxSpeed, 0, maxSpeed, speed);
        }
    }

    float MathMap(float outPutLow, float outPutHigh, float inputLow, float inputHigh, float value)
    {
        if (value <= inputLow)
            return outPutLow;
        else if (value >= inputHigh)
            return outPutHigh;
        return (outPutHigh - outPutLow) * ((value - inputLow) / (inputHigh - inputLow)) + outPutLow;
    }
}
