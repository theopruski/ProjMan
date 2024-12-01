using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public ParticleSystem RainParticle; // Système de particules pour la pluie

    public float minRateOverTime = 100f; // Minimum pour "Rate Over Time"
    public float maxRateOverTime = 1000f; // Maximum pour "Rate Over Time"
    public float minVelocityY = 25f;    // Vélocité minimale en Y (vitesse de la pluie)
    public float maxVelocityY = 35f;    // Vélocité maximale en Y (vitesse de la pluie)
    public float minDelay = 3f;         // Délai minimal entre les changements
    public float maxDelay = 10f;        // Délai maximal entre les changements

    // Modules pour contrôler les propriétés des particules
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;

    public bool IsRaining { get; private set; } // Propriété pour vérifier si la pluie est active
    public bool IsFoggy { get; private set; }   // Propriété pour vérifier si le brouillard est actif

    void Start()
    {
        // Accéder aux modules
        emissionModule = RainParticle.emission;
        velocityModule = RainParticle.velocityOverLifetime;

        // Valeurs initiales de la vitesse sur l'axe Y
        velocityModule.y = new ParticleSystem.MinMaxCurve(minVelocityY, maxVelocityY);

        // Assurez-vous que le brouillard est désactivé au départ
        RenderSettings.fog = false;

        // Démarrer la gestion de la pluie et du brouillard
        StartCoroutine(ManageWeather());
    }

    // Gestion de l'apparition et la disparition de la pluie et du brouillard 
    IEnumerator ManageWeather()
    {
        while (true)
        {
            // Action activer/désactiver le brouillard ou la pluie
            int action = Random.Range(0, 4);

            switch (action)
            {
                case 0: // Démarrer la pluie si le brouillard est actif
                    if (IsFoggy)
                    {
                        StartRain();
                    }
                    break;
                case 1: // Arrêter la pluie
                    StopRain();
                    break;
                case 2: // Activer le brouillard
                    StartFog();
                    break;
                case 3: // Désactiver le brouillard
                    StopFog();
                    break;
            }

            // Délai aléatoire avant la prochaine action
            float delay = Random.Range(minDelay, maxDelay);
            // Attente pendant un délai aléatoire pour eviter de bloquer le fonctionnement global du jeu
            yield return new WaitForSeconds(delay);
        }
    }

    // Méthode pour démarrer la pluie
    public void StartRain()
    {
        if (!RainParticle.isPlaying && IsFoggy)
        {
            RainParticle.Play();
        }
        IsRaining = true;
    }

    // Méthode pour arrêter la pluie
    public void StopRain()
    {
        if (RainParticle.isPlaying)
        {
            RainParticle.Stop();
        }
        IsRaining = false;
    }

    // Méthode pour démarrer le brouillard
    public void StartFog()
    {
        IsFoggy = true;
        RenderSettings.fog = true;
        AdjustFogIntensity();
        if (RainParticle.isPlaying)
        {
            IsRaining = true;
        }
    }

    // Méthode pour arrêter le brouillard
    public void StopFog()
    {
        IsFoggy = false;
        RenderSettings.fog = false;
        StopRain();
    }

    // Ajuster l'intensité du brouillard
    void AdjustFogIntensity()
    {
        if (IsFoggy)
        {
            // Changer la densité du brouillard
            RenderSettings.fogDensity = Random.Range(0.01f, 0.1f);
        }
    }

    // Ajuster l'intensité de la pluie
    void AdjustRainIntensity()
    {
        if (RainParticle.isPlaying)
        {
            // Modification de "Rate Over Time" (taux d'émission des gouttes)
            float newRateOverTime = Random.Range(minRateOverTime, maxRateOverTime);
            emissionModule.rateOverTime = newRateOverTime;

            // Modification de "Velocity Over Lifetime" en Y (vitesse des gouttes)
            float newMinVelocityY = Random.Range(minVelocityY - 5, minVelocityY); // Variation du minimum
            float newMaxVelocityY = Random.Range(maxVelocityY, maxVelocityY + 5); // Variation du maximum

            velocityModule.y = new ParticleSystem.MinMaxCurve(newMinVelocityY, newMaxVelocityY);
        }
    }
}
