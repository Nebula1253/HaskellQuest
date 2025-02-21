using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;

public class JoinUI : MonoBehaviour
{
    public TMP_InputField joinCodeInputField;
    public Button connectButton;
    public TMP_Text statusText;

    // Start is called before the first frame update
    void Start()
    {
        connectButton.onClick.AddListener(async () => await ConnectToHost());
    }

    async Task ConnectToHost() {
        joinCodeInputField.interactable = false;
        connectButton.interactable = false;
        statusText.text = "Connecting to Player 1...";

        try {
            var connectResult = await StartClientWithRelay(joinCodeInputField.text.ToUpper());
            if (connectResult) {
                statusText.text = "Connected! Waiting for Player 1 to start game...";
            }
            else {
                statusText.text = "Connection failed, please retry";
                connectButton.interactable = true;
                joinCodeInputField.interactable = true;
            }
        }
        catch (Exception e) {
            statusText.text = e.Message;
            connectButton.interactable = true;
            joinCodeInputField.interactable = true;
        }
    }

    // taken directly from Unity's tutorial on Relay with Netcode for GameObjects
    private async Task<bool> StartClientWithRelay(string joinCode)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }
}
