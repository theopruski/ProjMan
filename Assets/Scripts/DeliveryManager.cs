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

    [SerializeField] private TMPro.TextMeshProUGUI bossText; // Texte du patron
    [SerializeField] private float[] warningThresholds; // Les temps de retard
    [SerializeField] private string[] warningMessages;  // message apres avoir d�livrer.

    [SerializeField] private TMPro.TextMeshProUGUI clientText; // Texte du client
    [SerializeField] private string[] clientMessages; // Les messages du client, de positif � n�gatif
    [SerializeField] private float[] clientMessageThresholds; // Les seuils de temps restants (ex : [35, 20, 10])

    private bool[] warningsTriggered; // Pour ne pas r�p�ter les messages. voir si on garde

    private GameObject currentDeliveryZone; // Point de livraison actuel

    private float minX, maxX, minZ, maxZ; // Limites de la zone valide

    private const int baseSalary = 200; // Salaire de base

    void Start()
    {
        // Calcul les limites valides � partir des "borders"
        CalculateBorders();
        // G�n�rer un point de livraison d�s le d�but
        SpawnNewDeliveryPoint();
        // initalise le message du patron
        warningsTriggered = new bool[warningMessages.Length];
    }

    void Update()
    {
        // R�cup�rer le script PlayerVehicle
        PlayerVehicle playerVehicle = player.GetComponent<PlayerVehicle>();


        // V�rifier si le joueur a atteint le point de livraison actuel
        if (currentDeliveryZone != null && Vector3.Distance(player.position, currentDeliveryZone.transform.position) < deliveryRadius)
        {
            // Le joueur a atteint le point de livraison, donc on en place un nouveau
            Destroy(currentDeliveryZone); // D�truire le point actuel
            // r�initialiser le timer
            if (playerVehicle != null)
            {
                float remainingTime = playerVehicle.timer;
                int salary = CalculateSalary(remainingTime);
                playerVehicle.salary += salary;
                playerVehicle.SetSalaryText();
                DisplayClientReaction(remainingTime); // message client qui apparait
                playerVehicle.timer = 45.0f; // remettre le timer � 45 secondes
                playerVehicle.lateCounted = false; // R�initialise l'indicateur de retard compt�
                bossText.text = ""; // Nettoyer le texte
                warningsTriggered = new bool[warningMessages.Length]; // R�initialiser les avertissements
            }

            SpawnNewDeliveryPoint(); // Cr�er un nouveau point de livraison
        }

        // Affichage des messages mena�ants si le joueur est en retard
        if (playerVehicle != null)
        {
            float elapsedTime = 45f - playerVehicle.timer; // Combien de temps s'est �coul�
            for (int i = 0; i < warningThresholds.Length; i++)
            {
                if (elapsedTime > warningThresholds[i] && !warningsTriggered[i])
                {
                    bossText.text = warningMessages[i];
                    warningsTriggered[i] = true;
                }
            }
        }
    }

    private void DisplayClientReaction(float remainingTime)
    {
        // Choisir le bon message en fonction du temps restant
        for (int i = 0; i < clientMessageThresholds.Length; i++)
        {
            if (remainingTime >= clientMessageThresholds[i])
            {
                clientText.text = clientMessages[i];
                return;
            }
        }

        // message deviens plus n�gative
        clientText.text = clientMessages[clientMessages.Length - 1];

        //StartCoroutine(ClearClientMessageAfterSeconds(3f));
    }

    // efface le message du client quelque seconde plus tard
    private IEnumerator ClearClientMessageAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        clientText.text = "";
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
