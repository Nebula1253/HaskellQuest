using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalLaser : MonoBehaviour
{
    // Start is called before the first frame update
    private float speed;
    private bool fractalSplit;
    private AudioSource audioSource;
    void Start()
    {
        var player = GameObject.Find("Player");
        var playerPos = player.transform.position;
        var direction = (playerPos - transform.position).normalized;

        transform.Rotate(Quaternion.LookRotation(transform.forward, direction).eulerAngles);
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void SetVariables(float speed, bool fractalSplit) {
        this.speed = speed;
        this.fractalSplit = fractalSplit;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            var player = other.gameObject.GetComponent<PlayerAvatar>();
            if (fractalSplit) {
                player.TakeDamage(1);
            }
            else {
                player.TakeDamage(10);
            }
        }
        Destroy(gameObject);
    }

    void OnBecameInvisible() {
        Destroy(gameObject);
    }
}
