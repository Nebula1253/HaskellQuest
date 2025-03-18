using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
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

        start1PButton.onClick.AddListener(() => StartGame(true));
        start2PButton.onClick.AddListener(() => StartGame(false));
        creditsButton.onClick.AddListener(Credits);
        exitButton.onClick.AddListener(() => Application.Quit());
    }

    private void StartGame(bool singleplayer) {
        string url = "https://www.jdoodle.com/execute-haskell-online";
        try {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (singleplayer) {
                        Destroy(NetworkManager.Singleton.GetComponent<UnityTransport>());
                        UnityTransport newTransport = NetworkManager.Singleton.gameObject.AddComponent<UnityTransport>();
                        newTransport.SetConnectionData("127.0.0.1", 7777, "0.0.0.0");
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
                    statusText.text = "The online compiler is inaccessible. Please try again later.";
                }
            }
        }
        catch (WebException e) {
            statusText.color = Color.red;
            statusText.text = "Error connecting to the online compiler. Please check your Internet connection.";
            statusText.text += "\n" + e.Message;
        }
    }

    private void Credits() {
        SceneManager.LoadScene("Credits");
    }
}
