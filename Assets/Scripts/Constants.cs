using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public float scale = 1000; // meters per unity unit
    public float timeMultiplier = 100;
    public float fixedUpdateMultiplier = 0.02f; //Fixed Update runs at 50 fps
    
    public float gravConst = 6.67408f * Mathf.Pow(10,-11); // m^3.kg^-1.s^-2

    public Vector3 nullVector = new Vector3(0, 0, 0);
}
