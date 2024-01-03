using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { START, PLAYER_ACT, PLAYER_HACK, PLAYER_ITEM, ENEMYTURN, WON, LOST }

public class StateController : MonoBehaviour
{
    public BattleState state;
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
