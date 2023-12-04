using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : EnemyController
{
    // Start is called before the first frame update
    public GameObject missile;
    public int nrMissiles;
    public float missileCircleRadius, missileSpawnDelay, missileSpeed, missileRotateSpeed, missileLockDelay;
    public Vector3 firstMissilePos;

    private float missileDegreeIncrement;
    private Transform target;

    void Start()
    {
        missileDegreeIncrement = 360f / nrMissiles;
    }

    public override void Trigger(bool result) {
        if (result) {
            target = GameObject.FindGameObjectWithTag("Enemy").transform;
        } else {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        StartCoroutine(fireMissiles());
    }

    private void InstantiateMissile(Vector3 position, Transform target, float speed, float rotateSpeed, float lockDelay) {
        GameObject newMissile = Instantiate(missile, position, Quaternion.identity);
        newMissile.GetComponent<HomingMissile>().SetTarget(target);
        newMissile.GetComponent<HomingMissile>().SetSpeed(speed);
        newMissile.GetComponent<HomingMissile>().SetRotateSpeed(rotateSpeed);
        newMissile.GetComponent<HomingMissile>().SetLockDelay(lockDelay);
    }
    IEnumerator fireMissiles() {
        Vector3 currMissilePos;
        for (int i=0; i < nrMissiles; i++) {
            // find current missile position using missileDegreeIncrement, missileCircleRadius and firstMissilePos
            currMissilePos = new Vector3(firstMissilePos.x + missileCircleRadius * Mathf.Cos(missileDegreeIncrement * i * Mathf.Deg2Rad), 
                                        firstMissilePos.y + missileCircleRadius * Mathf.Sin(missileDegreeIncrement * i * Mathf.Deg2Rad), 
                                        firstMissilePos.z);

            InstantiateMissile(currMissilePos, target, missileSpeed, missileRotateSpeed, missileLockDelay);
            yield return new WaitForSeconds(missileSpawnDelay);
        }
    }
}
