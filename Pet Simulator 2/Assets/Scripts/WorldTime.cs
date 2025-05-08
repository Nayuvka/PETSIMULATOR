using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    public event EventHandler<TimeSpan> WorldTimeChanged;

    [SerializeField]
    private float dayLength = 300f; // seconds (default 5 minutes real time = 1 day in game)

    [SerializeField]
    private int startHour = 6; // Start the day at 6 AM

    private TimeSpan _currentTime;

    // Property to calculate minute length based on day length
    private float minuteLength => dayLength / WorldTimeConstants.MinutesInDay;

    [SerializeField]
    private float timeMultiplier = 1f; // Use this to slow down or speed up time

    private void Start()
    {
        // Initialize time to starting hour
        _currentTime = TimeSpan.FromHours(startHour);

        // Trigger initial time event
        WorldTimeChanged?.Invoke(this, _currentTime);

        // Debug to verify events are working
        Debug.Log($"WorldTime initialized with time: {_currentTime}");

        StartCoroutine(AddMinute());
    }

    private IEnumerator AddMinute()
    {
        // Use timeMultiplier to adjust the wait time
        yield return new WaitForSeconds(minuteLength * timeMultiplier);

        _currentTime += TimeSpan.FromMinutes(1);

        // Handle day rollover
        if (_currentTime.TotalHours >= 24)
        {
            _currentTime = TimeSpan.FromMinutes(_currentTime.TotalMinutes % (24 * 60));
        }

        WorldTimeChanged?.Invoke(this, _currentTime);
        Debug.Log($"Time updated to: {_currentTime}");

        StartCoroutine(AddMinute());
    }

    // Method to adjust time speed at runtime
    public void SetTimeMultiplier(float multiplier)
    {
        timeMultiplier = Mathf.Max(0.1f, multiplier); // Prevent negative or too small values
    }

    // Public method to manually set the time
    public void SetTime(int hours, int minutes)
    {
        hours = Mathf.Clamp(hours, 0, 23);
        minutes = Mathf.Clamp(minutes, 0, 59);

        _currentTime = new TimeSpan(hours, minutes, 0);
        WorldTimeChanged?.Invoke(this, _currentTime);

        Debug.Log($"Time manually set to: {_currentTime}");
    }
}