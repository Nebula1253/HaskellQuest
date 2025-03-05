using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalLaser : MonoBehaviour
{
    // Start is called before the first frame update
    private float speed;
    private int damage;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void SetVariables(float speed, int damage) {
        this.speed = speed;
        this.damage = damage;
    }

    public void SetWhichPlayer(int whichPlayer = 0) {
        var player = GameObject.FindGameObjectsWithTag("Player")[whichPlayer];
        var playerPos = player.transform.position;
        var direction = (playerPos - transform.position).normalized;

        transform.Rotate(Quaternion.LookRotation(transform.forward, direction).eulerAngles);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player") && NetworkHelper.Instance.IsPlayerOne) {
            var player = other.gameObject.GetComponent<PlayerAvatar>();
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
        
    }

    void OnBecameInvisible() {
        if (NetworkHelper.Instance.IsPlayerOne) {
            Destroy(gameObject);
        }
    }
}
