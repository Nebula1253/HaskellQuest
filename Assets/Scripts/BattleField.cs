using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BattleField : NetworkBehaviour
{
    public GameObject player1Prefab, player2Prefab;
    public GameObject battlefieldBG, enemyOverhead;
    public Vector3 spawnPoint;
    public float multiplayerXOffset;
    public bool IsActive = false;
    private bool spawnedPlayer = false;
    private GameObject[] playerRefs;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ActivateBattlefield() {
        battlefieldBG.SetActive(true);
        enemyOverhead.GetComponent<SpriteRenderer>().enabled = true;

        if (!spawnedPlayer) {
            if (IsServer) {
                // host spawns new player for itself
                var player = Instantiate(player1Prefab, transform);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId);

                Debug.Log("HOST SPAWNED");
            }
            else {
                // tell server to spawn new player owned by client
                SpawnPlayerServerRpc();
                Debug.Log("CLIENT SPAWNED");
            }
            spawnedPlayer = true;
        }
        else {
            foreach (var player in playerRefs)
            {
                player.SetActive(true);
            }
        }
        IsActive = true;

        if (IsServer) {
            // start attack
            GameObject.FindGameObjectWithTag("Enemy").GetComponent<AttackController>().Trigger(false);
        }
    }

    public void DeactivateBattlefield() {
        battlefieldBG.SetActive(false);
        enemyOverhead.GetComponent<SpriteRenderer>().enabled = false;
        
        var playerPrefabs = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in playerPrefabs)
        {
            player.SetActive(false);
        }
        playerRefs = playerPrefabs;
        IsActive = false;
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnPlayerServerRpc(ServerRpcParams serverRpcParams = default) {
        var player = Instantiate(player2Prefab, transform);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(serverRpcParams.Receive.SenderClientId);
    }
}
