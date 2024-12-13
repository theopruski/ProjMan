using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class DirectionalArrows : MonoBehaviour
//{
//    // r�f�rence au Transform de la ligne d'arriv�e
//    [SerializeField]
//    private Transform target;

//    private void Update()
//    {
//        // fait pivoter la fl�che directionnelle pour qu'elle regarde toujours vers la ligne d'arriv�e
//        transform.LookAt(target);
//    }
//}

public class DirectionalArrows : MonoBehaviour
{
    // R�f�rence au Transform de la zone de livraison
    private Transform target; // Le point de livraison actuel

    void Update()
    {
        // Faire en sorte que la fl�che regarde toujours vers la cible (zone de livraison)
        if (target != null)
        {
            transform.LookAt(target);
        }
    }

    // M�thode pour mettre � jour la cible
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}