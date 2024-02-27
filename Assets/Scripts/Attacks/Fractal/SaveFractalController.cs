using System.Collections;
using System.Collections.Generic;
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
        var preexisting = GameObject.FindGameObjectsWithTag("Fractal");
        foreach (var fractal in preexisting) {
            Destroy(fractal);
        }

        spawnedFractal = Instantiate(fractal, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, transform.parent).transform;
        StartCoroutine(fireLasers(result));
    }

    IEnumerator fireLasers(bool result) {
        yield return new WaitForSeconds(1);

        if (!result) {
            for (int i = 0; i < nrLasers; i++) {
                var laserInstance = Instantiate(laser, new Vector3(spawnedFractal.position.x, spawnedFractal.position.y - 1.3f, spawnedFractal.position.z), 
                                                Quaternion.identity, transform.parent);
                laserInstance.GetComponent<FractalLaser>().SetVariables(laserSpeed, result);
                yield return new WaitForSeconds(0.25f);
            }
        }

        lasersFired = true;
    }
}
