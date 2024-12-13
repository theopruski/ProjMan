using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField] private Transform player; // Le joueur
    [SerializeField] private GameObject[] deliveryZonesPrefabs; // Les préfabriqués des zones de livraison
    [SerializeField] private float deliveryRadius = 5f; // Rayon d'atteinte du point de livraison
    [SerializeField] private float distanceFromGround = 2f; // Hauteur au-dessus du sol pour le point de livraison

    // Liste des points où les livraisons peuvent apparaître
    [SerializeField] private Transform[] deliveryLocations; // Les routes
    // Les murs
    [SerializeField] private GameObject borderNorth;
    [SerializeField] private GameObject borderSouth;
    [SerializeField] private GameObject borderEast;
    [SerializeField] private GameObject borderWest;

    [SerializeField] private DirectionalArrows directionalArrow; // Référence au script DirectionalArrows

    private GameObject currentDeliveryZone; // Point de livraison actuel

    private float minX, maxX, minZ, maxZ; // Limites de la zone valide

    void Start()
    {
        // Calcul les limites valides à partir des "borders"
        CalculateBorders();
        // Générer un point de livraison dès le début
        SpawnNewDeliveryPoint();
    }

    void Update()
    {
        // Vérifier si le joueur a atteint le point de livraison actuel
        if (currentDeliveryZone != null && Vector3.Distance(player.position, currentDeliveryZone.transform.position) < deliveryRadius)
        {
            // Le joueur a atteint le point de livraison, donc on en place un nouveau
            Destroy(currentDeliveryZone); // Détruire le point actuel
            // Récupérer le script PlayerVehicle et réinitialiser le timer
            PlayerVehicle playerVehicle = player.GetComponent<PlayerVehicle>();
            if (playerVehicle != null)
            {
                playerVehicle.timer = 45.0f; // remettre le timer à 45 secondes
            }

            SpawnNewDeliveryPoint(); // Créer un nouveau point de livraison
        }
    }

    private void CalculateBorders()
    {
        // Calculer les limites X et Z en fonction des "Borders"
        minX = borderWest.GetComponent<Renderer>().bounds.min.x;
        maxX = borderEast.GetComponent<Renderer>().bounds.max.x;
        minZ = borderSouth.GetComponent<Renderer>().bounds.min.z;
        maxZ = borderNorth.GetComponent<Renderer>().bounds.max.z;
    }

    // Fonction pour générer un nouveau point de livraison dans les limites définies et endroit spécifique où le point de livraison doit apparaître
    private void SpawnNewDeliveryPoint()
    {
        Transform chosenLocation = null;
        Vector3 spawnPosition = Vector3.zero;
        bool validPosition = false;

        while (!validPosition)
        {
            // Choisir un endroit spécifique où le point de livraison doit apparaître
            int randomIndex = Random.Range(0, deliveryLocations.Length);
            chosenLocation = deliveryLocations[randomIndex];
            spawnPosition = chosenLocation.position;
            // Calculer la position Y en fonction du terrain
            if (Physics.Raycast(chosenLocation.position + Vector3.up * 500f, Vector3.down, out RaycastHit hitInfo, 1000f))
            {
                // Positionner au-dessus du sol
                spawnPosition.y = hitInfo.point.y + distanceFromGround;
            }
            else
            {
                Debug.LogWarning($"Impossible de trouver le sol sous {chosenLocation.name}, position par défaut utilisée.");
            }

            // Vérifier si le point est à l'intérieur des limites des "borders"
            if (IsInsideBorders(spawnPosition))
            {
                validPosition = true;
            }
        }

        // Préfab de zone de livraison aléatoire
        int randomZoneIndex = Random.Range(0, deliveryZonesPrefabs.Length);
        GameObject deliveryZonePrefab = deliveryZonesPrefabs[randomZoneIndex];
        // Crée le nouveau point de livraison à la position choisie
        currentDeliveryZone = Instantiate(deliveryZonePrefab, spawnPosition, Quaternion.identity);
        // Met à jour la flèche directionnelle pour qu'elle pointe vers le nouveau point de livraison
        directionalArrow.SetTarget(currentDeliveryZone.transform);
    }

    // Vérifie si une position est à l'intérieur des "borders"
    private bool IsInsideBorders(Vector3 position)
    {
        return position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ;
    }
}
