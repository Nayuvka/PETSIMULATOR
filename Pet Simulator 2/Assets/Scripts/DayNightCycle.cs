using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public Light2D targetLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float duration = 2.0f;
    [SerializeField] private float DayNightDuration = 5f;

    // Add color parameters
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.2f, 0.2f, 0.5f, 1f); // Bluish night color

    public bool forceDayTime = false; // Toggle this in the Inspector for day
    public bool forceNightTime = false; // Toggle this in the Inspector for night
    private bool isDayNight = false;
    private float elapsedTime = 0f;

    void Update()
    {
        if (forceDayTime)
        {
            targetLight.intensity = maxIntensity; // Force full daylight
            targetLight.color = dayColor; // Force day color
            isDayNight = false; // Stop automatic cycling
            return;
        }

        if (forceNightTime)
        {
            targetLight.intensity = minIntensity; // Force full night
            targetLight.color = nightColor; // Force night color
            isDayNight = false; // Stop automatic cycling
            return;
        }

        if (!isDayNight)
        {
            SunriseSunset();
        }
    }

    void SunriseSunset()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.PingPong(elapsedTime / duration, 1);
        targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        targetLight.color = Color.Lerp(nightColor, dayColor, t); // Smoothly transition between colors

        if (t <= 0.01f || t >= 0.99f)
        {
            StartCoroutine(CyclePeak());
        }
    }

    IEnumerator CyclePeak()
    {
        isDayNight = true;
        yield return new WaitForSeconds(DayNightDuration);
        isDayNight = false;
    }
}