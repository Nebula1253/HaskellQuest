using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private Transform target;
    private Vector3 targetPos;
    private float speed;
    private float rotateSpeed; // rotation speed in radians/second
    private float lockDelay;

    private bool missileLocked = false;
    private AudioSource source;
    private bool doesDamage = true;
    public bool destroyedByMines;

    public void SetTargetTransform(Transform target) {
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

    public void SetTargetPosition(Vector3 targetPos) {
        this.targetPos = targetPos;
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
            Vector3 targetDir;
            if (target != null) {
                targetDir = target.position - transform.position;
            }
            else {
                targetDir = targetPos - transform.position;
            }

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
            if (doesDamage) {
                other.gameObject.GetComponent<PlayerAvatar>().TakeDamage(5);
            }
        }
        if (NetworkHelper.Instance.IsPlayerOne) {
            if (other.gameObject.CompareTag("Landmine")) {
                if (destroyedByMines) {
                    Destroy(gameObject);
                }
            }
            else {
                Destroy(gameObject);
            }
        }
    }

    void OnBecameInvisible() {
        if (NetworkHelper.Instance.IsPlayerOne) {
            Destroy(gameObject);
        }
    }

    public void Deactivate() {
        doesDamage = false;
    }
}
