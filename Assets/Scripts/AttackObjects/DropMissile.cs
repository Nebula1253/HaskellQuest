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
                other.gameObject.GetComponent<PlayerAvatar>().TakeDamage(5);
            }
            if (NetworkHelper.Instance.IsPlayerOne) {
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Projectile")) {
            if (NetworkHelper.Instance.IsPlayerOne) {
                if (other.gameObject.GetComponent<FreezeMissile>() == null) {
                    Destroy(other.gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }

    void OnBecameInvisible() {
        if (NetworkHelper.Instance.IsPlayerOne) Destroy(gameObject);
    }
}
