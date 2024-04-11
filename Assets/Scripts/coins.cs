using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using System;

public class coins : MonoBehaviour
{
    
    public GameObject coinPrefab; // Le préfab de la pièce à faire spawner
    public int CoinNumber; // Le nombre de pièces à faire spawner
    public Vector3 spawnArea = new Vector3(1000, 0, 1000); // La zone dans laquelle les pièces seront spawnées
    public float collisionRadius = 100f; // rayon de collision
    public float distanceFromGround = 5f;


    // Start is called before the first frame update
    void Start()
    {
        
        // Faire spawner des pièces aléatoirement dans la zone spécifiée
        for (int i = 0; i < CoinNumber; i++)
        {
            Vector3 spawnPosition = Vector3.zero;
            bool validPosition = false;
            while (!validPosition) {
                spawnPosition = new Vector3(
                UnityEngine.Random.Range(-450, -850),
                0,
                UnityEngine.Random.Range(2550, 2950)
                );


                Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, collisionRadius);
                validPosition = true;

                foreach (Collider collider in hitColliders)
                {
                    if (collider.gameObject.name.StartsWith("building") && collider.gameObject.name.StartsWith("geometry"))
                    {
                        validPosition = false;
                        break;
                    }
                }

                RaycastHit hitInfo;
                if (Physics.Raycast(spawnPosition + Vector3.up * 1000f, Vector3.down, out hitInfo, 2000f))
                {
                    if (hitInfo.collider.gameObject.name.StartsWith("building"))
                    {
                        validPosition = false;
                    }
                }
            }

            RaycastHit hit; // Conservez cette variable pour l'utiliser dans cette portée
            if (Physics.Raycast(spawnPosition + Vector3.up * 1000f, Vector3.down, out hit, 2000f))
            {
                spawnPosition.y = hit.point.y + distanceFromGround;
            }

            Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        }
 
    }

    // Update is called once per frame
    void Update()
    {
    }
}
