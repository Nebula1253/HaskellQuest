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

        // this is stupid lol
        // the sprites are identical, the only advantage of having different objects is attaching different attack scripts
        // which in itself is a problematic choice given the sheer amount of code duplication
        if (NetworkHelper.Instance.IsMultiplayer) {
            if (enemyOverhead2P.GetComponent<SpriteRenderer>() != null) {
                enemyOverhead2P.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        else {
            if (enemyOverhead1P.GetComponent<SpriteRenderer>() != null) {
                enemyOverhead1P.GetComponent<SpriteRenderer>().enabled = true;
            }
        }

        if (!spawnedPlayer) {
            if (NetworkHelper.Instance.IsPlayerOne) {
                if (NetworkHelper.Instance.IsMultiplayer) {
                    // multiplayer - spawn both player objects
                    Vector3 player1Spawn = new Vector3(spawnPoint.x - multiplayerXOffset, spawnPoint.y, spawnPoint.z);
                    GameObject player1 = Instantiate(player1Prefab, player1Spawn, Quaternion.identity, transform);

                    player1.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId, true);

                    Vector3 player2Spawn = new Vector3(spawnPoint.x + multiplayerXOffset, spawnPoint.y, spawnPoint.z);
                    GameObject player2 = Instantiate(player2Prefab, player2Spawn, Quaternion.identity, transform);

                    ulong player2ID = 1;
                    foreach (var x in NetworkManager.Singleton.ConnectedClientsIds) {
                       if (x != OwnerClientId) {
                        player2ID = x;
                       }
                    }

                    player2.GetComponent<NetworkObject>().SpawnAsPlayerObject(player2ID, true);
                    player2.GetComponent<NetworkObject>().ChangeOwnership(player2ID);
                }
                else {
                    // single-player
                    GameObject player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity, transform);
                    player.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId, true);
                }
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
    }

    public void DeactivateBattlefield() {
        battlefieldBG.SetActive(false);
        
        if (enemyOverhead1P.GetComponent<SpriteRenderer>() != null) {
            enemyOverhead1P.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (enemyOverhead2P.GetComponent<SpriteRenderer>() != null) {
            enemyOverhead2P.GetComponent<SpriteRenderer>().enabled = false;
        }
        
        var playerPrefabs = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in playerPrefabs)
        {
            player.SetActive(false);
        }
        playerRefs = playerPrefabs;
        IsActive = false;

        if (NetworkHelper.Instance.IsPlayerOne) {
            foreach (var mine in GameObject.FindGameObjectsWithTag("Landmine"))
            {
                Destroy(mine);
            }

            foreach (var fractal in GameObject.FindGameObjectsWithTag("Fractal"))
            {
                Destroy(fractal);
            }
        }
    }
}
