using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMissileController : EnemyController
{
    private bool missilesFired = false;
    public GameObject missile;
    public int nrMissiles;
    public float missileSpeed, missileBorderGap;
    private float minX, maxX;
    // Start is called before the first frame update
    void Start()
    {
        var boxSize = transform.parent.GetChild(0).GetComponent<Renderer>().bounds.size;
        var boxCentre = transform.parent.position;
        var boxSizeXOffset = boxSize.x / 2;

        minX = -boxSizeXOffset + boxCentre.x + missileBorderGap;
        maxX = boxSizeXOffset + boxCentre.x - missileBorderGap;
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public override void Trigger(bool result)
    {
        var missileXInc = (maxX - minX) / (nrMissiles - 1);
        float missileX = 0;

        // randomly choose a missile to do damage
        int missileToDamage = Random.Range(0, nrMissiles);
        for (int i = 0; i < nrMissiles; i++) {
            GameObject currMissile = Instantiate(missile, new Vector3(minX + missileX, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);
            currMissile.GetComponent<DropMissile>().setSpeed(missileSpeed);
            if (i == missileToDamage) {
                currMissile.GetComponent<DropMissile>().setDoesDamage(true);
            }
            else {
                currMissile.GetComponent<DropMissile>().setDoesDamage(false);
            }
        }
    }
}
