using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    public float startingHeight = 111f * Mathf.Pow(10, 3); // m

    private float initialVelocity = 1613f; // m.s^-1
    private float lunarModuleDryMass = 4280; // kg
    private float lunarModuleFuelMass = 10920; // kg
    private float orbitingStageDryMass = 11900; // kg
    
    public Moon moon;
    public Constants constants;

    private Vector3 position = new Vector3(0,0,0);
    private Vector3 velocity = new Vector3(0,0,0);
    private Vector3 acceleration = new Vector3(0,0,0);
    
    private Vector3 orientation = new Vector3(0,0,0);
    private Vector3 angularVelocity = new Vector3(0,0,0);
    private Vector3 angularAcceleration = new Vector3(0,0,0);


    void Start()
    {
        position = new Vector3(startingHeight + moon.moonRadius, 0, 0);
        transform.position = position / constants.scale;
        Debug.Log(transform.position);
    }

    void FixedUpdate()
    {
        position += velocity;
        velocity += acceleration;

        transform.position = position / constants.scale;
    }
    
    float GetDistance(Vector3 pos1, Vector3 pos2)
    {
        float distance = 0;
        distance = Mathf.Sqrt(Mathf.Pow((pos1.x - pos2.x), 2) + Mathf.Pow((pos1.y - pos2.y), 2) +
                              Mathf.Pow((pos1.z - pos2.z), 2));
        return distance;
    }

    float GetGravity(float mass1, float mass2, float distance)
    {
        float G = 0;
        G = constants.gravConst * mass1 * mass2 / Mathf.Pow(distance, 2);
        return G;
    }
}
