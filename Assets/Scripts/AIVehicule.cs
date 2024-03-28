using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVehicule : MonoBehaviour
{
    public float speed = 10.0f; // Vitesse de déplacement du véhicule

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        // Déplacez le véhicule vers l'avant à chaque image en utilisant sa vitesse actuelle
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Vérifier si le véhicule a percuté un mur
        if (collision.gameObject.name == "Player" || collision.gameObject.name == "Border (East)" || collision.gameObject.name == "Border (West)" || collision.gameObject.name == "Border (South)" || collision.gameObject.name == "Border (North)")
        {
            // Détruire le véhicule
            Destroy(this.gameObject);
        }
    }
}
