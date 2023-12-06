using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : EnemyController
{
    public GameObject missile;
    public int nrMissiles;
    public float missileSpeed, missileRotateSpeed, missileLockDelay;
    public float missileAngleRange;
    private Transform target, playerTransform, enemyTransform;
    private GameObject gameField;
    
    // Start is called before the first frame update
    void Start()
    {
        gameField = GameObject.FindGameObjectWithTag("GameField");
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        enemyTransform = GameObject.FindGameObjectWithTag("Enemy").transform;
    }

    public override void Trigger(bool result) {
        // if (result) {
        //     target = transform;
        //     StartCoroutine(fireMissiles(true));
        // }
        // else {
        //     target = GameObject.FindGameObjectWithTag("Player").transform;
        //     StartCoroutine(fireMissiles());
        // }
        StartCoroutine(fireMissiles(result));
    }

    private GameObject InstantiateMissile(Vector3 position, Transform target) {
        var missileAngle = 180 + Random.Range(-missileAngleRange, missileAngleRange);
        Debug.Log(missileAngle);
        var missileRotation = Quaternion.AngleAxis(missileAngle, Vector3.forward.normalized);
        Vector3 missileStartOffset = missileRotation * new Vector3(0, 1.5f, 0);

        GameObject newMissile = Instantiate(missile, position + missileStartOffset, missileRotation, gameField.transform);
        newMissile.GetComponent<HomingMissile>().SetTarget(target);
        newMissile.GetComponent<HomingMissile>().SetSpeed(missileSpeed);
        newMissile.GetComponent<HomingMissile>().SetRotateSpeed(missileRotateSpeed);
        newMissile.GetComponent<HomingMissile>().SetLockDelay(missileLockDelay);

        return newMissile;
    }

    IEnumerator fireMissiles(bool redirect = false) {
        yield return new WaitForSeconds(1f);
        for (int i=0; i < nrMissiles; i++) {
            target = playerTransform;
            var missile = InstantiateMissile(transform.position, target);
            yield return new WaitForSeconds(0.5f);
            if (redirect) {
                target = enemyTransform;
                missile.GetComponent<HomingMissile>().SetTarget(target);
            }
        }
    }

    // IEnumerator redirectMissiles() {
    //     yield return new WaitForSeconds(1f);
    //     var missiles = GameObject.FindGameObjectsWithTag("Missile");
    //     foreach (var missile in missiles) {
    //         missile.GetComponent<HomingMissile>().SetTarget(target);
    //     }
    // }
}
