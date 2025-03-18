using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FractalLaserController2P : AttackController
{
    private bool splitWorks = false, depowerWorks = false;
    public GameObject drFractal, laser;
    public float laserSpeed, fractalBorderGap, timeForOneFlash;
    public int nrFractalsWhenSplit;
    public Image screenFlash;
    private float minX, maxX;
    private int nrLasers, damage, nrFractals;
    private GameObject[] fractals;
    private bool lasersFired = false;

    // Start is called before the first frame update
    void Start()
    {
        var boxSize = transform.parent.GetChild(0).GetComponent<Renderer>().bounds.size;
        var boxCentre = transform.parent.position;
        var boxSizeXOffset = boxSize.x / 2;

        minX = -boxSizeXOffset + fractalBorderGap + boxCentre.x;
        maxX = boxSizeXOffset - fractalBorderGap + boxCentre.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Trigger(bool result)
    {
        Debug.Log("Trigger called without additional conditions");
        DecideVars();
        StartAttackRpc(this.splitWorks);
    }

    public override void Trigger(bool result, string additionalConditions)
    {
        Debug.Log("Trigger called WITH additional conditions");
        ParseAdditionalConditions(additionalConditions);
        DecideVars();
        StartAttackRpc(this.splitWorks);
    }

    void ParseAdditionalConditions(string additionalConditions) {
        var split = additionalConditions.Split(",");

        splitWorks = split[0].Replace("(", "").Trim() == "True";
        depowerWorks = split[1].Replace(")", "").Trim() == "True";

        Debug.Log((splitWorks, depowerWorks));
    }

    void DecideVars() {
        if (splitWorks) {
            nrFractals = nrFractalsWhenSplit;
            if (depowerWorks) {
                nrLasers = 2;
                damage = 2;
            }
            else {
                nrLasers = 1;
                damage = 8;
            }
        }
        else {
            nrFractals = 1;
            if (depowerWorks) {
                nrLasers = 16;
                damage = 4;
            }
            else {
                nrLasers = 8;
                damage = 12;
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    void StartAttackRpc(bool split) {
        Debug.Log("Fractal attack started!");
        StartCoroutine(AttackPart1(split));
    }

    IEnumerator AttackPart1(bool splitWorks) {
        // Debug.Log("are we getting stuck here?");
        if (NetworkHelper.Instance.IsPlayerOne) {
            fractals = new GameObject[1];
            fractals[0] = Instantiate(drFractal, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);
            fractals[0].GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
        }
        // Debug.Log("or here?");

        yield return new WaitForSecondsRealtime(1f);

        // Debug.Log("perhaps here?");

        if (splitWorks) {
            // Debug.Log("omg hiiii");
            // transparent -> purple
            float alpha;
            for (float t = 0; t <= timeForOneFlash / 2; t += Time.deltaTime) {
                alpha = Mathf.Lerp(0, 1, t * 2 / timeForOneFlash);
                screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, alpha);
                yield return null;
            }
            screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, 1);

            if (NetworkHelper.Instance.IsPlayerOne) {
                // spawn smaller fractals to give the impression that Dr. Fractal is splitting
                Destroy(fractals[0]);
                fractals = new GameObject[nrFractals];
                float fractalXInc = (maxX - minX) / (nrFractals - 1);
                Debug.Log(fractalXInc);
                float fractalX = minX;
                for (int i = 0; i < nrFractals; i++) {
                    Debug.Log(fractalX);
                    fractals[i] = Instantiate(drFractal, new Vector3(fractalX, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);
                    fractals[i].transform.localScale *= 0.5f;
                    fractals[i].GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
                    fractalX += fractalXInc;
                }
            }

            // purple -> transparent
            for (float t = 0; t <= timeForOneFlash / 2; t += Time.deltaTime) {
                alpha = Mathf.Lerp(1, 0, t * 2 / timeForOneFlash);
                screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, alpha);
                yield return null;
            }
            screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, 0);
        }

        if (NetworkHelper.Instance.IsPlayerOne) {
            var yOffset = 1.1f;
            if (splitWorks) yOffset *= 0.5f;

            // fire lasers from each Fractal
            int whichPlayer = 0;
            foreach (GameObject fractal in fractals) {
                for (int i = 0; i < nrLasers; i++) {
                    GameObject currLaser = Instantiate(laser, new Vector3(fractal.transform.position.x, fractal.transform.position.y - yOffset, fractal.transform.position.z), 
                                                        Quaternion.identity, transform.parent);

                    currLaser.GetComponent<FractalLaser>().SetVariables(laserSpeed, damage);
                    currLaser.GetComponent<FractalLaser>().SetWhichPlayer(whichPlayer);
                    currLaser.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);

                    whichPlayer = (whichPlayer == 0) ? 1 : 0;

                    yield return new WaitForSecondsRealtime(0.7f);
                }
            }

            ContinueAttackRpc(this.splitWorks);
        }
    }

    [Rpc(SendTo.Everyone)]
    void ContinueAttackRpc(bool split) {
        Debug.Log("Fractal attack continues!");
        StartCoroutine(AttackPart2(split));
    }

    IEnumerator AttackPart2(bool splitWorks) {
        if (splitWorks) {
            // transparent -> purple
            float alpha;
            for (float t = 0; t <= timeForOneFlash / 2; t += Time.deltaTime) {
                alpha = Mathf.Lerp(0, 1, t * 2 / timeForOneFlash);
                screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, alpha);
                yield return null;
            }
            screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, 1);

            if (NetworkHelper.Instance.IsPlayerOne) {
                // remove smaller fractals, replace with the bigger one
                foreach (GameObject fractal in fractals) {
                    Destroy(fractal);
                }
                fractals = new GameObject[1];
                fractals[0] = Instantiate(drFractal, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);
                fractals[0].GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
            }

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
}
