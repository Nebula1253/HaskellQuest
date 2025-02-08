using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeMissile : MonoBehaviour
{
    // most of the intended behaviour is going to be provided by HomingMissile anyway
    // this is just here for the player movement freezing
    public float freezeDuration;
    private bool IsActive = true;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player") && IsActive) {
            other.gameObject.GetComponent<PlayerAvatar>().Freeze(freezeDuration);
        }
    }

    public void Deactivate() {
        IsActive = false;
    }
}
