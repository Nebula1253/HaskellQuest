using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HomingFreezeMissileController : AttackController
{
    public GameObject homingMissile, freezeMissile;
    private GameObject battlefield;
    public int nrMissiles;

    public float homingSpeed, homingRotateSpeed, homingLockDelay;
    public float homingAngleRange;

    public float freezeSpeed, freezeRotateSpeed, freezeLockDelay;
    public float freezeAngleRange;

    private Transform enemyTransform;


    private bool retarget = false, disable = false;
    private List<Transform> playerTransforms = new List<Transform>();

    private bool missilesFired = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyTransform = gameObject.transform;
        battlefield = GameObject.FindGameObjectWithTag("GameField");
    }

    public override void Trigger(bool result)
    {
        SetRetargetDisable("Neither works");
        StartCoroutine(fireMissiles());
    }

    public override void Trigger(bool result, string additionalConditions)
    {
        SetRetargetDisable(additionalConditions);
        StartCoroutine(fireMissiles());
    }

    private void SetRetargetDisable(string additionalConditions) {
        switch (additionalConditions)
        {
            case "Neither works":
                retarget = false;
                disable = false;
                break;

            case "Only retarget works":
                retarget = true;
                disable = false;
                break;
            
            case "Only disable works":
                retarget = false;
                disable = true;
                break;

            case "Both work":
                retarget = true;
                disable = true;
                break;
        }
    }

    private GameObject InstantiateHomingMissile(Vector3 position, Transform target) {
        var homingAngle = 180 + Random.Range(-homingAngleRange, homingAngleRange);
        // Debug.Log(homingAngle);
        var homingRotation = Quaternion.AngleAxis(homingAngle, Vector3.forward.normalized);
        Vector3 missileStartOffset = homingRotation * new Vector3(0, 1.5f, 0);

        GameObject newMissile = Instantiate(homingMissile, position + missileStartOffset, homingRotation, battlefield.transform);

        newMissile.GetComponent<HomingMissile>().SetTargetTransform(target);
        newMissile.GetComponent<HomingMissile>().SetSpeed(homingSpeed);
        newMissile.GetComponent<HomingMissile>().SetRotateSpeed(homingRotateSpeed);
        newMissile.GetComponent<HomingMissile>().SetLockDelay(homingLockDelay);

        newMissile.GetComponent<NetworkObject>().Spawn();

        return newMissile;
    }

    private void InstantiateFreezeMissile(Vector3 position, Transform target) {
        var freezeAngle = 180 + Random.Range(-freezeAngleRange, freezeAngleRange);
        // Debug.Log(homingAngle);
        var freezeRotation = Quaternion.AngleAxis(freezeAngle, Vector3.forward.normalized);
        Vector3 missileStartOffset = freezeRotation * new Vector3(0, 1.5f, 0);

        GameObject newMissile = Instantiate(freezeMissile, position + missileStartOffset, freezeRotation, battlefield.transform);

        newMissile.GetComponent<HomingMissile>().SetTargetTransform(target);
        newMissile.GetComponent<HomingMissile>().SetSpeed(freezeSpeed);
        newMissile.GetComponent<HomingMissile>().SetRotateSpeed(freezeRotateSpeed);
        newMissile.GetComponent<HomingMissile>().SetLockDelay(freezeLockDelay);

        if (disable) {
            newMissile.GetComponent<FreezeMissile>().Deactivate();
            newMissile.GetComponent<HomingMissile>().Deactivate();
        }

        newMissile.GetComponent<NetworkObject>().Spawn();
    }

    IEnumerator fireMissiles() {
        yield return new WaitForSeconds(1f);

        if (playerTransforms.Count == 0) {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                playerTransforms.Add(player.transform);
            }
        }

        int whichMissileFreezes = Random.Range(1, nrMissiles / 2);

        int nrMissilesFiredP1 = 0;
        int nrMissilesFiredP2 = 0;
        int whichPlayer = 0;

        for (int i = 0; i < (nrMissiles * playerTransforms.Count); i++) {
            int nrToCompare;

            if (whichPlayer == 0) {
                nrMissilesFiredP1 += 1;
                nrToCompare = nrMissilesFiredP1;
                whichPlayer = 1;
            }
            else {
                nrMissilesFiredP2 += 1;
                nrToCompare = nrMissilesFiredP2;
                whichPlayer = 0;
            }

            if (nrToCompare == whichMissileFreezes) {
                // spawn freeze missile
                InstantiateFreezeMissile(transform.position, playerTransforms[whichPlayer]);
                yield return new WaitForSecondsRealtime(0.5f);
            }
            else {
                // spawn homing missile
                var missile = InstantiateHomingMissile(transform.position, playerTransforms[whichPlayer]);
                yield return new WaitForSecondsRealtime(0.5f);

                if (retarget) {
                    missile.GetComponent<HomingMissile>().SetTargetTransform(enemyTransform);
                }
            }
        }

        missilesFired = true;
    }

    public override bool AttackEnd()
    {
        if (missilesFired) {
            if (GameObject.FindGameObjectsWithTag("Projectile").Length == 0) {
                missilesFired = false;
            }
            return GameObject.FindGameObjectsWithTag("Projectile").Length == 0;
        }
        else {
            return false;
        }
    }
}
