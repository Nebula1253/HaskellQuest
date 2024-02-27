using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackController : MonoBehaviour
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
