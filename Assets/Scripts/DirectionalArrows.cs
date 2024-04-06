using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalArrows : MonoBehaviour
{
    // référence au Transform de la ligne d'arrivée
    [SerializeField]
    private Transform target;

    private void Update()
    {
        // fait pivoter la flèche directionnelle pour qu'elle regarde toujours vers la ligne d'arrivée
        transform.LookAt(target);
    }
}
