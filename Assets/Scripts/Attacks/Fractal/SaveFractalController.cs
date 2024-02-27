using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveFractalController : AttackController
{
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool AttackEnd() {
        return true;
    }
    
    public override bool IsRecursionHandled()
    {
        return true;
    }
}
