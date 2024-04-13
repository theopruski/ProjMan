using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VehicleBusAI : MonoBehaviour
{
    public float speed = 15.0f; // vitesse de d�placement du v�hicule
    public UnityEvent OnVehicleDestroyed;  // �v�nement Unity d�clench� lorsque le v�hicule est d�truit

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime); // d�placer le v�hicule vers l'avant en fonction de la vitesse et du temps
    }

    // M�thode appel�e lorsque le v�hicule entre en collision avec un autre objet
    private void OnCollisionEnter(Collision collision)
    {
        // v�rifier si le v�hicule a percut� un mur ou le joueur
        if (collision.gameObject.name == "Player" || collision.gameObject.name == "Border (East)" || collision.gameObject.name == "Border (West)"
            || collision.gameObject.name == "Border (South)" || collision.gameObject.name == "Border (North)")
        {
            Destroy(this.gameObject); // d�truire le v�hicule
            OnVehicleDestroyed?.Invoke();  // d�clencher l'�v�nement OnVehicleDestroyed
        }
    }
}

