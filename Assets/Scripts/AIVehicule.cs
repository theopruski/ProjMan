using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVehicule : MonoBehaviour
{
    public float speed = 10.0f; // Vitesse de d�placement du v�hicule

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        // D�placez le v�hicule vers l'avant � chaque image en utilisant sa vitesse actuelle
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // V�rifier si le v�hicule a percut� un mur
        if (collision.gameObject.name == "Player" || collision.gameObject.name == "Border (East)" || collision.gameObject.name == "Border (West)" || collision.gameObject.name == "Border (South)" || collision.gameObject.name == "Border (North)")
        {
            // D�truire le v�hicule
            Destroy(this.gameObject);
        }
    }
}
