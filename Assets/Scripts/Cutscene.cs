using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    public TextAsset cutsceneText;
    public AudioClip cutsceneAudio;
    private DialogBox dbox;
    // Start is called before the first frame update
    void Start()
    {
        // dbox = GameObject.Find("DialogBox").GetComponent<DialogBox>();
        dbox = DialogBox.Instance;
        StartCoroutine(startCutscene());
    }

    IEnumerator startCutscene() {
        dbox.StartDialogue(cutsceneText.text, cutsceneAudio);
        // dbox.StartDialogue(cutsceneText);
        while (!dbox.GetDialogueComplete()) {
            yield return null;
        }
        NetworkManager.Singleton.SceneManager.LoadScene(SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1), LoadSceneMode.Single);
    }
}
