using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    public float startingHeight = 111f * Mathf.Pow(10, 3); // m

    private float initialVelocity = 1628f; // m.s^-1  1628f
    private float lunarModuleDryMass = 4280; // kg
    private float lunarModuleFuelMass = 10920; // kg
    private float orbitingStageDryMass = 23572; // kg

    public Moon moon;
    public Constants constants;

    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;
    private float accelerationMulitplier; //Engine acceleration depends on mass left in rocket

    private Vector3 orientation;
    private Vector3 angularVelocity;
    private Vector3 angularAcceleration;
    private float angularAccelerationMultiplier; //RCS Boosters acceleration depends on mass left in rocket


    void Start()
    {
        position = new Vector3(startingHeight + moon.moonRadius, 0, 0);
        transform.position = position / constants.scale;
        velocity = new Vector3(0, initialVelocity, 0);

        orientation = new Vector3(0, 0, 90);
        transform.Rotate(orientation / constants.scale); // use 'tranform.Rotate(x);' not 'transform.rotation = x;' because using 3 angles not quaternions 
        angularVelocity = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {
        acceleration = GetAcceleration(position, moon.position);
        // Debug.Log(acceleration);
        velocity += acceleration * constants.fixedUpdateMultiplier * constants.timeMultiplier;
        position += velocity * constants.fixedUpdateMultiplier * constants.timeMultiplier;

        transform.position = position / constants.scale;

        //angularAcceleration = getAngularAcceleration(velocity.normalized); 
        angularVelocity += angularAcceleration * constants.fixedUpdateMultiplier * constants.timeMultiplier;
        orientation += angularVelocity * constants.fixedUpdateMultiplier * constants.timeMultiplier;

        transform.Rotate(orientation / constants.scale);
    }

    float GetDistance(Vector3 pos1, Vector3 pos2)
    {
        float distance = 0;
        distance = Mathf.Sqrt(Mathf.Pow((pos1.x - pos2.x), 2) + Mathf.Pow((pos1.y - pos2.y), 2) +
                              Mathf.Pow((pos1.z - pos2.z), 2));
        return distance;
    }

    Vector3 GetAcceleration(Vector3 posRocket, Vector3 posMoon) // Assuming the only force exerted on the rocket is the gravity of the moon
    {
        Vector3 acceleration = posMoon - posRocket;
        acceleration = acceleration.normalized * constants.gravConst * moon.moonMass / Mathf.Pow(GetDistance(posRocket, posMoon), 2);
        return acceleration;
    }

    Vector3 orientationVector(Vector3 orientation)
    {
        Vector3 orientationVector = new Vector3(-1 * Mathf.Sin(orientation[1]) * Mathf.Sin(orientation[2]), Mathf.Cos(orientation[0]) * Mathf.Cos(orientation[2]), Mathf.Cos(orientation[0]) * Mathf.Cos(orientation[1]));
        // Calculating the vector based on my hypotheses that the x value is not affected when rotated around the x axis, 
        //and that the vector depends on the multiplication of whatever method is used to calculate it individually, 
        //and those were teseted individually (explaines the sin(-z) for the x value)
        return orientationVector;
    }

    Vector3 angleToRotate(Vector3 orientationVector, Vector3 targetVector)
    {
        Vector3 orientationAngles = constants.nullVector;
        Vector3 targetAngles = constants.nullVector;
        Vector3 angleDifference = constants.nullVector;
        
        for (int i = 0; i < 3; i++) // i = 0 - x-angle, i = 1 - y-angle, i = 3 - z-angle
        { 
            if (orientationVector[(i+1)%3] == 0) // can't use arctan if the bottom value is 0 ie. it is vertical
            {
                orientationAngles[i] = 90 * orientationVector[(i+1)%3] / Mathf.Abs(orientationVector[(i+1)%3]);
            }
            else
            {
                orientationAngles[i] = Mathf.Atan(orientationVector[(i+2)%3] / orientationVector[(i+1)%3]);
            }
            // angles for target
            if (targetVector[(i+1)%3] == 0)
            {
                targetAngles[i] = 90 * targetVector[(i+1)%3] / Mathf.Abs(targetVector[(i+1)%3]);
            }
            else
            {
                targetAngles[i] = Mathf.Atan(targetVector[(i+2)%3] / targetVector[(i+1)%3]);
            }
            // angles difference
            angleDifference[i] = Mathf.Abs(targetAngles[i] - orientationAngles[i]);
            if (angleDifference[i] > 180) // to have an angle under 180 degres --> be efficient when rotating
            {
                angleDifference[i] = 360 - angleDifference[i];
            }
        }
        return angleDifference;
    }

}
    
    /*
    Vector3 angularAccelerationDirection()
    {
        Vector3 angleRotatedUntilStop = constants.nullVector;
        Vector3 rotationTillVelocity = constants.nullVector;
        Vector3 angularAccelerationDirection = constants.nullVector;
        for (int i = 0; i < 3; i++)
        {
            angleRotatedUntilStop[i] = -1 * Mathf.Pow(angularVelocity[i], 2) / (2 * angularAcceleration[i]);
            rotationTillVelocity[i] = orientationDifference[i] - angleRotatedUntilStop[i];
            if (rotationTillVelocity[i] < 0) { angularAccelerationDirection[i] = -1; }
            else if (rotationTillVelocity[i] > 0) { angularAccelerationDirection[i] = 1; }
        }
        return angularAccelerationDirection;
    }

    Vector3 getAngularAcceleration(Vector3 targetOrientation)
    {
        Vector3 angularAcceleration = constants.nullVector;
        Vector3 orientationDifference; //How do we fkn calculate this?!
        for (int i = 0; i < 3; i++)
        {
            if (orientationDifference[i] == 0 && angularVelocity[i] == 0)
            {
                angularAcceleration[i] = 0;
            }
            else if (orientationDifference[i] == 0 && angularVelocity[i] < 0)
            {
                angularAcceleration[i] = angularAccelerationMultiplier;
            }
            else if (orientationDifference[i] == 0 && angularVelocity[i] > 0)
            {
                angularAcceleration[i] = -1 * angularAccelerationMultiplier;
            }
            else if (orientationDifference[i] < 0 && angularVelocity[i] = 0) { }

        }
    }
    */

