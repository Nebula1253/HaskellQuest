using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FractalTreeController2P : AttackController
{
    public int boltLayers, fractalLayers;
    public float timeForOneFlash, fractalBorderGap, boltAngle, boltSpeed;
    public Image screenFlash;
    public GameObject drFractal, nodeBolt;
    private GameObject battlefield;
    private GameObject[] fractals;
    private int intDamage = 10;
    private float floatDamage = 0.9f, minX, maxX;

    private bool boltFired = false;

    // Start is called before the first frame update
    void Start()
    {
        var boxSize = transform.parent.GetChild(0).GetComponent<Renderer>().bounds.size;
        var boxCentre = transform.parent.position;
        var boxSizeXOffset = boxSize.x / 2;

        minX = -boxSizeXOffset + fractalBorderGap + boxCentre.x;
        maxX = boxSizeXOffset - fractalBorderGap + boxCentre.x;

        battlefield = GameObject.FindGameObjectWithTag("GameField");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Trigger(bool result)
    {
        StartCoroutine(Attack());
    }

    public override void Trigger(bool result, string additionalConditions)
    {
        ParseDamageVals(additionalConditions);
        
    }

    [Rpc(SendTo.Everyone)]
    void StartAttackRpc() {
        StartCoroutine(Attack());
    }

    private void ParseDamageVals(string additionalConditions) {
        var split = additionalConditions.Split(",");

        var flatDamage = split[0].Trim().Replace("(", "");
        Debug.Log(flatDamage);
        intDamage = int.Parse(flatDamage);

        var ratio = split[1].Trim().Replace(")", "");
        Debug.Log(ratio);
        floatDamage = float.Parse(ratio);

        Debug.Log((intDamage, floatDamage));
    }

    void SpawnFractals(int nrFractals) {
        if (NetworkHelper.Instance.IsPlayerOne) {
            foreach (var fractal in fractals)
            {
                Destroy(fractal);
            }

            // spawn smaller fractals to give the impression that Dr. Fractal is splitting
            fractals = new GameObject[nrFractals];
            float fractalXInc = (maxX - minX) / (nrFractals - 1);
            Debug.Log(fractalXInc);
            float fractalX = minX;
            for (int i = 0; i < nrFractals; i++) {
                Debug.Log(fractalX);
                fractals[i] = Instantiate(drFractal, new Vector3(fractalX, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);
                fractals[i].transform.localScale *= 0.8f;
                fractals[i].GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
                fractalX += fractalXInc;
            }
        }
    }

    IEnumerator Attack() {
        if (NetworkHelper.Instance.IsPlayerOne) {
            fractals = new GameObject[1];
            fractals[0] = Instantiate(drFractal, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);
            fractals[0].GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
        }

        for (int i = 1; i <= fractalLayers; i++) {
            yield return new WaitForSecondsRealtime(0.8f);

            float alpha;
            for (float t = 0; t <= timeForOneFlash / 2; t += Time.deltaTime) {
                alpha = Mathf.Lerp(0, 1, t * 2 / timeForOneFlash);
                screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, alpha);
                yield return null;
            }
            screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, 1);

            SpawnFractals((int) Math.Pow(2, i));

            // purple -> transparent
            for (float t = 0; t <= timeForOneFlash / 2; t += Time.deltaTime) {
                alpha = Mathf.Lerp(1, 0, t * 2 / timeForOneFlash);
                screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, alpha);
                yield return null;
            }
            screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, 0);
        }

        if (NetworkHelper.Instance.IsPlayerOne) {
            for (int i = 0; i < fractals.Length; i++) {
                if (i % 2 == 1) {
                    GameObject node = Instantiate(nodeBolt, fractals[i].transform.position, Quaternion.identity, battlefield.transform);

                    var boltScript = node.GetComponent<NodeBolt>();
                    boltScript.SetLayer(boltLayers);
                    boltScript.SetAngleSpeed(boltAngle, boltSpeed);
                    boltScript.SetDamageValues(intDamage, floatDamage);

                    node.GetComponent<NetworkObject>().Spawn(true);
                }
            }
        }

        boltFired = true;
    }

    public override bool AttackEnd()
    {
        if (boltFired) {
            if (GameObject.FindGameObjectsWithTag("Bolt").Length == 0) {
                boltFired = false;
            }
            return GameObject.FindGameObjectsWithTag("Bolt").Length == 0;
        }
        else return false;
    }
}
