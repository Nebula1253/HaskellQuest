using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class AttackController : NetworkBehaviour
{
    // template for all attack scripts
    public virtual void Trigger(bool result) {}
    public virtual void Trigger(bool result, string additionalConditions) {
        Debug.Log("NEVER CALL THIS LMAO");
    }

    public abstract bool AttackEnd();

    public virtual bool IsRecursionHandled() {
        return false;
    }
}
