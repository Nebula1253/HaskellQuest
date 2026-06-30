using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    public GameObject start1P, start2P, credits, status, exit, setup2PUI, startUI;
    private Button start1PButton, start2PButton, creditsButton, exitButton;
    private TMP_Text statusText;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;

        start1PButton = start1P.GetComponent<Button>();
        start2PButton = start2P.GetComponent<Button>();
        creditsButton = credits.GetComponent<Button>();
        exitButton = exit.GetComponent<Button>();
        statusText = status.GetComponent<TMP_Text>();

        start1PButton.onClick.AddListener(() => StartCoroutine(StartGame(true)));
        start2PButton.onClick.AddListener(() => StartCoroutine(StartGame(false)));
        creditsButton.onClick.AddListener(Credits);
        exitButton.onClick.AddListener(() => Application.Quit());
    }

    IEnumerator StartGame(bool singleplayer) {
        string url = "https://play.haskell.org/versions";

        using (UnityWebRequest haskellPlaygroundReq = UnityWebRequest.Get(url)) {
            yield return haskellPlaygroundReq.SendWebRequest();

            if (haskellPlaygroundReq.result == UnityWebRequest.Result.Success) {
                if (haskellPlaygroundReq.downloadHandler.text.Contains("8.10.7"))
                {
                    if (singleplayer) {
                        Destroy(NetworkManager.Singleton.GetComponent<UnityTransport>());
                        
                        DoesNothingTransport newTransport = NetworkManager.Singleton.gameObject.AddComponent<DoesNothingTransport>();
                        NetworkManager.Singleton.NetworkConfig.NetworkTransport = newTransport;

                        NetworkManager.Singleton.StartHost();
                        NetworkManager.Singleton.SceneManager.LoadScene(SceneUtility.GetScenePathByBuildIndex(1), LoadSceneMode.Single);
                    
                    }
                    else {
                        setup2PUI.SetActive(true);
                        startUI.SetActive(false);
                    }
                }
                else
                {
                    statusText.color = Color.red;
                    statusText.text = "The Haskell compilation service no longer offers the language version used.";
                }
            }
            else {
                statusText.color = Color.red;
                statusText.text = "Cannot access the online compiler. Please try again later.\n" + haskellPlaygroundReq.error;
            }
        }
    }

    private void Credits() {
        SceneManager.LoadScene("Credits");
    }
}
