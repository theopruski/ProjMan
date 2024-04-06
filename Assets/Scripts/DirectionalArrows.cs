using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalArrows : MonoBehaviour
{
    // r�f�rence au Transform de la ligne d'arriv�e
    [SerializeField]
    private Transform target;

    private void Update()
    {
        // fait pivoter la fl�che directionnelle pour qu'elle regarde toujours vers la ligne d'arriv�e
        transform.LookAt(target);
    }
}
