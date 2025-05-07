using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    public event EventHandler<TimeSpan> WorldTimeChanged;

    [SerializeField]
    private float dayLength = 300f; // seconds (default 5 minutes real time = 1 day in game)

    private TimeSpan _currentTime;

    // Property to calculate minute length based on day length
    private float minuteLength => dayLength / WorldTimeConstants.MinutesInDay;

    [SerializeField]
    private float timeMultiplier = 1f; // Use this to slow down or speed up time

    private void Start()
    {
        StartCoroutine(AddMinute());
    }

    private IEnumerator AddMinute()
    {
        _currentTime += TimeSpan.FromMinutes(1);
        WorldTimeChanged?.Invoke(this, _currentTime);

        // Use timeMultiplier to adjust the wait time
        yield return new WaitForSeconds(minuteLength * timeMultiplier);

        StartCoroutine(AddMinute());
    }

    // Method to adjust time speed at runtime
    public void SetTimeMultiplier(float multiplier)
    {
        timeMultiplier = Mathf.Max(0.1f, multiplier); // Prevent negative or too small values
    }
}