using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMissileController : AttackController
{
    private bool missilesFired = false;
    public GameObject missile;
    public int nrMissiles, nrMissileBarrages;
    public float missileSpeed, missileBorderGap, missileBarrageTimeDelay, missileYOffset;
    private float minX, maxX;
    private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        var boxSize = transform.parent.GetChild(0).GetComponent<Renderer>().bounds.size;
        var boxCentre = transform.parent.position;
        var boxSizeXOffset = boxSize.x / 2;

        minX = -boxSizeXOffset + boxCentre.x + missileBorderGap;
        maxX = boxSizeXOffset + boxCentre.x - missileBorderGap;
        Debug.Log("min X " + minX);
        Debug.Log("max X " + maxX);

        audioSource = GetComponent<AudioSource>();
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
        Start(); // don't know why I have to force this but whatever
        StartCoroutine(DropMissiles(result));
    }

    IEnumerator DropMissiles(bool filter) {
        float missileXInc = (maxX - minX) / (nrMissiles - 1);
        float missileX, missileY;
        missileY = transform.position.y + missileYOffset;

        for (int j = 0; j < nrMissileBarrages; j++) {
            yield return new WaitForSeconds(missileBarrageTimeDelay);
            missileX = 0;
            
            int safeMissile = Random.Range(0, nrMissiles);
            for (int i = 0; i < nrMissiles; i++) {
                if (i == safeMissile) {
                    GameObject currMissile = Instantiate(missile, new Vector3(minX + missileX, missileY, transform.position.z), Quaternion.identity, transform.parent);
                    currMissile.GetComponent<DropMissile>().setSpeed(missileSpeed);
                    currMissile.GetComponent<DropMissile>().setDoesDamage(false);
                    audioSource.Play();
                }
                else {
                    if (!filter) {
                        GameObject currMissile = Instantiate(missile, new Vector3(minX + missileX, missileY, transform.position.z), Quaternion.identity, transform.parent);
                        currMissile.GetComponent<DropMissile>().setSpeed(missileSpeed);
                        currMissile.GetComponent<DropMissile>().setDoesDamage(true);
                        if (!audioSource.isPlaying) {
                            audioSource.Play();
                        }
                    }
                }
                missileX += missileXInc;
            }
        }
        missilesFired = true;
    }

}
