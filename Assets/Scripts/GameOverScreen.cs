using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    private Button restartButton;
    // Start is called before the first frame update
    void Start()
    {
        restartButton = transform.Find("RetryButton").GetComponent<Button>();
        restartButton.onClick.AddListener(RetryBattle);
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    void RetryBattle() {
        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
