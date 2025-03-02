using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalTreeController : AttackController
{
    public int nrLayers;
    public GameObject nodeBolt;
    private int intDamage;
    private float floatDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Trigger(bool result)
    {
        base.Trigger(result);
    }

    public override void Trigger(bool result, string additionalConditions)
    {
        base.Trigger(result, additionalConditions);
    }

    public override bool AttackEnd()
    {
        throw new System.NotImplementedException();
    }
}
