using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extras : MonoBehaviour
{
    /*
    public Constants constants;
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
    float AccelerationDirection(float angleToRotate, float angularVelocity, float angularAcceleration)
    {
        float angleRotatedAtStop = -1 * Mathf.Pow(angularVelocity, 2) / (2 * angularAcceleration);
        float angleToGo = angleToRotate - angleRotatedAtStop;
        if (angleToGo > 0 && angleToRotate > 0 || angleToGo >= 0 && angleToRotate < 0)
        {
            return 1;
        }
        return -1;
    }
    Vector3 GetAngularAcceleration(Vector3 targetVector, Vector3 angularVelocity, float angularAccelerationMultiplier)
    {
        Vector3 accelerationVector = constants.nullVector;
        //Vector3 angleToRotate = AngleToRotate(orientation, targetVector);
        Vector3 angleToRotate = AngleToRotateOnlyZ(orientation, targetVector);
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
            else
            {
                Debug.Log("Something fucked up");
            }
        }
        accelerationVector *= angularAccelerationMultiplier;
        return accelerationVector;
    }
    */
}
