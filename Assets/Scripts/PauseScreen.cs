using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreen : NetworkBehaviour
{
    public bool canBePaused = true;
    private bool isPaused = false;
    private Button quitButton, resumeButton;

    // Start is called before the first frame update
    void Start()
    {
        quitButton = transform.Find("QuitButton").GetComponent<Button>();
        quitButton.onClick.AddListener(QuitToMenu);

        resumeButton = transform.Find("ResumeButton").GetComponent<Button>();
        resumeButton.onClick.AddListener(ResumeBtn);
    }

    // Update is called once per frame
    void Update()
    {
        if (canBePaused) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (isPaused) {
                    ResumeGameRpc();
                }
                else {
                    PauseGameRpc();
                }
            }
        }
    }

    void ResumeBtn() {
        if (isPaused) {
            ResumeGameRpc();
        }
    }

    void QuitToMenu() {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton);

        SceneManager.LoadScene(0);
    }

    [Rpc(SendTo.Everyone)]
    void PauseGameRpc() {
        Time.timeScale = 0f;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        isPaused = true;
    }

    [Rpc(SendTo.Everyone)]
    void ResumeGameRpc() {
        Time.timeScale = 1f;

        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }

        isPaused = false;
    }
}
