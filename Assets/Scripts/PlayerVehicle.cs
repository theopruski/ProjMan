using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerVehicle : MonoBehaviour
{

    private float speed = 10.0f;
    private float turnSpeed = 25.0f;
    private float horizontalInput;
    private float forwardInput;
    private int coinCount;
    public TextMeshProUGUI counterText;

    // Start is called before the first frame update
    void Start()
    {
        GameObject counterObject = GameObject.Find("Counter");
        coinCount = 0;
        SetCountText();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        // Move the vehicle forward
        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("coins"))
        {
            // Si la voiture rentre en collision avec l'objet, l'objet est désactiver.
            other.gameObject.SetActive(false);
            coinCount = coinCount + 1;
            SetCountText();
        }
    }

    void SetCountText()
    {
        // Mettre à jour le texte du compteur avec le nouveau nombre de pièces collectées
        counterText.text = "Count: " + coinCount.ToString();
    }
}
