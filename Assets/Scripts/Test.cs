using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Test : NetworkBehaviour
{
    public GameObject battlefield;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) {
            EnableBattleFieldRpc();
            // battlefield.GetComponent<BattleField>().SpawnPlayer();
        }
    }

    [Rpc(SendTo.Everyone)]
    void EnableBattleFieldRpc() {
        if (battlefield.GetComponent<BattleField>().IsActive) {
            // battlefield.SetActive(false);
        }
        else {
            battlefield.GetComponent<BattleField>().SpawnPlayer();
        }
        
    }
}
