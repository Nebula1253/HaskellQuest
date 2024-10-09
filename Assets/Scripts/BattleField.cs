using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BattleField : NetworkBehaviour
{
    public GameObject playerPrefab;
    public bool IsActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SpawnPlayer() {
        var children = GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(true);
        }

        if (IsServer) {
            // host spawns new player for itself
            var player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId);

            Debug.Log("HOST SPAWNED");
        }
        else {
            // tell server to spawn new player owned by client
            SpawnPlayerServerRpc();
            Debug.Log("CLIENT SPAWNED");
        }
        IsActive = true;
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnPlayerServerRpc(ServerRpcParams serverRpcParams = default) {
        var player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(serverRpcParams.Receive.SenderClientId);
    }
}
