using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM : MonoBehaviour
{

    private float initialVelocity = 1628f; // m.s^-1  1628f
    private float LMDryMass = 4280; // kg mass when the rocket is empty
    private float LMInitFuelMass = 7942; // kg mass of the fuel only
    private float LMFuelMass = 0;
    private static float LMThrust = 45.04f * Mathf.Pow(10, 3); // N
    private float LMMassFlowRate = 8.7279f; // kg.s^-1

    public Moon moon;
    public Constants constants;
    public UIManager uiManager;
    
    public GameObject redDot;

    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;
    private float accelerationMulitplier; //Engine acceleration depends on mass left in rocket

    private bool accelerating = true;
    private bool linked = true;

    private bool orbiting = true;
    private bool smallAcceleration = false;
    private bool DOI = false;
    private bool breakingPhase = false;
    private bool approachPhase = false;
    private bool terminalDescent = false;

    private float time = 0; // in seconds
    private float smallAccelerationTime = 30f; // in seconds
    private float DOITime = 600f; // in seconds
    private float breakingPhaseTime = 650f; // in seconds
    private float approachPhaseTime = 60f; // in seconds
    private float terminalDescentTime = 60f; // in seconds

    private float vc1 = 0; // speed at r1
    private float deltav = -34.8f; // in m.s^-1

    private Vector3 orientation;

    void Start()
    {
        if (constants == null)
        {
            constants = GameObject.FindGameObjectWithTag("Constants").GetComponent<Constants>();
        }
        
        if (moon == null)
        {
            moon = GameObject.FindGameObjectWithTag("Moon").GetComponent<Moon>();
        }
        
        if (uiManager == null)
        {
            uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        }

        LMFuelMass = LMInitFuelMass;

        StartCoroutine(RedDots());
    }
    
    IEnumerator RedDots() 
    {
        while (true)
        {
            Instantiate(redDot, this.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(3 / constants.timeMultiplier);
        }
    }

    void FixedUpdate()
    {
        if (!linked)
        {
            time += constants.fixedUpdateMultiplier * constants.timeMultiplier;
            acceleration = GetGravityAcceleration(position, moon.position);
            
            if (orbiting)
            {
                orbiting = false;
                smallAcceleration = true;
                Debug.Log("started smallAcceleration");
            }
            else if (smallAcceleration)
            {
                acceleration += Accelerate(orientation + new Vector3(0,0,30), 20);
                if (time > smallAccelerationTime)
                {
                    smallAcceleration = false;
                    StartDOI();
                }
            }
            else if (DOI)
            {
                
                acceleration += Accelerate(orientation + new Vector3(0,0,180), 100);
                
                if (Magnitude(velocity) - vc1 <= deltav)
                {
                    DOI = false;
                    breakingPhase = true;
                    Debug.Log("started breakingPhase");
                }
                
            }

            else if (breakingPhase)
            {
                if (time > breakingPhaseTime)
                {
                    breakingPhase = false;
                    approachPhase = true;
                    Debug.Log("started aproachPhase");
                }
            }
            else if (approachPhase)
            {
                if (time > approachPhaseTime)
                {
                    approachPhase = false;
                    terminalDescent = true;
                    Debug.Log("started terminalDescent");
                }
            }
            else if (terminalDescent)
            {
                if (time > terminalDescentTime)
                {
                    terminalDescent = false;
                    Debug.Log("landing");
                }
            }
            
            velocity += acceleration * constants.fixedUpdateMultiplier * constants.timeMultiplier;
            position += velocity * constants.fixedUpdateMultiplier * constants.timeMultiplier;

            transform.position = position / constants.scale;

            orientation += AngleToRotateOnlyZ(orientation, velocity);
            transform.rotation = Quaternion.Euler(orientation);
            
            uiManager.UpdateHeight(Mathf.Round(GetHeight(position) * 1f) / 1000, Mathf.Round(Magnitude(velocity) * 10) / 10, Mathf.Round(Magnitude(acceleration) * 10) / 10, Mathf.Round(orientation.z * 10) / 10);
        }
    }

    void StartDOI()
    {
        vc1 = Magnitude(velocity);
        DOI = true;
        Debug.Log("started DOI");
    }

    float GetHeight(Vector3 pos)
    {
        if (Magnitude(pos) > moon.moonRadius)
        {
            return Magnitude(pos) - moon.moonRadius;
        }
        else
        {
            return 0;
        }
    }

    float GetDistance(Vector3 pos1, Vector3 pos2)
    {
        float distance = 0;
        distance = Mathf.Sqrt(Mathf.Pow((pos1.x - pos2.x), 2) + Mathf.Pow((pos1.y - pos2.y), 2) +
                              Mathf.Pow((pos1.z - pos2.z), 2));
        return distance;
    }

    float Magnitude(Vector3 vector)
    {
        return (Mathf.Sqrt(Mathf.Pow(vector.x, 2) + Mathf.Pow(vector.y, 2) + Mathf.Pow(vector.z, 2)));
    }
    
    Vector3 GetGravityAcceleration(Vector3 posRocket, Vector3 posMoon) // Assuming the only force exerted on the rocket is the gravity of the moon
    {
        Vector3 acceleration = posMoon - posRocket;
        acceleration = acceleration.normalized * constants.gravConst * moon.moonMass / Mathf.Pow(GetDistance(posRocket, posMoon), 2);
        return acceleration;
    }

    Vector3 Accelerate(Vector3 orientation, float thrustPercentage)
    {
        if (LMFuelMass > 0)
        {
            float zOrientation = orientation.z * Mathf.PI / 180;
            Vector3 addedAcceleration = new Vector3(Mathf.Sin(zOrientation - Mathf.PI), Mathf.Cos(zOrientation), 0);
            addedAcceleration *= LMThrust * thrustPercentage * 0.01f / (LMDryMass + LMFuelMass);
            LMFuelMass -= LMMassFlowRate * thrustPercentage * 0.01f * constants.fixedUpdateMultiplier * constants.timeMultiplier;
            
            if (LMFuelMass < 0)
            {
                LMFuelMass = 0;
                accelerating = false;
            }

            uiManager.UpdateFuel(LMInitFuelMass, LMFuelMass);
            return addedAcceleration;
        }
        return constants.nullVector;
    }

    Vector3 AngleToRotateOnlyZ(Vector3 orientationAngles, Vector3 targetVector)
    {
        Vector3 targetAngles = constants.nullVector;
        Vector3 angleToRotate = constants.nullVector;


        // Calculate the global orientation of the target vector
        float x = targetVector.x;
        float y = targetVector.y;

        if (x == 0)
        {
            if (y > 0)
            {
                targetAngles.z = 0;
            }
            else
            {
                targetAngles.z = 180;
            }
        }
        else if (y == 0)
        {
            if (x > 0)
            {
                targetAngles.z = -90;
            }
            else
            {
                targetAngles.z = 90;
            }
        }
        else
        {

            float targetMagnitude = Magnitude(targetVector);
            targetAngles.z = 90 - Mathf.Asin(targetVector.y / targetMagnitude) * 180 / Mathf.PI;

            if (x > 0 && y < 0)
            {
                targetAngles.z *= -1;
            }
            else if (x > 0 && y > 0)
            {
                targetAngles.z *= -1;
            }
            else if (x < 0 && y < 0)
            {
                // nothing to do
            }
        }

        angleToRotate = targetAngles - orientationAngles;

        // Makes it less than 180 degrees
        for (int i = 0; i < 3; i++)
        {
            if (angleToRotate[i] > 180)
            {
                angleToRotate[i] -= 360;
            }
            else if (angleToRotate[i] < -180)
            {
                angleToRotate[i] += 360;
            }
        }

        return angleToRotate;
    }

    public void Unlink(Vector3 givenPosition, Vector3 givenVelocity, Vector3 givenOrientation)
    {
        linked = false;
        position = transform.position * constants.scale;
        position = position * Magnitude(givenPosition) / Magnitude(position);
        velocity = UpdateVelocity(givenVelocity, position);
        transform.parent = null;

        orientation = givenOrientation;
        transform.rotation = Quaternion.Euler(orientation);

    }

    private Vector3 UpdateVelocity(Vector3 givenVelocity, Vector3 givenPosition)
    {
        float xMultiplier = 0;
        float yMultiplier = 0;

        xMultiplier = -(givenPosition.y) / GetDistance(givenPosition, constants.nullVector);
        yMultiplier = givenPosition.x / GetDistance(givenPosition, constants.nullVector);

        return Magnitude(givenVelocity) * (new Vector3(xMultiplier, yMultiplier, 0));
    }

    private bool CheckConditions()
    {
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CheckConditions())
        {
            Debug.Log("landed");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
