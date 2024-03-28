using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using System;

public class coins : MonoBehaviour
{
    public float rotationSpeed = 50f;
    
    //public GameObject coinPrefab; // Le pr�fab de la pi�ce � faire spawner
    public int CoinNumber = 10; // Le nombre de pi�ces � faire spawner
    public Vector3 spawnArea = new Vector3(10, 0, 10); // La zone dans laquelle les pi�ces seront spawn�es
    
    // Start is called before the first frame update
    void Start()
    {
        
        // Faire spawner des pi�ces al�atoirement dans la zone sp�cifi�e
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
