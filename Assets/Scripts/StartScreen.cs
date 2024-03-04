using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    public GameObject start, credits, status;
    private Button startButton, creditsButton;
    private TMP_Text statusText;

    // Start is called before the first frame update
    void Start()
    {
        startButton = start.GetComponent<Button>();
        creditsButton = credits.GetComponent<Button>();
        statusText = status.GetComponent<TMP_Text>();

        startButton.onClick.AddListener(StartGame);
        creditsButton.onClick.AddListener(Credits);
    }

    private void StartGame() {
        string url = "https://www.jdoodle.com/execute-haskell-online";
        try {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
