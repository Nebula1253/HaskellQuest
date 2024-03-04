using System.Collections;
using System.Collections.Generic;
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
        dbox = GameObject.Find("DialogBox").GetComponent<DialogBox>();
        StartCoroutine(startCutscene());
    }

    IEnumerator startCutscene() {
        // dbox.StartDialogue(cutsceneText, cutsceneAudio);
        dbox.StartDialogue(cutsceneText);
        while (!dbox.GetDialogueComplete()) {
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
