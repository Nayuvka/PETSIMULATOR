using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class WorldLight : MonoBehaviour
{
    private Light2D _light;

    [SerializeField]
    private WorldTime _worldTime;

    [SerializeField]
    private Gradient _gradient;

    [SerializeField]
    private float transitionSpeed = 1f; // Controls how smoothly light changes

    private float targetEvaluation;
    private float currentEvaluation;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _worldTime.WorldTimeChanged += OnWorldTimeChanged;
        currentEvaluation = PercentOfDay(TimeSpan.Zero);
    }

    private void OnDestroy()
    {
        _worldTime.WorldTimeChanged -= OnWorldTimeChanged;
    }

    private void OnWorldTimeChanged(object sender, TimeSpan newTime)
    {
        targetEvaluation = PercentOfDay(newTime);
    }

    private void Update()
    {
        // Smoothly interpolate between current and target evaluation
        currentEvaluation = Mathf.Lerp(currentEvaluation, targetEvaluation, Time.deltaTime * transitionSpeed);
        _light.color = _gradient.Evaluate(currentEvaluation);
    }

    private float PercentOfDay(TimeSpan timespan)
    {
        return (float)timespan.TotalMinutes % WorldTimeConstants.MinutesInDay / WorldTimeConstants.MinutesInDay;
    }
}