using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoesNothingTransport : NetworkTransport
{
    public override ulong ServerClientId => 12;

    public override void Initialize(NetworkManager networkManager = null)
    {
        // throw new System.NotImplementedException();
    }

    public override void Shutdown()
    {
        // throw new System.NotImplementedException();
    }

    public override bool StartServer()
    {
        return true;
    }

    public override bool StartClient()
    {
        return true;
    }

    public override void DisconnectLocalClient()
    {
        // throw new System.NotImplementedException();
    }

    public override void Send(ulong clientId, ArraySegment<byte> payload, NetworkDelivery networkDelivery)
    {
        // throw new NotImplementedException();
    }

    public override ulong GetCurrentRtt(ulong clientId)
    {
        // throw new System.NotImplementedException();
        return 1;
    }

     public override void DisconnectRemoteClient(ulong clientId)
    {
        // throw new NotImplementedException();
    }
    public override NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime)
    {
        clientId = default;
        payload = default;
        receiveTime = default;
        return NetworkEvent.Nothing;
    }
}
