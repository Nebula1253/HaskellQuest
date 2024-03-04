using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    private TMP_Text dialogText;
    private GameObject advanceArrow;
    private Button advanceButton;

    private string[] dialogueLines;
    private Color nameColor = Color.clear;
    
    private string characterName = ""; 
    private int lineCounter = 0;
    private bool advanceEnabled = false;
    private bool dialogueComplete = false;
    private bool startCalled = false; // rot in hell
    private AudioSource source;
    private AudioClip clip;
    


    // Start is called before the first frame update
    void Start()
    {
        // dumbest fucking bug ever
        if (!startCalled) {
            advanceButton = GameObject.Find("ScreenClick").GetComponent<Button>();
            advanceButton.onClick.AddListener(AdvanceDialogue);

            dialogText = GetComponentInChildren<TMP_Text>();

            advanceArrow = transform.Find("AdvanceArrow").gameObject;
            source = GetComponent<AudioSource>();

            startCalled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool GetDialogueComplete() {
        return dialogueComplete;
    }

    public void StartDialogue(TextAsset dialogue) {
        // despite changing the order directly in build settings, the script calling StartDialogue has its Start() method called BEFORE the actual DialogueBox Start()
        // so basically the StartDialogue() code executes before the Start() code does
        // I'm force-calling Start() here and have added a stupid bool to make sure it doesn't get called a second time
        Start();

        gameObject.SetActive(true);
        dialogueLines = dialogue.text.Split('\n');
        lineCounter = 0;
        dialogueComplete = false;

        advanceArrow.SetActive(false);
        advanceEnabled = false;
        
        StartCoroutine(runThruDialogue());
    }

    public void StartDialogue(TextAsset dialogue, AudioClip clip) {
        this.clip = clip;
        StartDialogue(dialogue);
    }

    public void StartDialogue(TextAsset dialogue, Color nameColor, string characterName, AudioClip clip) {
        this.nameColor = nameColor;
        this.characterName = characterName;
        this.clip = clip;
        StartDialogue(dialogue);
    }

    private void AdvanceDialogue() {
        if (!dialogueComplete) {
            if (advanceEnabled) {
                lineCounter++;
                advanceEnabled = false;

                if (lineCounter > dialogueLines.Length - 1) {
                    gameObject.SetActive(false);
                    dialogueComplete = true;
                }
                else {
                    StartCoroutine(runThruDialogue());
                    advanceArrow.SetActive(false);
                }
            }
            else {
                // complete the line flat out and allow the player to move forward
                StopAllCoroutines();

                if (!nameColor.Equals(Color.clear) && characterName != "") {
                    dialogText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(nameColor) + ">" + characterName.ToUpper() + "</color>: ";
                }
                else {
                    dialogText.text = "";
                }
                dialogText.text += dialogueLines[lineCounter];
                
                advanceArrow.SetActive(true);
                advanceEnabled = true;
            }
        }
    }

    IEnumerator runThruDialogue() {
        if (!nameColor.Equals(Color.clear) && characterName != "") {
            dialogText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(nameColor) + ">" + characterName.ToUpper() + "</color>: ";
        }
        else {
            dialogText.text = "";
        }

        char[] line = dialogueLines[lineCounter].ToCharArray();

        int i = 0;
        while (i < line.Length) {
            char c = line[i];
       
            // handling of HTML markup tags
            if (c == '<' && dialogueLines[lineCounter].Substring(i).Contains('>')) {
                while (c != '>') {
                    dialogText.text += c;
                    i++;
                    c = line[i];
                }
                dialogText.text += '>';
            }
            else {
                dialogText.text += c;
                if (!source.isPlaying && clip != null) {
                    source.PlayOneShot(clip);
                }
                yield return new WaitForSecondsRealtime(0.03f);
            }
            i++;
        }
        advanceArrow.SetActive(true);
        advanceEnabled = true;
    }
} 
