using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SaveFractalController : AttackController
{
    public GameObject fractal, laser;
    private Transform spawnedFractal;
    public float laserSpeed;
    public int nrLasers;
    private bool lasersFired = false;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool AttackEnd()
    {
        if (lasersFired) {
            if (GameObject.FindGameObjectsWithTag("Projectile").Length == 0) {
                lasersFired = false;
            }
            return GameObject.FindGameObjectsWithTag("Projectile").Length == 0;
        }
        else {
            return false;
        }
    }

    public override void Trigger(bool result)
    {
        spawnedFractal = Instantiate(fractal, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, transform.parent).transform;
        StartCoroutine(fireLasers(result));
    }

    IEnumerator fireLasers(bool result) {
        yield return new WaitForSecondsRealtime(1);

        if (!result) {
            for (int i = 0; i < nrLasers; i++) {
                var laserInstance = Instantiate(laser, new Vector3(spawnedFractal.position.x, spawnedFractal.position.y - 1.1f, spawnedFractal.position.z), 
                                                Quaternion.identity, transform.parent);
                laserInstance.GetComponent<FractalLaser>().SetVariables(laserSpeed, 10);
                if (NetworkHelper.Instance.IsMultiplayer) {
                    laserInstance.GetComponent<FractalLaser>().SetWhichPlayer(i % 2);
                    laserInstance.GetComponent<NetworkObject>().Spawn(true);
                }
                else {
                    laserInstance.GetComponent<FractalLaser>().SetWhichPlayer();
                }
                yield return new WaitForSecondsRealtime(0.25f);
            }
        }

        lasersFired = true;
    }
}
