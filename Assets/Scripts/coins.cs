using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using System;

public class coins : MonoBehaviour
{
    public float rotationSpeed = 50f;
    
    public GameObject coinPrefab; // Le pr�fab de la pi�ce � faire spawner
    public int CoinNumber; // Le nombre de pi�ces � faire spawner
    public Vector3 spawnArea = new Vector3(1000, 0, 1000); // La zone dans laquelle les pi�ces seront spawn�es
    public float collisionRadius = 100f; // rayon de collision


    // Start is called before the first frame update
    void Start()
    {
        
        // Faire spawner des pi�ces al�atoirement dans la zone sp�cifi�e
        for (int i = 0; i < CoinNumber; i++)
        {
            Vector3 spawnPosition = Vector3.zero;
            bool validPosition = false;
            while (!validPosition) {
                spawnPosition = new Vector3(
                UnityEngine.Random.Range(-450, -850),
                256,
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
            }
            Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        }
 
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
