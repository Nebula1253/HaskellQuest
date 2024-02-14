using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ComboMissileController : AttackController
{
    private bool missilesFired = false;
    public GameObject missile, homingMissile;
    public int nrMissiles;
    public float dropMissileSpeed, dropMissileBorderGap, dropMissileYOffset;
    public float homingMissileSpeed, homingMissileRotateSpeed, homingMissileLockDelay, homingMissileAngleRange;
    private Transform playerTransform;
    private float minX, maxX;

    // Start is called before the first frame update
    void Start()
    {
        var boxSize = transform.parent.GetChild(0).GetComponent<Renderer>().bounds.size;
        var boxCentre = transform.parent.position;
        var boxSizeXOffset = boxSize.x / 2;

        minX = -boxSizeXOffset + boxCentre.x + dropMissileBorderGap;
        maxX = boxSizeXOffset + boxCentre.x - dropMissileBorderGap;
        Debug.Log("min X " + minX);
        Debug.Log("max X " + maxX);

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log(playerTransform);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool AttackEnd()
    {
        if (!missilesFired) return false;

        if (GameObject.FindGameObjectsWithTag("Projectile").Length == 0) {
            missilesFired = false;
        }
        return GameObject.FindGameObjectsWithTag("Projectile").Length == 0;
    }

    public override void Trigger(bool result)
    {
        StartCoroutine(fireMissiles(result));
    }

    private GameObject InstantiateMissile(Vector3 position, Transform target) {
        var missileAngle = 180 + Random.Range(-homingMissileAngleRange, homingMissileAngleRange);
        Debug.Log(missileAngle);

        var missileRotation = Quaternion.AngleAxis(missileAngle, Vector3.forward.normalized);
        Vector3 missileStartOffset = missileRotation * new Vector3(0, 2f, 0);

        GameObject newMissile = Instantiate(homingMissile, position + missileStartOffset, missileRotation, transform.parent);
        Debug.Log(newMissile);
        Debug.Log(target);
        newMissile.GetComponent<HomingMissile>().SetTarget(target);
        newMissile.GetComponent<HomingMissile>().SetSpeed(homingMissileSpeed);
        newMissile.GetComponent<HomingMissile>().SetRotateSpeed(homingMissileRotateSpeed);
        newMissile.GetComponent<HomingMissile>().SetLockDelay(homingMissileLockDelay);

        return newMissile;
    }

    IEnumerator fireMissiles(bool retarget) {
        yield return new WaitForSeconds(1f);

        // GameObject[] homingMissiles = new GameObject[nrMissiles];
        float missileX = 0, missileY = transform.position.y + dropMissileYOffset;
        float missileXInc = (maxX - minX) / (nrMissiles - 1);
        
        // fire drop missiles
        GameObject[] dropMissiles = new GameObject[nrMissiles];
        for (int i = 0; i < nrMissiles; i++) {
            GameObject currMissile = Instantiate(missile, new Vector3(minX + missileX, missileY, transform.position.z), Quaternion.identity, transform.parent);
            currMissile.GetComponent<DropMissile>().setSpeed(dropMissileSpeed);
            currMissile.GetComponent<DropMissile>().setDoesDamage(true);
            dropMissiles[i] = currMissile;

            missileX += missileXInc;
        }

        yield return new WaitForSeconds(1.5f);

        // fire homing missiles, retarget homing missiles if the player has passed the test
        for (int i=0; i < nrMissiles; i++) {
            Debug.Log("Firing missile " + i);
            var target = playerTransform;
            if (retarget) {
                target = dropMissiles[i].transform;
            }

            InstantiateMissile(transform.position, target);
            yield return new WaitForSeconds(0.1f);
        }

        missilesFired = true;
    }
}
