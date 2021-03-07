﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Constants constants;
    public TextMeshProUGUI timeTxt;
    public float time = 0;
    
    void Start()
    {
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
        
        return (hoursTxt + hours + " h " + minsTxt + mins + " mins " + secsTxt + secs + " secs");
    }
    
    private IEnumerator Timer()
    {
        while(true)
        {
            timeTxt.text = "time: " + TimeConverter(time);
            yield return new WaitForSeconds(0.1f);
        }
    }
}