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

    private int minutes;

    public int Minutes
    { get { return minutes; } set { minutes = value; OnMinutesChange(value); } }

    private int hours = 8;

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
        if (value == 6)
        {
            StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, 1));
            StartCoroutine(LerpLight(graddientNightToSunrise, 1));
        }
        else if (value == 10)
        {
            StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, 1));
            StartCoroutine(LerpLight(graddientSunriseToDay, 1));
        }
        else if (value == 18)
        {
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, 1));
            StartCoroutine(LerpLight(graddientDayToSunset, 1));
        }
        else if (value == 22)
        {
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, 1));
            StartCoroutine(LerpLight(graddientSunsetToNight, 1));
        }
        else if (value == 24)
        {
            value = 0;
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

    private IEnumerator LerpLight(Gradient lightGradient, float time)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            globalLight.color = lightGradient.Evaluate(i / time);
            RenderSettings.fogColor = globalLight.color;
            yield return null;
        }
    }
}