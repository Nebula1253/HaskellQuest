using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoEndScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.Shutdown();

        StartCoroutine(ReturnToStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ReturnToStart() {
        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
