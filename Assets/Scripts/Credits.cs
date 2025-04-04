using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public AudioClip creditsMusic;
    public TextAsset creditsText;
    public float scrollSpeed, creditsEndY;
    private AudioSource audioSource;
    private TMP_Text creditsTMP;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        creditsTMP = GetComponentInChildren<TMP_Text>();

        // StartCoroutine(secret());
        audioSource.clip = creditsMusic;
        creditsTMP.text = creditsText.text;

        audioSource.Play();

        StartCoroutine(SafelyDestroyNetworkManager());
    }

    IEnumerator SafelyDestroyNetworkManager() {
        yield return new WaitForSecondsRealtime(5f);

        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.Shutdown();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
        
        if (GetComponent<RectTransform>().anchoredPosition.y >= creditsEndY) {
            SceneManager.LoadScene("StartScreen");
        }
    }

    void OnBecameInvisible() {
        StartCoroutine(ExitScene());
    }

    IEnumerator ExitScene() {
        yield return new WaitForSeconds(1f);
        Debug.Log("Exiting scene");
        SceneManager.LoadScene(0);
    }
}
