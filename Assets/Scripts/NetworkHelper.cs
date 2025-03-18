using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using ParrelSync;
#endif

using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkHelper : NetworkBehaviour
{
    // largely to make life easier while developing
    public bool IsMultiplayer { get; private set; }
    public bool IsPlayerOne { get; private set; }
    public int debugNrPlayers;
    public static NetworkHelper Instance { get; private set; }
    public GameObject disconnectOverlay;
    private Button backToMenuButton;

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
#if UNITY_EDITOR
            // testing multiplayer
            IsMultiplayer = true;
            IsPlayerOne = !ClonesManager.IsClone();
#endif
        }
        else if (debugNrPlayers == 1) {
            // testing singleplayer
            IsMultiplayer = false;
            IsPlayerOne = true;
        }
        else if (debugNrPlayers == 0) {
            // actual game
            IsPlayerOne = NetworkManager.Singleton.IsHost;
            IsMultiplayer = IsPlayerOne ? NetworkManager.Singleton.ConnectedClientsList.Count > 1 : true;
        }

        backToMenuButton = disconnectOverlay.GetComponentInChildren<Button>();
        backToMenuButton.onClick.AddListener(BackToMenu);

        NetworkManager.Singleton.OnClientDisconnectCallback += Disconnect;
    }

    private void Disconnect(ulong obj)
    {
        Debug.Log("DISCONNECT DETECTED");
        Time.timeScale = 0f;
        disconnectOverlay.SetActive(true);
    }

    void BackToMenu() {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);
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

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientDisconnectCallback -= Disconnect;
        }
    }
}
