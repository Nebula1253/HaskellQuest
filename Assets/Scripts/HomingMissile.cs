using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public Transform target;
    public float speed = 2.5f;
    public float rotateSpeed = 10f; // rotation speed in radians/second
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDir = target.position - transform.position;

        float singleStep = rotateSpeed * Time.deltaTime;

        Vector3 newDirection = Vector3.RotateTowards(transform.up, targetDir, singleStep, 0.0f);

        transform.rotation = Quaternion.LookRotation(transform.forward, newDirection);
        transform.position += transform.up * Time.deltaTime * speed;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            Debug.Log("Player hit!");
            Destroy(gameObject);
        }
    }
}
