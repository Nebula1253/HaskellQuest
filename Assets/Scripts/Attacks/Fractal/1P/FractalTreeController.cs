using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalTreeController : AttackController
{
    public int nrLayers;
    public float boltAngle, boltSpeed;
    public GameObject nodeBolt, fractal;
    private GameObject battlefield;
    private int intDamage = 10;
    private float floatDamage = 0.9f;
    private bool boltFired = false;

    // Start is called before the first frame update
    void Start()
    {
        battlefield = GameObject.FindGameObjectWithTag("GameField");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Trigger(bool result)
    {
        SpawnRootNode();
    }

    public override void Trigger(bool result, string additionalConditions)
    {
        ParseDamageVals(additionalConditions);
        SpawnRootNode();
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

    void SpawnRootNode() {
        Instantiate(fractal, new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z), Quaternion.identity, transform.parent);

        GameObject node = Instantiate(nodeBolt, transform.position, Quaternion.identity, battlefield.transform);

        var boltScript = node.GetComponent<NodeBolt>();
        boltScript.SetLayer(nrLayers);
        boltScript.SetAngleSpeed(boltAngle, boltSpeed);
        boltScript.SetDamageValues(intDamage, floatDamage);

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
