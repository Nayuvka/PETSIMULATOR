using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    [SerializeField]
    private float _dayLength; //seconds 

    private TimeSpan _currentTime;
    private float _minuteLength => _dayLength /1440; //no.of min in a day 
}
