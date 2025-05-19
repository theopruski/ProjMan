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

    [SerializeField] private TMPro.TextMeshProUGUI bossText; // Texte du patron
    [SerializeField] private float[] warningThresholds; // Les temps de retard
    [SerializeField] private string[] warningMessages;  // message apres avoir délivrer.

    [SerializeField] private TMPro.TextMeshProUGUI clientText; // Texte du client
    [SerializeField] private string[] clientMessages; // Les messages du client, de positif à négatif
    [SerializeField] private float[] clientMessageThresholds; // Les seuils de temps restants (ex : [35, 20, 10])

    private bool[] warningsTriggered; // Pour ne pas répéter les messages. voir si on garde

    private GameObject currentDeliveryZone; // Point de livraison actuel

    private float minX, maxX, minZ, maxZ; // Limites de la zone valide

    private const int baseSalary = 200; // Salaire de base

    void Start()
    {
        // Calcul les limites valides à partir des "borders"
        CalculateBorders();
        // Générer un point de livraison dès le début
        SpawnNewDeliveryPoint();
        // initalise le message du patron
        warningsTriggered = new bool[warningMessages.Length];
    }

    void Update()
    {
        // Récupérer le script PlayerVehicle
        PlayerVehicle playerVehicle = player.GetComponent<PlayerVehicle>();


        // Vérifier si le joueur a atteint le point de livraison actuel
        if (currentDeliveryZone != null && Vector3.Distance(player.position, currentDeliveryZone.transform.position) < deliveryRadius)
        {
            // Le joueur a atteint le point de livraison, donc on en place un nouveau
            Destroy(currentDeliveryZone); // Détruire le point actuel
            // réinitialiser le timer
            if (playerVehicle != null)
            {
                float remainingTime = playerVehicle.timer;
                int salary = CalculateSalary(remainingTime);
                playerVehicle.salary += salary;
                playerVehicle.SetSalaryText();
                DisplayClientReaction(remainingTime); // message client qui apparait
                playerVehicle.timer = 45.0f; // remettre le timer à 45 secondes
                playerVehicle.lateCounted = false; // Réinitialise l'indicateur de retard compté
                bossText.text = ""; // Nettoyer le texte
                warningsTriggered = new bool[warningMessages.Length]; // Réinitialiser les avertissements
            }

            SpawnNewDeliveryPoint(); // Créer un nouveau point de livraison
        }

        // Affichage des messages menaçants si le joueur est en retard
        if (playerVehicle != null)
        {
            float elapsedTime = 45f - playerVehicle.timer; // Combien de temps s'est écoulé
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

        // message deviens plus négative
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

    // Fonction pour générer un nouveau point de livraison dans les limites définies et endroit spécifique où le point de livraison doit apparaître
    private void SpawnNewDeliveryPoint()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool validPosition = false;

        while (!validPosition)
        {
            // Générer une position aléatoire à l'intérieur des limites de bordure
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            spawnPosition = new Vector3(randomX, 0, randomZ);

            // Calculer la position Y en fonction du terrain
            if (Physics.Raycast(spawnPosition + Vector3.up * 500f, Vector3.down, out RaycastHit hitInfo, 1000f))
            {
                // Vérifier si le point est sur une route
                if (hitInfo.collider.CompareTag("Road"))
                {
                    // Positionner au-dessus du sol
                    spawnPosition.y = hitInfo.point.y + distanceFromGround;
                    validPosition = true;
                }
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
