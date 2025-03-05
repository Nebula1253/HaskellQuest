using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DropMissilesLandminesController : AttackController
{
    public GameObject missile, landmine;
    public int nrMissiles, nrMissileBarrages;
    public float missileSpeed, borderXGap, missileBarrageTimeDelay, missileYOffset;
    public int nrLandmines;
    public float landmineTimeDelay, landmineMinGap;

    private bool fired = false;
    private float minX, maxX, minY;
    public float maxY;
    private AudioSource audioSource;

    private List<float> prevLandmineX = new List<float>();
    private List<float> prevLandmineY = new List<float>();

    private bool missilesFiltered = false, landminesFiltered = false;
    // Start is called before the first frame update
    void Start()
    {
        var boxSize = transform.parent.GetChild(0).GetComponent<Renderer>().bounds.size;
        var boxCentre = transform.parent.position;
        var boxSizeXOffset = boxSize.x / 2;

        minX = -boxSizeXOffset + boxCentre.x + borderXGap;
        maxX = boxSizeXOffset + boxCentre.x - borderXGap;
        minY = transform.position.y + missileYOffset;

        audioSource = GetComponent<AudioSource>();
    }

    public override void Trigger(bool result)
    {
        SetFilters("Neither works");
        StartCoroutine(DropMinesMissiles());
    }

    public override void Trigger(bool result, string additionalConditions)
    {
        SetFilters(additionalConditions);
        StartCoroutine(DropMinesMissiles());
    }

    private void SetFilters(string additionalConditions) {
        switch (additionalConditions) {
            case "Neither works":
                missilesFiltered = false;
                landminesFiltered = false;
                break;

            case "Only missile works":
                missilesFiltered = true;
                landminesFiltered = false;
                break;

            case "Only landmine works":
                missilesFiltered = false;
                landminesFiltered = true;
                break;

            case "Both work":
                missilesFiltered = true;
                landminesFiltered = true;
                break;
        }
    }

    private void InstantiateLandmine(bool isDamaging) {
        GameObject mine = Instantiate(landmine, new Vector3(transform.position.x, minY, 90.6f), Quaternion.identity);
        mine.GetComponent<Landmine>().setIsDamaging(isDamaging);

        float mineX = 0f;
        bool xValid = false;

        while (!xValid) {
            xValid = true;
            mineX = UnityEngine.Random.Range(minX, maxX);

            foreach (var prevX in prevLandmineX)
            {
                if (Math.Abs(prevX - mineX) < landmineMinGap) {
                    xValid = false;
                }
            }
        }
        prevLandmineX.Add(mineX);

        float mineY = 0f;
        bool yValid = false;

        while (!yValid) {
            yValid = true;
            mineY = UnityEngine.Random.Range(minY, maxY);

            foreach (var prevY in prevLandmineY) {
                if (Math.Abs(prevY - mineY) < landmineMinGap) {
                    yValid = false;
                }
            }
        }
        prevLandmineY.Add(mineY);

        Vector3 mineTargetPos = new Vector3(mineX, mineY, 90.6f);

        // GameObject mine = Instantiate(landmine, mineTargetPos, Quaternion.identity);
        // mine.GetComponent<Landmine>().setIsDamaging(isDamaging);
        mine.GetComponent<Landmine>().setTargetPos(mineTargetPos);

        if (!audioSource.isPlaying) {
            audioSource.Play();
        }

        mine.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
    }

    IEnumerator DropMinesMissiles() {
        int safeLandmine = UnityEngine.Random.Range(0, nrLandmines);
        for (int i = 0; i < nrLandmines; i++) {
            yield return new WaitForSecondsRealtime(landmineTimeDelay);

            if (i == safeLandmine) {
                InstantiateLandmine(false);
            }
            else {
                if (!landminesFiltered) {
                    InstantiateLandmine(true);
                }
            }
        }

        float missileXInc = (maxX - minX) / (nrMissiles - 1);
        float missileX;

        for (int j = 0; j < nrMissileBarrages; j++) {
            yield return new WaitForSecondsRealtime(missileBarrageTimeDelay);
            missileX = 0;
            
            int safeMissile = UnityEngine.Random.Range(0, nrMissiles);
            for (int i = 0; i < nrMissiles; i++) {
                if (i == safeMissile) {
                    GameObject currMissile = Instantiate(missile, new Vector3(minX + missileX, minY, transform.position.z), Quaternion.identity, transform.parent);
                    currMissile.GetComponent<DropMissile>().setSpeed(missileSpeed);
                    currMissile.GetComponent<DropMissile>().setDoesDamage(false);

                    currMissile.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
                    audioSource.Play();
                }
                else {
                    if (!missilesFiltered) {
                        GameObject currMissile = Instantiate(missile, new Vector3(minX + missileX, minY, transform.position.z), Quaternion.identity, transform.parent);
                        currMissile.GetComponent<DropMissile>().setSpeed(missileSpeed);
                        currMissile.GetComponent<DropMissile>().setDoesDamage(true);

                        currMissile.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
                        if (!audioSource.isPlaying) {
                            audioSource.Play();
                        }
                    }
                }
                missileX += missileXInc;
            }
        }

        fired = true;
    }

    public override bool AttackEnd()
    {
        if (fired) {
            if (GameObject.FindGameObjectsWithTag("Projectile").Length == 0) {
                fired = false;
                prevLandmineX.Clear();
                prevLandmineY.Clear();
            }
            return GameObject.FindGameObjectsWithTag("Projectile").Length == 0;
        }
        else return false;
    }
}
