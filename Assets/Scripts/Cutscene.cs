using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    public TextAsset cutsceneText1P, cutsceneText2P;
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
        if (NetworkHelper.Instance.IsMultiplayer) {
            dbox.StartDialogue(cutsceneText2P.text, cutsceneAudio);
        }
        else {
            dbox.StartDialogue(cutsceneText1P.text, cutsceneAudio);
        }
        
        while (!dbox.GetDialogueComplete()) {
            yield return null;
        }

        if (NetworkHelper.Instance.IsPlayerOne) {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1), LoadSceneMode.Single);
        }
    }
}
