using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VehicleBusAI : MonoBehaviour
{
    public float speed = 15.0f; // vitesse de déplacement du véhicule
    public UnityEvent OnVehicleDestroyed;  // événement Unity déclenché lorsque le véhicule est détruit

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime); // déplacer le véhicule vers l'avant en fonction de la vitesse et du temps
    }

    // Méthode appelée lorsque le véhicule entre en collision avec un autre objet
    private void OnCollisionEnter(Collision collision)
    {
        // vérifier si le véhicule a percuté un mur ou le joueur
        if (collision.gameObject.name == "Player" || collision.gameObject.name == "Border (East)" || collision.gameObject.name == "Border (West)"
            || collision.gameObject.name == "Border (South)" || collision.gameObject.name == "Border (North)")
        {
            Destroy(this.gameObject); // détruire le véhicule
            OnVehicleDestroyed?.Invoke();  // déclencher l'événement OnVehicleDestroyed
        }
    }
}

