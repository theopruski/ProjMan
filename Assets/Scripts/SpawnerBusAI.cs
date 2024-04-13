using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBusAI : MonoBehaviour
{
    public GameObject vehiculePrefab; // r�f�rence au pr�fab du v�hicule
    public float spawnInterval = 15.0f; // intervalle de temps entre chaque spawn de v�hicule
    private float timer;  // compteur de temps pour le prochain spawn
    private bool canSpawn = true; // indicateur pour savoir si un v�hicule peut  apparaitre

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

    // M�thode pour spawner un v�hicule � la position et la rotation sp�cifi�es
    private void SpawnVehicule()
    {
        Vector3 spawnPosition = new Vector3(-338.4f, 253.97f, 2447.9f);  // d�finir la position du v�hicule
        Quaternion spawnRotation = Quaternion.Euler(0f, 28.011f, 0f); // d�finir la rotation du v�hicule
        GameObject spawnedVehicle = Instantiate(vehiculePrefab, spawnPosition, spawnRotation); // instancier le pr�fab du v�hicule � la position et la rotation sp�cifi�es
        VehicleBusAI vehicleMovement = spawnedVehicle.GetComponent<VehicleBusAI>(); // r�cup�rer le composant du v�hicule
        if (vehicleMovement != null)
        {
            // Ajouter un listener � l'�v�nement OnVehicleDestroyed pour activer le prochain spawn lorsque le v�hicule est d�truit
            vehicleMovement.OnVehicleDestroyed.AddListener(HandleVehicleDestroyed);
        }
    }

    // M�thode pour activer le prochain spawn de v�hicule lorsque l'�v�nement OnVehicleDestroyed est d�clench�
    private void HandleVehicleDestroyed()
    {
        canSpawn = true;
    }
}

