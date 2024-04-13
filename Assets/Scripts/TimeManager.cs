using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // image pour représenter les different étape du ciel
    [SerializeField] private Material skyboxNight;
    [SerializeField] private Material skyboxSunrise;
    [SerializeField] private Material skyboxDay;
    [SerializeField] private Material skyboxSunset;
    // Couleur de la lumière
    [SerializeField] private Gradient graddientNightToSunrise;
    [SerializeField] private Gradient graddientSunriseToDay;
    [SerializeField] private Gradient graddientDayToSunset;
    [SerializeField] private Gradient graddientSunsetToNight;

    // variable stocker de la minutes
    private int minutes;

    // propriéter pour modifier la valeur de la minute
    public int Minutes
    { get { return minutes; } set { minutes = value; OnMinutesChange(value); } }


    // variable stocker de l'heure, commence en 10
    private int hours = 10;

    // propriéter pour modifier la valeur de l'heure
    public int Hours
    { get { return hours; } set { hours = value; OnHoursChange(value); } }

    // variable float temporaire le temps de le transformer en minute
    private float tempSecond;

    // direction de la lumière
    public Light directionalLight;
    // Durée d'une journée en secondes
    public float dayLength; 


    public void Update()
    {
        // ajout le temps de framme dans dans tempSecond
        tempSecond += Time.deltaTime;

        // Le tempSecond est converti en Minutes
        if (tempSecond >= 1)
        {
            Minutes += 1;
            tempSecond = 0;
        }

        // Temps normalisé entre 0 et 1
        float time = Time.time / dayLength;
        // Convertir en angle de rotation
        float angle = time * 360f; 

        // déplace la rotation de la lumière selon le temps écouler.
        directionalLight.transform.rotation = Quaternion.Euler(50 + angle, -30, 0);

    }

    // donne une heure quand les minutes atteints 60
    private void OnMinutesChange(int value)
    {
        if (value >= 60)
        {
            Hours++;
            minutes = 0;
        }
    }

    // differents changement dans la journer au fur à mesure que les heures passe
    private void OnHoursChange(int value)
    {
        // retourne à 0 une fois que les heures atteints 24
        if (value == 24) { value = 0; }

        // l'intensiter de la lumière
        float lightIntensity = 0;
        // variable du gradient de la lumière
        Gradient lightGradient = null;
        // la texture du ciel actuel
        Material skyboxA = RenderSettings.skybox;
        // La texture suivante
        Material skyboxB = null;

        // passe en en lever de soleil si il est 6 ehure
        if (value == 6)
        {
            lightIntensity = 0.5f;
            lightGradient = graddientNightToSunrise;
            skyboxA = skyboxNight;
            skyboxB = skyboxSunrise;
        }
        // passe en journer si il est 10 heure
        else if (value == 10)
        {
            lightIntensity = 0.8f;
            lightGradient = graddientSunriseToDay;
            skyboxA = skyboxSunrise;
            skyboxB = skyboxDay;
        }
        // passe en coucher de soleil si il est 18 heure
        else if (value == 18)
        {
            lightIntensity = 0.5f;
            lightGradient = graddientDayToSunset;
            skyboxA = skyboxDay;
            skyboxB = skyboxSunset;
        }
        // passe en nuit si il est 22 heure
        else if (value == 22)
        {
            lightIntensity = 0.001f;
            lightGradient = graddientSunsetToNight;
            skyboxA = skyboxSunset;
            skyboxB = skyboxNight;
        }
        else
        {
            // Maintien de l'intensité lumineuse actuelle,
            // permet de resoudre le bug de la lumière qui disparait lors du passage d'une heure
            lightIntensity = directionalLight.intensity;
        }

        // appelle les fonctions pour mettre à jour le gradient et la texture de la skybox
        if (skyboxA != null && skyboxB != null && lightGradient != null)
        {
            StartCoroutine(LerpSkybox(skyboxA, skyboxB, 0));
            StartCoroutine(LerpLight(lightGradient, directionalLight.intensity, lightIntensity, 0));
        }

        // met à jour l'intensiter de la lumière
        directionalLight.intensity = lightIntensity;
    }

    // met à jour la texture du ciel
    private IEnumerator LerpSkybox(Material a, Material b, float time)
    {
        // définie la texture actuel
        RenderSettings.skybox = a;
        // boucle pour vérifier quand changer la texture
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.Lerp(a, b, i / time);
            yield return null;
        }
        // définie la texture de remplacement
        RenderSettings.skybox = b;
    }

    // définie l'intensiter de la lumière et la couleur de la lumière
    private IEnumerator LerpLight(Gradient lightGradient, float intensityStart, float intensityEnd, float time)
    {
        // variable d'interpolation de l'intensiter de la lumière
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