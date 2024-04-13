using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBusAI : MonoBehaviour
{
    public GameObject vehiculePrefab; // référence au préfab du véhicule
    public float spawnInterval = 15.0f; // intervalle de temps entre chaque spawn de véhicule
    private float timer;  // compteur de temps pour le prochain spawn
    private bool canSpawn = true; // indicateur pour savoir si un véhicule peut  apparaitre

    private void Update()
    {
        // Boucle de spawn
        if (canSpawn)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnVehicule();
                timer = 0f;
                canSpawn = false;
            }
        }
    }

    // Méthode pour spawner un véhicule à la position et la rotation spécifiées
    private void SpawnVehicule()
    {
        Vector3 spawnPosition = new Vector3(-338.4f, 253.97f, 2447.9f);  // définir la position du véhicule
        Quaternion spawnRotation = Quaternion.Euler(0f, 28.011f, 0f); // définir la rotation du véhicule
        GameObject spawnedVehicle = Instantiate(vehiculePrefab, spawnPosition, spawnRotation); // instancier le préfab du véhicule à la position et la rotation spécifiées
        VehicleBusAI vehicleMovement = spawnedVehicle.GetComponent<VehicleBusAI>(); // récupérer le composant du véhicule
        if (vehicleMovement != null)
        {
            // Ajouter un listener à l'événement OnVehicleDestroyed pour activer le prochain spawn lorsque le véhicule est détruit
            vehicleMovement.OnVehicleDestroyed.AddListener(HandleVehicleDestroyed);
        }
    }

    // Méthode pour activer le prochain spawn de véhicule lorsque l'événement OnVehicleDestroyed est déclenché
    private void HandleVehicleDestroyed()
    {
        canSpawn = true;
    }
}

