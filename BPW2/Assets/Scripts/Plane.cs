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
    private Transform FloatLeft, FloatRight;

    [Header("Other Parts")]
    [SerializeField]
    private Transform WaterSpawner;
    [SerializeField]
    public Transform WaterEffect;
    [SerializeField]
    private Transform Arrow;

    private Rigidbody rb;
        

    [Header("Variables")]
    public bool debug = false;
    public bool EngineRunning = false;
    private bool isMissingParts = false;

    private bool leftEngineRunning, rightEngineRunning = false;

    public float thrust, speed = 0;

    //private float pitch, roll, yaw = 0;
    private float ElevAngleSmooth, YawAngleSmooth, RollAngleSmooth = 0;
    private float pitchSmooth = 0;

    private float lift = 0;
    private Vector3 gravityVector, liftForce = new Vector3(0, 0, 0);

    private float propSpeedLeft, propSpeedRight = 0;
    private float propMinSpeed = 1200f;
    private float propMaxSpeed = 2400f;

    private float maxSpeed = 250f;

    float rotX, rotY, rotZ = 0;

    float maxAngle = 20;

    private float enginePitch, enginePitchOld;
    public float waterLevel = 0;

    public bool frozen = false;
    public bool onWater = false;

    private float floatAngle = 0;
    private float floatMaxAngle = 90;

    private Coroutine _coroutine;

    private IEnumerator startEngines()
    {
        propSpeedLeft = 200;
        AudioManager.Instance.playSound("engineStart");
        yield return new WaitForSeconds(1.8f);
        propSpeedLeft = propMinSpeed;
        leftEngineRunning = true;
        SmokeLeft.gameObject.SetActive(true);
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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isMissingParts = (!AilLeft || !AilRight || !ElevLeft || !ElevRight || !LeftProp || !RightProp || !Rudder || !FloatLeft || !FloatRight);
    }
    
    void FixedUpdate()
    {
        checkOnWater();
        checkInput();

        if (WaterSpawner != null)
        {
            WaterSpawner.gameObject.SetActive(Input.GetKey("q") && waterLevel > 0);
        }

        if (leftEngineRunning && rightEngineRunning)
        {
            thrust = /*Input.GetAxis("Throttle") * */12;
            speed = Mathf.Clamp(speed + thrust * Time.deltaTime, 0, maxSpeed);
        }

        float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);

        if (terrainHeight > transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, terrainHeight, transform.position.z);
        }


        float pitch = Input.GetAxis("Pitch");
        float roll = Input.GetAxis("Roll") * maxAngle;
        float yaw = Input.GetAxis("Yaw");

        float ElevAngle = -pitch * maxAngle;
        float YawAngle = yaw * maxAngle;
        float RollAngle = transform.position.y - terrainHeight > 30 ? -YawAngle : 0;

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

            //transform.Translate(0, 0, speed * Time.deltaTime);

            movePlane(1, 0);
        }


        float rotPitch = Mathf.Clamp(ElevAngleSmooth * Time.deltaTime, -20, 20);

        rotX = transform.rotation.eulerAngles.x;

        setEnginePitch();
    }

    void checkInput()
    {
        if (!LevelProperties.Instance.levelStarted) return;
        if (Input.GetKeyDown("t"))
        {
            EngineRunning = true;
            leftEngineRunning = true;
            rightEngineRunning = true;
            speed = 400;
            AudioManager.Instance.playSound("engineRunning");
        }

        if (Input.GetKeyDown("v"))
        {
            frozen = !frozen;
        }

        if (Input.GetKeyDown("m"))
        {
            if (!EngineRunning)
            {
                startEngine();
            }
            else
            {
                stopEngine();
            }
        }

        if (Input.GetKey("q"))
        {
            waterLevel = Mathf.Max(waterLevel - 10 * Time.deltaTime, 0);
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (debug)
        {
            print("Gameobject: " + collision.gameObject);
            print("Tag: " + collision.transform.tag);
            print("Layer: " + collision.gameObject.layer);
        }
    }

    void startEngine()
    {
        EngineRunning = true;
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _coroutine = StartCoroutine(startEngines());
    }

    void stopEngine()
    {
        EngineRunning = false;

        propSpeedLeft = 0;
        propSpeedRight = 0;
        AudioManager.Instance.stopSound("engineRunning");
        leftEngineRunning = false;
        rightEngineRunning = false;
        speed = 0;
    }

    void checkOnWater()
    {
        if (onWater)
        {
            if (!WaterEffect.gameObject.activeSelf)
            {
                WaterEffect.gameObject.SetActive(true);
            }
            waterLevel = Mathf.Min(waterLevel + 10 * Time.deltaTime, 100);
        }

        else
        {
            if (WaterEffect.gameObject.activeSelf)
            {
                WaterEffect.gameObject.SetActive(false);
            }
        }
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

            floatAngle = onWater ? Mathf.Min(floatAngle + 90 * Time.deltaTime, floatMaxAngle) : Mathf.Max(floatAngle - 90 * Time.deltaTime, 0);

            FloatLeft.localRotation = Quaternion.Euler(0, 0, floatAngle);
            FloatRight.localRotation = Quaternion.Euler(0, 0, -floatAngle);
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

    void movePlane(float inputY,float inputX)
    {
        Vector3 forwardMovement = (transform.forward * inputY * speed * Time.deltaTime);
        Vector3 sidewaysMovement = (transform.right * inputX * speed * Time.deltaTime);

       rb.MovePosition(gameObject.transform.position + (forwardMovement) + (sidewaysMovement));

        //dit moet het doen
        //je moet althans wel nog deze functie oproepen maar dat weet je wel en hem ff goed instellen
        //goed dat je bezig bent met unity man documenteer al je progress en je resultaten en dat moet je laten zien op je herkansing man
        //dan heb je veel kans om alsnog door te gaan met HKU
        //ja bedankt man, zal ik zeker doen. top, gozer ik ga weer veel succes als je me nodig hebt ik kan eind volgende week weer helpen en btw
        // 9 augustus afrijden examen like a baus
        //goed bezig succes ik ga weer
        //ja bedankt voor de moeite.np brother je bent een goeie camaraad doe ik het voor (Y) < duimpje in fb taal XD ltr
    }


}
