using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    public float startingHeight = 111f * Mathf.Pow(10, 3); // m

    private float initialVelocity = 1628f; // m.s^-1
    private float lunarModuleDryMass = 4280; // kg
    private float lunarModuleFuelMass = 10920; // kg
    private float orbitingStageDryMass = 23572; // kg
    
    public Moon moon;
    public Constants constants;

    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;
    
    private Vector3 orientation;
    private Vector3 angularVelocity;
    private Vector3 angularAcceleration;


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
        velocity += acceleration * constants.fixedUpdateMultiplier * constants.timeMultiplier;
        position += velocity * constants.fixedUpdateMultiplier * constants.timeMultiplier;

        transform.position = position / constants.scale;

        angularAcceleration = GetAngularAcceleration();
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

    Vector3 GetAngularAcceleration()
    {
        Vector3 placeholder = new Vector3(0, 0, 0);
        return placeholder;
    }
}
