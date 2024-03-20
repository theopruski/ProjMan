using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public float distance = 10f;
    public float height = 5f;
    public float rotationSmoothTime = 0.12f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPosition = player.transform.position - player.transform.forward * distance + Vector3.up * height;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * rotationSmoothTime);
        transform.LookAt(player.transform);
    }
}
