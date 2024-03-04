using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FractalLaserController : AttackController
{
    public GameObject drFractal, laser;
    public Image screenFlash;
    public float laserSpeed, fractalBorderGap, timeForOneFlash;
    public int nrLasersWhenLost, nrLasersWhenWon, nrFractals;
    private GameObject[] fractals;
    private float minX, maxX;
    private bool lasersFired = false;

    void Awake()
    {
        var boxSize = transform.parent.GetChild(0).GetComponent<Renderer>().bounds.size;
        var boxCentre = transform.parent.position;
        var boxSizeXOffset = boxSize.x / 2;

        minX = -boxSizeXOffset + fractalBorderGap + boxCentre.x;
        maxX = boxSizeXOffset - fractalBorderGap + boxCentre.x;
        Debug.Log("min X " + minX);
        Debug.Log("max X " + maxX);
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
        // if the player successfully writes split, spawn 8 smaller enemies
        // if the player fails, spawn 1 big enemy
        StartCoroutine(spawnAndFire(result));
    }

    IEnumerator spawnAndFire(bool result) {
        // if the player successfully writes split, spawn 8 smaller enemies
        // if the player fails, spawn 1 big enemy
        int nrLasers = nrLasersWhenLost;

        if (fractals == null) {
            fractals = new GameObject[1];
            fractals[0] = Instantiate(drFractal, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);
        }

        yield return new WaitForSecondsRealtime(1f);

        if (result) {
            nrLasers = nrLasersWhenWon;

            // transparent -> purple
            float alpha;
            for (float t = 0; t <= timeForOneFlash / 2; t += Time.deltaTime) {
                alpha = Mathf.Lerp(0, 1, t * 2 / timeForOneFlash);
                screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, alpha);
                yield return null;
            }
            screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, 1);

            // spawn smaller fractals to give the impression that Dr. Fractal is splitting
            Destroy(fractals[0].gameObject);
            fractals = new GameObject[nrFractals];
            float fractalXInc = (maxX - minX) / (nrFractals - 1);
            Debug.Log(fractalXInc);
            float fractalX = minX;
            for (int i = 0; i < nrFractals; i++) {
                Debug.Log(fractalX);
                fractals[i] = Instantiate(drFractal, new Vector3(fractalX, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);
                fractals[i].transform.localScale *= 0.5f;
                fractalX += fractalXInc;
            }

            // purple -> transparent
            for (float t = 0; t <= timeForOneFlash / 2; t += Time.deltaTime) {
                alpha = Mathf.Lerp(1, 0, t * 2 / timeForOneFlash);
                screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, alpha);
                yield return null;
            }
            screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, 0);
        }

        var yOffset = 1.1f;
        if (result) yOffset *= 0.5f;

        // fire lasers from each Fractal
        foreach (GameObject fractal in fractals) {
            Debug.Log("firing lasers");
            for (int i = 0; i < nrLasers; i++) {
                GameObject currLaser = Instantiate(laser, new Vector3(fractal.transform.position.x, fractal.transform.position.y - yOffset, fractal.transform.position.z), 
                                                    Quaternion.identity, transform.parent);
                if (result) currLaser.transform.localScale *= 0.5f;
                currLaser.GetComponent<FractalLaser>().SetVariables(laserSpeed, result);
                yield return new WaitForSecondsRealtime(0.7f);
            }
        }

        if (result) {
            // transparent -> purple
            float alpha;
            for (float t = 0; t <= timeForOneFlash / 2; t += Time.deltaTime) {
                alpha = Mathf.Lerp(0, 1, t * 2 / timeForOneFlash);
                screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, alpha);
                yield return null;
            }
            screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, 1);

            // remove smaller fractals, replace with the bigger one
            foreach (GameObject fractal in fractals) {
                Destroy(fractal.gameObject);
            }
            fractals = new GameObject[1];
            fractals[0] = Instantiate(drFractal, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);

            // purple -> transparent
            for (float t = 0; t <= timeForOneFlash / 2; t += Time.deltaTime) {
                alpha = Mathf.Lerp(1, 0, t * 2 / timeForOneFlash);
                screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, alpha);
                yield return null;
            }
            screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, 0);
        }

        lasersFired = true;
    }
}
