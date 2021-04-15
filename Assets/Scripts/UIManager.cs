using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Constants constants;
    public TextMeshProUGUI timeTxt;
    public TextMeshProUGUI fuelTxt;
    public TextMeshProUGUI heightTxt;
    public TextMeshProUGUI speedTxt;
    public TextMeshProUGUI accelerationTxt;
    public TextMeshProUGUI orientationTxt;
    public float time = 0;
    
    void Start()
    {
        if (constants == null)
        {
            constants = GameObject.FindGameObjectWithTag("Constants").GetComponent<Constants>();
        }
    
        StartCoroutine((Timer()));
    }

    void FixedUpdate()
    {
        time += 1 * constants.fixedUpdateMultiplier * constants.timeMultiplier;
    }

    string TimeConverter(float time)
    {
        int hours = 0;
        string hoursTxt = "";
        int mins = 0;
        string minsTxt = "";
        int secs = (int)time;
        string secsTxt = "";
        
        hours = secs / 3600;
        mins = (secs - hours * 3600) / 60;
        secs = secs - hours * 3600 - mins * 60;

        if (hours < 10)
        {
            hoursTxt = "0";
        }
        else
        {
            hoursTxt = "";
        }
        
        if (mins < 10)
        {
            minsTxt = "0";
        }
        else
        {
            minsTxt = "";
        }
        
        if (secs < 10)
        {
            secsTxt = "0";
        }
        else
        {
            secsTxt = "";
        }
        
        return (hoursTxt + hours + " h " + minsTxt + mins + " m " + secsTxt + secs + " s");
    }
    
    private IEnumerator Timer()
    {
        while(true)
        {
            timeTxt.text = "time: " + TimeConverter(time);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void UpdateFuel(float inititalFuel, float currentFuel)
    {
        fuelTxt.text = "fuel: " + Mathf.Round(currentFuel / inititalFuel * 1000) / 10 + "%";
    }

    public void UpdateHeight(float newHeight, float newSpeed, float newAcceleration, float newOrientation)
    {
        heightTxt.text = "height: " + newHeight + " km";
        speedTxt.text = "speed: " + newSpeed;
        accelerationTxt.text = "acc: " + newAcceleration;
        orientationTxt.text = "orientation: " + newOrientation;
    }
}
