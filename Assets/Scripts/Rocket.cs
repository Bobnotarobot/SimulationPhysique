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

        orientation = new Vector3(0, 0, 0);
        transform.Rotate(orientation / constants.scale); // use 'tranform.Rotate(x);' not 'transform.rotation = x;' because using 3 angles not quaternions 
        angularVelocity = new Vector3(0, 0, 0);

    }

    void FixedUpdate()
    {
        acceleration = GetGravityAcceleration(position, moon.position);
        velocity += acceleration * constants.fixedUpdateMultiplier * constants.timeMultiplier;
        position += velocity * constants.fixedUpdateMultiplier * constants.timeMultiplier;

        transform.position = position / constants.scale;

        angularAccelerationMultiplier = 1960 / lunarModuleFuelMass; // 4 thursters of 490N each
        angularAcceleration = GetAngularAcceleration(velocity, angularVelocity, angularAccelerationMultiplier); 
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

    Vector3 AngleToRotate(Vector3 orientationAngles, Vector3 targetVector)
    {
        Vector3 targetAngles = constants.nullVector;
        Vector3 angleToRotate = constants.nullVector;
        
        // Calculate the global orientation of the target vector
        float targetMagnitude = Magnitude(targetVector);
        for (int i = 0; i < 3; i++)
        {
            targetAngles[i] = Mathf.Acos(targetVector[i] / targetMagnitude);
        }
        
        targetAngles *= 180 / Mathf.PI;
        angleToRotate = targetAngles - orientationAngles;
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

    
    float AccelerationDirection( float angleToRotate, float angularVelocity, float angularAcceleration)
    {
        float angleRotatedAtStop = -1 * Mathf.Pow(angularVelocity, 2) / (2 * angularAcceleration);
        float angleToGo = angleToRotate - angleRotatedAtStop;
        if (angleToGo > 0 && angleToRotate > 0 || angleToGo >= 0 && angleToRotate < 0)
        {
            return 1;
        }
        return 0;
    }
        
    Vector3 GetAngularAcceleration(Vector3 targetVector, Vector3 angularVelocity, float angularAccelerationMultiplier)
    {
        Vector3 accelerationVector = constants.nullVector;
        Vector3 angleToRotate = AngleToRotate(orientation, targetVector);
        for (int i = 0; i < 3; i++) 
        {
            if (angleToRotate[i] == 0 && angularVelocity[i] == 0)
            {
                accelerationVector[i] = 0;
            }
            else if (angleToRotate[i] == 0 && angularVelocity[i] < 0)
            {
                accelerationVector[i] = 1;
            }
            else if (angleToRotate[i] == 0 && angularVelocity[i] > 0)
            {
                accelerationVector[i] = -1;
            }
            else if (angleToRotate[i] < 0 && angularVelocity[i] == 0)
            {
                accelerationVector[i] = -1;
            }
            else if (angleToRotate[i] > 0 && angularVelocity[i] == 0)
            {
                accelerationVector[i] = 1;
            }
            else if (angleToRotate[i] < 0 && angularVelocity[i] < 0)
            {
                accelerationVector[i] = AccelerationDirection(angleToRotate[i], angularVelocity[i], angularAccelerationMultiplier);
            }
            else if (angleToRotate[i] > 0 && angularVelocity[i] > 0)
            {
                accelerationVector[i] = AccelerationDirection(angleToRotate[i], angularVelocity[i], -angularAccelerationMultiplier);
            }
        }
        accelerationVector *= angularAccelerationMultiplier;
        return accelerationVector;
    }




}
    


