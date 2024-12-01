using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public ParticleSystem RainParticle; // Syst�me de particules pour la pluie

    public float minRateOverTime = 100f; // Minimum pour "Rate Over Time"
    public float maxRateOverTime = 1000f; // Maximum pour "Rate Over Time"
    public float minVelocityY = 25f;    // V�locit� minimale en Y (vitesse de la pluie)
    public float maxVelocityY = 35f;    // V�locit� maximale en Y (vitesse de la pluie)
    public float minDelay = 3f;         // D�lai minimal entre les changements
    public float maxDelay = 10f;        // D�lai maximal entre les changements

    // Modules pour contr�ler les propri�t�s des particules
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;

    public bool IsRaining { get; private set; } // Propri�t� pour v�rifier si la pluie est active
    public bool IsFoggy { get; private set; }   // Propri�t� pour v�rifier si le brouillard est actif

    void Start()
    {
        // Acc�der aux modules
        emissionModule = RainParticle.emission;
        velocityModule = RainParticle.velocityOverLifetime;

        // Valeurs initiales de la vitesse sur l'axe Y
        velocityModule.y = new ParticleSystem.MinMaxCurve(minVelocityY, maxVelocityY);

        // Assurez-vous que le brouillard est d�sactiv� au d�part
        RenderSettings.fog = false;

        // D�marrer la gestion de la pluie et du brouillard
        StartCoroutine(ManageWeather());
    }

    // Gestion de l'apparition et la disparition de la pluie et du brouillard 
    IEnumerator ManageWeather()
    {
        while (true)
        {
            // Action activer/d�sactiver le brouillard ou la pluie
            int action = Random.Range(0, 4);

            switch (action)
            {
                case 0: // D�marrer la pluie si le brouillard est actif
                    if (IsFoggy)
                    {
                        StartRain();
                    }
                    break;
                case 1: // Arr�ter la pluie
                    StopRain();
                    break;
                case 2: // Activer le brouillard
                    StartFog();
                    break;
                case 3: // D�sactiver le brouillard
                    StopFog();
                    break;
            }

            // D�lai al�atoire avant la prochaine action
            float delay = Random.Range(minDelay, maxDelay);
            // Attente pendant un d�lai al�atoire pour eviter de bloquer le fonctionnement global du jeu
            yield return new WaitForSeconds(delay);
        }
    }

    // M�thode pour d�marrer la pluie
    public void StartRain()
    {
        if (!RainParticle.isPlaying && IsFoggy)
        {
            RainParticle.Play();
        }
        IsRaining = true;
    }

    // M�thode pour arr�ter la pluie
    public void StopRain()
    {
        if (RainParticle.isPlaying)
        {
            RainParticle.Stop();
        }
        IsRaining = false;
    }

    // M�thode pour d�marrer le brouillard
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

    // M�thode pour arr�ter le brouillard
    public void StopFog()
    {
        IsFoggy = false;
        RenderSettings.fog = false;
        StopRain();
    }

    // Ajuster l'intensit� du brouillard
    void AdjustFogIntensity()
    {
        if (IsFoggy)
        {
            // Changer la densit� du brouillard
            RenderSettings.fogDensity = Random.Range(0.01f, 0.1f);
        }
    }

    // Ajuster l'intensit� de la pluie
    void AdjustRainIntensity()
    {
        if (RainParticle.isPlaying)
        {
            // Modification de "Rate Over Time" (taux d'�mission des gouttes)
            float newRateOverTime = Random.Range(minRateOverTime, maxRateOverTime);
            emissionModule.rateOverTime = newRateOverTime;

            // Modification de "Velocity Over Lifetime" en Y (vitesse des gouttes)
            float newMinVelocityY = Random.Range(minVelocityY - 5, minVelocityY); // Variation du minimum
            float newMaxVelocityY = Random.Range(maxVelocityY, maxVelocityY + 5); // Variation du maximum

            velocityModule.y = new ParticleSystem.MinMaxCurve(newMinVelocityY, newMaxVelocityY);
        }
    }
}
