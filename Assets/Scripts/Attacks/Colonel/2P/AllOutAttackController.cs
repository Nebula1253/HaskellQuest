using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AllOutAttackController : AttackController
{
    public GameObject homingMissile, freezeMissile, dropMissile, landmine;

    [Header("Homing Missile Params")]
    public float homingAngleRange;
    public float homingSpeed, homingRotateSpeed, homingLockDelay;
    
    [Header("Freeze Missile Params")]
    public float freezeAngleRange;
    public float freezeSpeed, freezeRotateSpeed, freezeLockDelay;

    [Header("Drop Missile Params")]
    public int nrMissiles;
    public float dropMissileSpeed, dropMissileBorderGap, dropMissileYOffset;

    [Header("Landmine Params")]
    public int nrLandmines;
    public float landmineTimeDelay, landmineMinGap, landmineMaxY;

    private bool homingRedirect = false, freezeRedirect = false;
    private GameObject battlefield;
    private float minX, maxX, minY;
    private List<Vector3> landminePositions = new List<Vector3>();
    private AudioSource audioSource;
    private bool fired = false;

    // Start is called before the first frame update
    void Start()
    {
        battlefield = GameObject.FindGameObjectWithTag("GameField");
        audioSource = GetComponent<AudioSource>();

        var boxSize = transform.parent.GetChild(0).GetComponent<Renderer>().bounds.size;
        var boxCentre = transform.parent.position;
        var boxSizeXOffset = boxSize.x / 2;

        minX = -boxSizeXOffset + boxCentre.x + dropMissileBorderGap;
        maxX = boxSizeXOffset + boxCentre.x - dropMissileBorderGap;
        minY = transform.position.y + dropMissileYOffset;
    }

    public override void Trigger(bool result)
    {
        SetBools("Neither work");
        StartCoroutine(Attack());
    }

    public override void Trigger(bool result, string additionalConditions)
    {
        SetBools(additionalConditions);
        StartCoroutine(Attack());
    }

    private void SetBools(string additionalConditions) {
        switch (additionalConditions) {
            case "Neither work":
                homingRedirect = false;
                freezeRedirect = false;
                break;

            case "Both work":
                homingRedirect = true;
                freezeRedirect = true;
                break;

            case "Only homing redirect works":
                homingRedirect = true;
                freezeRedirect = false;
                break;

            case "Only freeze redirect works":
                homingRedirect = false;
                freezeRedirect = true;
                break;
        }
    }

    private GameObject InstantiateHomingMissile(Vector3 position, Transform target) {
        var homingAngle = 180 + UnityEngine.Random.Range(-homingAngleRange, homingAngleRange);
        // Debug.Log(homingAngle);
        var homingRotation = Quaternion.AngleAxis(homingAngle, Vector3.forward.normalized);
        Vector3 missileStartOffset = homingRotation * new Vector3(0, 1.5f, 0);

        GameObject newMissile = Instantiate(homingMissile, position + missileStartOffset, homingRotation, battlefield.transform);

        newMissile.GetComponent<HomingMissile>().SetTargetTransform(target);
        newMissile.GetComponent<HomingMissile>().SetSpeed(homingSpeed);
        newMissile.GetComponent<HomingMissile>().SetRotateSpeed(homingRotateSpeed);
        newMissile.GetComponent<HomingMissile>().SetLockDelay(homingLockDelay);

        newMissile.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);

        return newMissile;
    }

    private void InstantiateFreezeMissile(Vector3 position, Vector3 targetPos) {
        var freezeAngle = 180 + UnityEngine.Random.Range(-freezeAngleRange, freezeAngleRange);
        // Debug.Log(homingAngle);
        var freezeRotation = Quaternion.AngleAxis(freezeAngle, Vector3.forward.normalized);
        Vector3 missileStartOffset = freezeRotation * new Vector3(0, 1.5f, 0);

        GameObject newMissile = Instantiate(freezeMissile, position + missileStartOffset, freezeRotation, battlefield.transform);

        newMissile.GetComponent<HomingMissile>().SetTargetPosition(targetPos);
        newMissile.GetComponent<HomingMissile>().SetSpeed(freezeSpeed);
        newMissile.GetComponent<HomingMissile>().SetRotateSpeed(freezeRotateSpeed);
        newMissile.GetComponent<HomingMissile>().SetLockDelay(freezeLockDelay);

        newMissile.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
    }

    private void InstantiateLandmine() {
        GameObject mine = Instantiate(landmine, new Vector3(transform.position.x, minY, 90.6f), Quaternion.identity);
        mine.GetComponent<Landmine>().setIsDamaging(true);

        float mineX = 0f;
        bool xValid = false;

        while (!xValid) {
            xValid = true;
            mineX = UnityEngine.Random.Range(minX, maxX);

            foreach (var prevMine in landminePositions)
            {
                var prevX = prevMine.x;
                if (Math.Abs(prevX - mineX) < landmineMinGap) {
                    xValid = false;
                    break;
                }
            }
        }

        float mineY = 0f;
        bool yValid = false;

        while (!yValid) {
            yValid = true;
            mineY = UnityEngine.Random.Range(minY, landmineMaxY);

            foreach (var prevMine in landminePositions) {
                var prevY = prevMine.y;
                if (Math.Abs(prevY - mineY) < landmineMinGap) {
                    yValid = false;
                    break;
                }
            }
        }

        Vector3 mineTargetPos = new Vector3(mineX, mineY, 90.6f);
        mine.GetComponent<Landmine>().setTargetPos(mineTargetPos);
        landminePositions.Add(mineTargetPos);

        if (!audioSource.isPlaying) {
            audioSource.Play();
        }

        mine.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
    }

    IEnumerator Attack() {
        yield return new WaitForSecondsRealtime(1f);

        // spawn landmines
        for (int i = 0; i < nrLandmines; i++) {
            yield return new WaitForSecondsRealtime(landmineTimeDelay);

            InstantiateLandmine();
        }

        yield return new WaitForSecondsRealtime(1f);

        // fire drop missiles

        float missileX = 0;
        float missileXInc = (maxX - minX) / (nrMissiles - 1);
        
        GameObject[] dropMissiles = new GameObject[nrMissiles];
        for (int i = 0; i < nrMissiles; i++) {
            GameObject currMissile = Instantiate(dropMissile, new Vector3(minX + missileX, minY, transform.position.z), Quaternion.identity, transform.parent);
            currMissile.GetComponent<DropMissile>().setSpeed(dropMissileSpeed);
            currMissile.GetComponent<DropMissile>().setDoesDamage(true);
            currMissile.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);

            dropMissiles[i] = currMissile;

            missileX += missileXInc;
            audioSource.Play();
        }

        yield return new WaitForSecondsRealtime(1f);

        // fire freeze missiles
        var players = GameObject.FindGameObjectsWithTag("Player");

        Vector3 targetPos;
        for (int i = 0; i < nrLandmines; i++) {
            targetPos = freezeRedirect ? landminePositions[i] : players[i % 2].transform.position;
            InstantiateFreezeMissile(transform.position, targetPos);
        }

        yield return new WaitForSecondsRealtime(1f);

        // fire homing missiles
        Transform target;
        for (int i = 0; i < nrMissiles; i++) {
            target = homingRedirect ? dropMissiles[i].transform : players[i % 2].transform;
            InstantiateHomingMissile(transform.position, target);
        }

        fired = true;
    }

    public override bool AttackEnd()
    {
        if (fired) {
            if (GameObject.FindGameObjectsWithTag("Projectile").Length == 0) {
                fired = false;
                landminePositions.Clear();
            }
            return GameObject.FindGameObjectsWithTag("Projectile").Length == 0;
        }
        else return false;
    }
}
