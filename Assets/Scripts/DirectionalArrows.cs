using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalArrows : MonoBehaviour
{
    // Référence au Transform de la zone de livraison
    private Transform target; // Le point de livraison actuel

    void Update()
    {
        // Faire en sorte que la flèche regarde toujours vers la cible (zone de livraison)
        if (target != null)
        {
            transform.LookAt(target);
        }
    }

    // Méthode pour mettre à jour la cible
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}