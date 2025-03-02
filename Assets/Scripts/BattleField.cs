using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;

public class BattleField : NetworkBehaviour
{
    public GameObject playerPrefab, player1Prefab, player2Prefab;
    public GameObject battlefieldBG, enemyOverhead1P, enemyOverhead2P;
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
        // enemyOverhead1P.GetComponent<SpriteRenderer>().enabled = true;
        if (NetworkHelper.Instance.IsMultiplayer) {
            enemyOverhead2P.GetComponent<SpriteRenderer>().enabled = true;
        }
        else {
            enemyOverhead1P.GetComponent<SpriteRenderer>().enabled = true;
        }

        if (!spawnedPlayer) {
            if (NetworkHelper.Instance.IsPlayerOne) {
                // host spawns new player for itself
                GameObject player;
                if (NetworkHelper.Instance.IsMultiplayer) {
                    // multiplayer
                    Vector3 playerSpawn = new Vector3(spawnPoint.x - multiplayerXOffset, spawnPoint.y, spawnPoint.z);
                    player = Instantiate(player1Prefab, playerSpawn, Quaternion.identity, transform);
                }
                else {
                    // single-player
                    player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity, transform);
                }
                
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId, true);

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

        // if (IsServer) {
        //     // start attack
        //     GameObject.FindGameObjectWithTag("Enemy").GetComponent<AttackController>().Trigger(false);
        // }
    }

    public void DeactivateBattlefield() {
        battlefieldBG.SetActive(false);
        
        enemyOverhead1P.GetComponent<SpriteRenderer>().enabled = false;
        enemyOverhead2P.GetComponent<SpriteRenderer>().enabled = false;
        
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
        // assumption - in a singe-player game, the player starts as the host and there are no clients
        // therefore, if the server / host received this RPC, there has to have been a client, and we are in 2P mode - thus the offset needs to be added
        Vector3 playerSpawn = new Vector3(spawnPoint.x + multiplayerXOffset, spawnPoint.y, spawnPoint.z);

        var player = Instantiate(player2Prefab, playerSpawn, Quaternion.identity, transform);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(serverRpcParams.Receive.SenderClientId);

        Debug.Log("CLIENT SPAWNED");
    }
}
