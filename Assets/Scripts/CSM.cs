using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSM : MonoBehaviour
{

    private float initialVelocity = 1628f; // m.s^-1  1628f
    private float CSMDryMass = 23572; // kg

    public Moon moon;
    public Constants constants;

    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;
    private float accelerationMulitplier; //Engine acceleration depends on mass left in rocket

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
    }

    void FixedUpdate()
    {
        if (!linked)
        {
            acceleration = GetGravityAcceleration(position, moon.position);

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
        velocity = givenVelocity;
        transform.parent = null;

        orientation = givenOrientation;
        transform.rotation = Quaternion.Euler(orientation);
    }
}
