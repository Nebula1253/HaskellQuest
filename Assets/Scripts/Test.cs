using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Test : NetworkBehaviour
{
    public GameObject battlefield;
    public GameObject enemy;
    public bool debug;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && debug) {
            EnableBattleFieldRpc(); // everyone enables the battlefield and the players
        }
    }

    [Rpc(SendTo.Everyone)]
    void EnableBattleFieldRpc() {
        if (battlefield.GetComponent<BattleField>().IsActive) {
            battlefield.GetComponent<BattleField>().DeactivateBattlefield();
        }
        else {
            battlefield.GetComponent<BattleField>().ActivateBattlefield();
        }
    }
}
