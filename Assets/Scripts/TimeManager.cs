using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Material skyboxNight;
    [SerializeField] private Material skyboxSunrise;
    [SerializeField] private Material skyboxDay;
    [SerializeField] private Material skyboxSunset;

    [SerializeField] private Gradient graddientNightToSunrise;
    [SerializeField] private Gradient graddientSunriseToDay;
    [SerializeField] private Gradient graddientDayToSunset;
    [SerializeField] private Gradient graddientSunsetToNight;

    [SerializeField] private Light globalLight;
    [SerializeField] private Light directionalLight;

    private int minutes;

    public int Minutes
    { get { return minutes; } set { minutes = value; OnMinutesChange(value); } }

    private int hours = 2;

    public int Hours
    { get { return hours; } set { hours = value; OnHoursChange(value); } }

    private float tempSecond;

    public void Update()
    {
        tempSecond += Time.deltaTime;

        if (tempSecond >= 1)
        {
            Minutes += 1;
            tempSecond = 0;
        }

    }

    private void OnMinutesChange(int value)
    {
        globalLight.transform.Rotate(Vector3.up, (1f / (1440f / 4f)) * 360f, Space.World);
        if (value >= 60)
        {
            Hours++;
            minutes = 0;
        }
        if (Hours >= 24)
        {
            Hours = 0;
        }
    }

    private void OnHoursChange(int value)
    {
        if (value == 24) { value = 0; }
        float lightIntensity = 0;
        Gradient lightGradient = null;
        Material skyboxA = null;
        Material skyboxB = null;
        float lerpTime = 1;

        if (value == 6)
        {
            lightIntensity = 0.5f;
            lightGradient = graddientNightToSunrise;
            skyboxA = skyboxNight;
            skyboxB = skyboxSunrise;
        }
        else if (value == 10)
        {
            lightIntensity = 1;
            lightGradient = graddientSunriseToDay;
            skyboxA = skyboxSunrise;
            skyboxB = skyboxDay;
        }
        else if (value == 18)
        {
            lightIntensity = 0.5f;
            lightGradient = graddientDayToSunset;
            skyboxA = skyboxDay;
            skyboxB = skyboxSunset;
        }
        else if (value == 22)
        {
            lightIntensity = 0.001f;
            lightGradient = graddientSunsetToNight;
            skyboxA = skyboxSunset;
            skyboxB = skyboxNight;
        }

        if (skyboxA != null && skyboxB != null && lightGradient != null)
        {
            StartCoroutine(LerpSkybox(skyboxA, skyboxB, lerpTime));
            StartCoroutine(LerpLight(lightGradient, directionalLight.intensity, lightIntensity, lerpTime));
        }
    }

    private IEnumerator LerpSkybox(Material a, Material b, float time)
    {
        RenderSettings.skybox = a;
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.Lerp(a, b, i / time);
            yield return null;
        }
        RenderSettings.skybox = b;
    }

    private IEnumerator LerpLight(Gradient lightGradient, float intensityStart, float intensityEnd, float time)
    {
        float intensityLerp = 0;
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            globalLight.color = lightGradient.Evaluate(i / time);
            intensityLerp = Mathf.Lerp(intensityStart, intensityEnd, i / time);
            directionalLight.intensity = intensityLerp;
            RenderSettings.fogColor = globalLight.color;
            yield return null;
        }
        directionalLight.intensity = intensityEnd;
    }
}