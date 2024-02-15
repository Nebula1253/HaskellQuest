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
    private Color nameColor;
    private Button hackButton;
    private string characterName; 
    private int lineCounter = 0;
    private bool advanceEnabled = false;
    private bool dialogueComplete = false;
    


    // Start is called before the first frame update
    void Start()
    {
        advanceButton = GetComponentInChildren<Button>();
        advanceButton.onClick.AddListener(AdvanceDialogue);

        dialogText = GetComponentInChildren<TMP_Text>();

        advanceArrow = transform.Find("AdvanceArrow").gameObject;

        hackButton = GameObject.Find("HackButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetDialogueComplete() {
        return dialogueComplete;
    }

    public void StartDialogue(TextAsset dialogue) {
        Start();

        gameObject.SetActive(true);
        hackButton.interactable = false;
        dialogueLines = dialogue.text.Split('\n');
        lineCounter = 0;
        dialogueComplete = false;
        
        StartCoroutine(runThruDialogue());
    }

    public void StartDialogue(TextAsset dialogue, Color nameColor, string characterName) {
        this.nameColor = nameColor;
        this.characterName = characterName;
        StartDialogue(dialogue);
    }

    private void AdvanceDialogue() {
        if (advanceEnabled) {
            lineCounter++;
            if (lineCounter >= dialogueLines.Length) {
                gameObject.SetActive(false);
                dialogueComplete = true;
                hackButton.interactable = true;
            }
            else {
                advanceEnabled = false;
                StartCoroutine(runThruDialogue());
                advanceArrow.SetActive(false);
            }
        }
    }

    IEnumerator runThruDialogue() {
        if (nameColor != null && characterName != "") {
            dialogText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(nameColor) + ">" + characterName.ToUpper() + "</color>: ";
        }
        foreach (char c in dialogueLines[lineCounter])
        {
            dialogText.text += c;
            yield return new WaitForSeconds(0.015f);
        }
        advanceArrow.SetActive(true);
        advanceEnabled = true;
    }
} 
