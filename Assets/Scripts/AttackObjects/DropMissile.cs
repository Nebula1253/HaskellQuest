using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMissile : MonoBehaviour
{
    private float speed;
    private bool doesDamage;

    public void setSpeed(float speed) {
        this.speed = speed;
    }

    public void setDoesDamage(bool doesDamage) {
        this.doesDamage = doesDamage;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= transform.up * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Player hit!");
            if (doesDamage) {
                other.gameObject.GetComponent<PlayerHeart>().TakeDamage(5);
            }
        }
        else if (other.gameObject.CompareTag("Projectile")) {
            Destroy(other.gameObject);
        }
        Destroy(gameObject);
    }

    void OnBecameInvisible() {
        Destroy(gameObject);
    }
}
