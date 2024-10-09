using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;

// TAKEN FROM UNITY THEMSELVES
public class OwnerNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
