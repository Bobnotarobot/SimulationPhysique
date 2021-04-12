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

    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;
    private float accelerationMulitplier; //Engine acceleration depends on mass left in rocket

    private bool accelerating = true;
    private bool linked = true;

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
    }

    void FixedUpdate()
    {
        if (!linked)
        {
            acceleration = GetGravityAcceleration(position, moon.position);
        
            if (accelerating)
            {
                acceleration += Accelerate(orientation);   
            }

            velocity += acceleration * constants.fixedUpdateMultiplier * constants.timeMultiplier;
            position += velocity * constants.fixedUpdateMultiplier * constants.timeMultiplier;

            transform.position = position / constants.scale;

            orientation += AngleToRotateOnlyZ(orientation, velocity);
            transform.rotation = Quaternion.Euler(orientation);   
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

    Vector3 Accelerate(Vector3 orientation)
    {
        if (LMFuelMass > 0)
        {
            float zOrientation = orientation.z * Mathf.PI / 180;
            Vector3 addedAcceleration = new Vector3(Mathf.Sin(zOrientation - Mathf.PI), Mathf.Cos(zOrientation), 0);
            addedAcceleration *= LMThrust / (LMDryMass + LMFuelMass);
            LMFuelMass -= LMMassFlowRate * constants.fixedUpdateMultiplier * constants.timeMultiplier;
            
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
}
