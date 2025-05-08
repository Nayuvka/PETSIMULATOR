using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WorldTimeDisplay : MonoBehaviour
{
    [SerializeField]
    private WorldTime _worldTime;

    [SerializeField]
    private TMP_Text _text;

    private void Start()
    {
        // Make sure we have a reference to the text component
        if (_text == null)
        {
            _text = GetComponent<TMP_Text>();
        }

        // Make sure we have a reference to the WorldTime component
        if (_worldTime == null)
        {
            _worldTime = FindObjectOfType<WorldTime>();

            if (_worldTime == null)
            {
                Debug.LogError("WorldTimeDisplay: No WorldTime component found in the scene!");
                enabled = false;
                return;
            }
        }

        // Register event handler
        _worldTime.WorldTimeChanged += OnWorldTimeChanged;

        // Initialize with current time
        UpdateTimeDisplay(TimeSpan.Zero);
    }

    private void OnDestroy()
    {
        if (_worldTime != null)
        {
            _worldTime.WorldTimeChanged -= OnWorldTimeChanged;
        }
    }

    private void OnWorldTimeChanged(object sender, TimeSpan newTime)
    {
        UpdateTimeDisplay(newTime);
    }

    private void UpdateTimeDisplay(TimeSpan time)
    {
        if (_text != null)
        {
            // Format hours and minutes with leading zeros
            int hours = time.Hours;
            int minutes = time.Minutes;

            // Convert to 12-hour format with AM/PM
            string period = hours >= 12 ? "PM" : "AM";
            hours = hours % 12;
            if (hours == 0) hours = 12; // Convert 0 to 12 for 12 AM

            string timeString = string.Format("{0:D2}:{1:D2} {2}", hours, minutes, period);
            _text.text = timeString;

            // For debugging
            //Debug.Log($"Clock updated to: {timeString} (Raw time: {time})");
        }
        else
        {
            Debug.LogError("WorldTimeDisplay: No text component found!");
        }
    }
}