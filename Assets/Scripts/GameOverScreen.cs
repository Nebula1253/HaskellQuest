using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : NetworkBehaviour
{
    private Button restartButton, quitButton;
    // Start is called before the first frame update
    void Start()
    {
        restartButton = transform.Find("RetryButton").GetComponent<Button>();
        restartButton.onClick.AddListener(RetryBattleRpc);

        quitButton = transform.Find("QuitButton").GetComponent<Button>();
        quitButton.onClick.AddListener(QuitToMenu);
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void EnableScreen() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
        }
    }

    [Rpc(SendTo.Everyone)]
    void RetryBattleRpc() {
        if (NetworkHelper.Instance.IsPlayerOne) {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }

    void QuitToMenu() {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton);
        
        SceneManager.LoadScene(0);
    }
}
