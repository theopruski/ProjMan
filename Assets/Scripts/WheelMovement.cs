using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMovement : MonoBehaviour
{
    public GameObject player;
    public float turnSpeed = 25.0f;
    private float horizontalInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        Quaternion turnRotation = Quaternion.Euler(0f, horizontalInput * turnSpeed * Time.deltaTime, 0f);
        transform.localRotation *= turnRotation;
    }
}
