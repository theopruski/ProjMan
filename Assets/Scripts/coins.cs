using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using System;

public class coins : MonoBehaviour
{
    public float rotationSpeed = 50f;
    
    //public GameObject coinPrefab; // Le préfab de la pièce à faire spawner
    public int CoinNumber = 10; // Le nombre de pièces à faire spawner
    public Vector3 spawnArea = new Vector3(10, 0, 10); // La zone dans laquelle les pièces seront spawnées
    
    // Start is called before the first frame update
    void Start()
    {
        
        // Faire spawner des pièces aléatoirement dans la zone spécifiée
        for (int i = 0; i < CoinNumber; i++)
        {
            Vector3 spawnPosition = new Vector3(
                UnityEngine.Random.Range(-spawnArea.x, spawnArea.x),
                0,
                UnityEngine.Random.Range(-spawnArea.z, spawnArea.z)
            );
            //Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        }
 
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
