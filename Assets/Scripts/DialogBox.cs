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
    private string characterName; 
    private int lineCounter = 0;
    private bool advanceEnabled = false;
    


    // Start is called before the first frame update
    void Start()
    {
        advanceButton = GetComponentInChildren<Button>();
        advanceButton.onClick.AddListener(AdvanceDialogue);

        dialogText = GetComponentInChildren<TMP_Text>();

        advanceArrow = transform.Find("AdvanceArrow").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDialogue(TextAsset dialogue) {
        dialogueLines = dialogue.text.Split('\n');
        lineCounter = 0;
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
            dialogText.text = characterName.ToUpper();
        }
        foreach (char c in dialogueLines[lineCounter])
        {
            dialogText.text += c;
            yield return null;
        }
        advanceArrow.SetActive(true);
        advanceEnabled = true;
    }
} 
