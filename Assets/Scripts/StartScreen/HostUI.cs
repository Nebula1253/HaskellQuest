using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;
using System.Net;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class HostUI : MonoBehaviour
{
    public TMP_Text joinCodeText, connectStatus;
    public Button startButton;
    private UnityTransport transport;
    private Guid relayAllocationID;

    // Start is called before the first frame update
    void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        startButton.onClick.AddListener(StartGame);

        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
    }

    void StartGame() {
        NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnect;
        NetworkManager.Singleton.ConnectionApprovalCallback -= ConnectApproval;

        NetworkManager.Singleton.SceneManager.LoadScene(SceneUtility.GetScenePathByBuildIndex(1), LoadSceneMode.Single);
    }

    public async void StartHost() {
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnect;
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectApproval;

        try {
            var joinCode = await StartHostWithRelay();
            joinCodeText.text = joinCode;
        }
        catch (Exception e) {
            connectStatus.text = e.Message;
        }
        
    }

    // taken directly from Unity's tutorial on Relay with Netcode for GameObjects
    private async Task<string> StartHostWithRelay(int maxConnections=2)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        relayAllocationID = allocation.AllocationId;

        transport.SetRelayServerData(new RelayServerData(allocation, "dtls"));
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(relayAllocationID);
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    void ClientConnect(ulong clientId) {
        if (clientId != NetworkManager.Singleton.LocalClientId) {
            connectStatus.text = "Client connected!";
            startButton.gameObject.SetActive(true);
        }
    }

    void ClientDisconnect(ulong obj)
    {
        connectStatus.text = "Waiting for Player 2 to connect...";
        startButton.gameObject.SetActive(false);
    }

    void ConnectApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count >= 2) {
            response.Approved = false;
            response.Reason = "Only one client allowed.";
        }
        else {
            response.Approved = true;
            response.Pending = false;
        }
    }
}
