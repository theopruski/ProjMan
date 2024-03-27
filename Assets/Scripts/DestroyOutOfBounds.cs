using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    private float xMin = -1123f; // Limite inférieure sur l'axe X
    private float xMax = -126f; // Limite supérieure sur l'axe X
    private float yMin = 250f; // Limite inférieure sur l'axe Y
    private float yMax = 262f; // Limite supérieure sur l'axe Y
    private float zMin = 2401f; // Limite inférieure sur l'axe Z
    private float zMax = 3121f; // Limite supérieure sur l'axe Z

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Position initiale : " + transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Position actuelle : " + transform.position);

        if (transform.position.x < xMin || transform.position.x > xMax ||
            transform.position.y < yMin || transform.position.y > yMax ||
            transform.position.z < zMin || transform.position.z > zMax)
        {
            Debug.Log("Game Over!");
            Destroy(gameObject);
        }
    }
}
