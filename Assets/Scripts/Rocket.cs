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
        acceleration = GetAcceleration(position, moon.position);
        // Debug.Log(acceleration);
        velocity += acceleration * constants.fixedUpdateMultiplier * constants.timeMultiplier;
        position += velocity * constants.fixedUpdateMultiplier * constants.timeMultiplier;

        transform.position = position / constants.scale;

        angularAcceleration = getAngularAcceleration(velocity, orientation, angularVelocity);
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
            if (orientationDifference[i] == 0)
            {
                if (angularVelocity[i] == 0)
                {
                    angularAcceleration[i] = 0;
                }
                else if (angularVelocity[i] > 0)
                {
                    angularAcceleration[i] = -1 * angularAccelerationMultiplier;
                }
                else
                {
                    angularAcceleration[i] = angularAccelerationMultiplier;
                }

            }
        }
    }
}
