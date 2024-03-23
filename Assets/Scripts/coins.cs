using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class coins : MonoBehaviour
{
    public float rotationSpeed = 50f;

    private int coinCount = 0;

    private static TMPro.TextMeshProUGUI counterText;

    // Start is called before the first frame update
    void Start()
    {
        GameObject counterObject = GameObject.Find("Counter");
        if (counterObject != null)
        {
            // R�cup�rer le composant TextMeshProUGUI
            counterText = counterObject.GetComponent<TMPro.TextMeshProUGUI>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            coinCount++;

            // Mettre � jour le texte du compteur avec le nouveau nombre de pi�ces collect�es
            counterText.text = "Counter: " + coinCount.ToString();

            // Si l'objet entre en collision avec une voiture, le faire dispara�tre
            Destroy(gameObject);
        }
    }
}
