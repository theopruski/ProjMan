using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField] private Transform player; // Le joueur
    [SerializeField] private GameObject[] deliveryZonesPrefabs; // Les pr�fabriqu�s des zones de livraison
    [SerializeField] private float deliveryRadius = 5f; // Rayon d'atteinte du point de livraison
    [SerializeField] private float distanceFromGround = 2f; // Hauteur au-dessus du sol pour le point de livraison

    // Liste des points o� les livraisons peuvent appara�tre
    [SerializeField] private Transform[] deliveryLocations; // Les routes
    // Les murs
    [SerializeField] private GameObject borderNorth;
    [SerializeField] private GameObject borderSouth;
    [SerializeField] private GameObject borderEast;
    [SerializeField] private GameObject borderWest;

    [SerializeField] private DirectionalArrows directionalArrow; // R�f�rence au script DirectionalArrows

    private GameObject currentDeliveryZone; // Point de livraison actuel

    private float minX, maxX, minZ, maxZ; // Limites de la zone valide

    private const int baseSalary = 200; // Salaire de base

    void Start()
    {
        // Calcul les limites valides � partir des "borders"
        CalculateBorders();
        // G�n�rer un point de livraison d�s le d�but
        SpawnNewDeliveryPoint();
    }

    void Update()
    {
        // V�rifier si le joueur a atteint le point de livraison actuel
        if (currentDeliveryZone != null && Vector3.Distance(player.position, currentDeliveryZone.transform.position) < deliveryRadius)
        {
            // Le joueur a atteint le point de livraison, donc on en place un nouveau
            Destroy(currentDeliveryZone); // D�truire le point actuel
            // R�cup�rer le script PlayerVehicle et r�initialiser le timer
            PlayerVehicle playerVehicle = player.GetComponent<PlayerVehicle>();
            if (playerVehicle != null)
            {
                float remainingTime = playerVehicle.timer;
                int salary = CalculateSalary(remainingTime);
                playerVehicle.salary += salary;
                playerVehicle.SetSalaryText();
                playerVehicle.timer = 45.0f; // remettre le timer � 45 secondes
                playerVehicle.lateCounted = false; // R�initialise l'indicateur de retard compt�
            }

            SpawnNewDeliveryPoint(); // Cr�er un nouveau point de livraison
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

    // Fonction pour g�n�rer un nouveau point de livraison dans les limites d�finies et endroit sp�cifique o� le point de livraison doit appara�tre
    private void SpawnNewDeliveryPoint()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool validPosition = false;

        while (!validPosition)
        {
            // G�n�rer une position al�atoire � l'int�rieur des limites de bordure
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            spawnPosition = new Vector3(randomX, 0, randomZ);

            // Calculer la position Y en fonction du terrain
            if (Physics.Raycast(spawnPosition + Vector3.up * 500f, Vector3.down, out RaycastHit hitInfo, 1000f))
            {
                // V�rifier si le point est sur une route
                if (hitInfo.collider.CompareTag("Road"))
                {
                    // Positionner au-dessus du sol
                    spawnPosition.y = hitInfo.point.y + distanceFromGround;
                    validPosition = true;
                }
            }
        }

        // Pr�fab de zone de livraison al�atoire
        int randomZoneIndex = Random.Range(0, deliveryZonesPrefabs.Length);
        GameObject deliveryZonePrefab = deliveryZonesPrefabs[randomZoneIndex];
        // Cr�e le nouveau point de livraison � la position choisie
        currentDeliveryZone = Instantiate(deliveryZonePrefab, spawnPosition, Quaternion.identity);
        // Met � jour la fl�che directionnelle pour qu'elle pointe vers le nouveau point de livraison
        directionalArrow.SetTarget(currentDeliveryZone.transform);
    }

    // V�rifie si une position est � l'int�rieur des "borders"
    private bool IsInsideBorders(Vector3 position)
    {
        return position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ;
    }

    // Calcule le salaire en fonction du temps restant
    private int CalculateSalary(float remainingTime)
    {
        if (remainingTime >= 10 && remainingTime <= 45)
        {
            return 250;
        }
        else if (remainingTime >= -10 && remainingTime < 10)
        {
            return 200;
        }
        else
        {
            return 150;
        }
    }
}
