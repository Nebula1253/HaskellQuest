using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float rotateSpeed; // rotation speed in radians/second
    private float lockDelay;

    private bool missileLocked = false;
    private AudioSource source;

    public void SetTarget(Transform target) {
        this.target = target;
    }

    public void SetSpeed(float speed) {
        this.speed = speed;
    }

    public void SetRotateSpeed(float rotateSpeed) {
        this.rotateSpeed = rotateSpeed;
    }

    public void SetLockDelay(float lockDelay) {
        this.lockDelay = lockDelay;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(missileLockDelay());
        source = GetComponent<AudioSource>();
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!missileLocked) {
            Vector3 targetDir = target.position - transform.position;

            float singleStep = rotateSpeed * Time.deltaTime;

            Vector3 newDirection = Vector3.RotateTowards(transform.up, targetDir, singleStep, 0.0f);

            transform.rotation = Quaternion.LookRotation(transform.forward, newDirection);
        }
        transform.position += transform.up * Time.deltaTime * speed;
    }

    IEnumerator missileLockDelay() {
        yield return new WaitForSecondsRealtime(lockDelay);
        missileLocked = true;
    }

    public void resetTarget(Transform newTarget) {
        target = newTarget;
        missileLocked = false;
        StartCoroutine(missileLockDelay());
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Player hit!");
            other.gameObject.GetComponent<PlayerAvatar>().TakeDamage(5);
        }
        Destroy(gameObject);
    }

    void OnBecameInvisible() {
        Destroy(gameObject);
    }
}
