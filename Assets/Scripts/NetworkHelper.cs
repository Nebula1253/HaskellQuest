using System.Collections;
using System.Collections.Generic;
using ParrelSync;
using Unity.Netcode;
using UnityEngine;

public class NetworkHelper : NetworkBehaviour
{
    // largely to make life easier while developing
    public bool IsMultiplayer { get; private set; }
    public bool IsPlayerOne { get; private set; }
    public int debugNrPlayers;
    public static NetworkHelper Instance { get; private set; }

    private void Awake() 
    {
        // there should only be one instance per scene
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }

        if (debugNrPlayers == 2) {
            // testing multiplayer
            IsMultiplayer = true;
            IsPlayerOne = !ClonesManager.IsClone();
        }
        else if (debugNrPlayers == 1) {
            // testing singleplayer
            IsMultiplayer = false;
            IsPlayerOne = true;
        }
        else if (debugNrPlayers == 0) {
            // actual game
            IsPlayerOne = IsServer;
            IsMultiplayer = IsPlayerOne ? NetworkManager.ConnectedClientsList.Count > 1 : true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // presumably handling of disconnects will go here
    }
}
