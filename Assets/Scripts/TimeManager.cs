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

    private int minutes;

    public int Minutes
    { get { return minutes; } set { minutes = value; OnMinutesChange(value); } }

    private int hours = 11;

    public int Hours
    { get { return hours; } set { hours = value; OnHoursChange(value); } }

    private float tempSecond;

    public Light directionalLight;
    public float dayLength = 1440f; // Durée d'une journée en secondes

    public void Update()
    {
        tempSecond += Time.deltaTime;

        if (tempSecond >= 1)
        {
            Minutes += 1;
            tempSecond = 0;
        }

        float time = Time.time / dayLength; // Temps normalisé entre 0 et 1
        float angle = time * 180f; // Convertir en angle de rotation

        directionalLight.transform.rotation = Quaternion.Euler(50 + angle, -30, 0);

    }

    private void OnMinutesChange(int value)
    {
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

        UnityEngine.Debug.Log("Heure actuelle : " + value);

        if (value == 6)
        {
            lightIntensity = 0.5f;
            lightGradient = graddientNightToSunrise;
            skyboxA = skyboxNight;
            skyboxB = skyboxSunrise;
        }
        else if (value == 10)
        {
            lightIntensity = 0.8f;
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
        else
        {
            // Maintain current light intensity
            lightIntensity = directionalLight.intensity;
        }

        if (skyboxA != null && skyboxB != null && lightGradient != null)
        {
            StartCoroutine(LerpSkybox(skyboxA, skyboxB, lerpTime));
            StartCoroutine(LerpLight(lightGradient, directionalLight.intensity, lightIntensity, lerpTime));
        }

        directionalLight.intensity = lightIntensity;
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
            directionalLight.color = lightGradient.Evaluate(i / time);
            intensityLerp = Mathf.Lerp(intensityStart, intensityEnd, i / time);
            directionalLight.intensity = intensityLerp;
            RenderSettings.fogColor = directionalLight.color;
            yield return null;
        }
        directionalLight.intensity = intensityEnd;
    }
}