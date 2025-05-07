using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(TMP_Text))]
public class WorldTimeDisplay : MonoBehaviour
{
    [SerializeField]
    private WorldTime _worldTime;

    private TMP_Text _text;

    [SerializeField]
    private float updateSpeed = 2f; // Controls how smoothly the time display updates

    private TimeSpan _targetTime;
    private TimeSpan _displayTime;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _worldTime.WorldTimeChanged += OnWorldTimeChanged;
    }

    private void OnDestroy()
    {
        _worldTime.WorldTimeChanged -= OnWorldTimeChanged;
    }

    private void OnWorldTimeChanged(object sender, TimeSpan newTime)
    {
        _targetTime = newTime;
    }

    private void Update()
    {
        // Smoothly update display time toward target time
        if (_displayTime != _targetTime)
        {
            // Gradually move display time towards target time
            int currentTotalMinutes = (int)_displayTime.TotalMinutes;
            int targetTotalMinutes = (int)_targetTime.TotalMinutes;

            if (currentTotalMinutes < targetTotalMinutes)
            {
                _displayTime = TimeSpan.FromMinutes(currentTotalMinutes + Time.deltaTime * updateSpeed);
                if (_displayTime.TotalMinutes > _targetTime.TotalMinutes)
                    _displayTime = _targetTime;
            }

            _text.SetText(_displayTime.ToString("hh\\:mm"));
        }
    }
}