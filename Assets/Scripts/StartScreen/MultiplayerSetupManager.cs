using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerSetupManager : MonoBehaviour
{
    public GameObject hostUI, joinUI, multiplayerSetupUI, initUI;
    public Button hostButton, joinButton, backButton;

    // Start is called before the first frame update
    void Start()
    {
        hostButton.onClick.AddListener(Host);
        joinButton.onClick.AddListener(Join);
        backButton.onClick.AddListener(Back);
    }

    void Host() {
        NetworkManager.Singleton.Shutdown();
        hostUI.SetActive(true);
        joinUI.SetActive(false);

        hostUI.GetComponent<HostUI>().StartHost();
    }

    void Join() {
        NetworkManager.Singleton.Shutdown();
        joinUI.SetActive(true);
        hostUI.SetActive(false);
    }

    void Back() {
        NetworkManager.Singleton.Shutdown();
        hostUI.SetActive(false);
        joinUI.SetActive(false);

        multiplayerSetupUI.SetActive(false);
        initUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
