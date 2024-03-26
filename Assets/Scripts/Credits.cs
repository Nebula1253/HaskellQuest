using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public AudioClip creditsMusic, secretMusic;
    public TextAsset creditsText, secretText;
    public float scrollSpeed, secretScrollSpeed, creditsEndY, secretEndY;
    private float actualScrollSpeed, actualCreditsEndY;
    private AudioSource audioSource;
    private TMP_Text creditsTMP;
    private bool creditsStarted = false;
    private bool secretDone = false;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        creditsTMP = GetComponentInChildren<TMP_Text>();

        StartCoroutine(secret());
    }

    IEnumerator secret() {
        yield return new WaitForSecondsRealtime(0.8f);
        creditsStarted = true;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Credits started: " + creditsStarted);
        // Debug.Log("Secret done: " + secretDone);
        if (creditsStarted) {
            transform.Translate(Vector3.up * actualScrollSpeed * Time.deltaTime);
        }
        else {
            // silly!
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W)) {
                if (!secretDone) {
                    audioSource.clip = secretMusic;
                    creditsTMP.text = secretText.text;
                    actualScrollSpeed = secretScrollSpeed;
                    actualCreditsEndY = secretEndY;
                    secretDone = true;
                } 
            } else {
                audioSource.clip = creditsMusic;
                creditsTMP.text = creditsText.text;
                actualScrollSpeed = scrollSpeed;
                actualCreditsEndY = creditsEndY;
            } 
        }
        Debug.Log(transform.position.y);
        if (transform.position.y >= actualCreditsEndY) {
            SceneManager.LoadScene("StartScreen");
        }
    }

    void OnBecameInvisible() {
        StartCoroutine(ExitScene());
    }

    IEnumerator ExitScene() {
        yield return new WaitForSeconds(1f);
        Debug.Log("Exiting scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("IntroCutscene");
    }
}
