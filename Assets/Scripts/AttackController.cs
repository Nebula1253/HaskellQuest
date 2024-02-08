using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackController : MonoBehaviour
{
    public abstract void Trigger(bool result);

    public abstract bool AttackEnd();
}
