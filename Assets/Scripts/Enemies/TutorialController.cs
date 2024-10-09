using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : EnemyController
{
    public TextAsset[] phaseZeroText;
    public TextAsset[] phaseOneText;
    public TextAsset[] phaseTwoText;
    public TextAsset endingText;
    public AudioClip lambdaManDialog;
    public AudioClip hologramDialog;
    public Color lambdaManColor, hologramColor;
    private PlayerState pState;
    
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        pState = PlayerState.Instance;
    }

    public override void PhaseTransition(int phase)
    {
        if (!skipDialogue) {
            switch(phase) {
                case 0:
                    StartCoroutine(phaseZero());
                    break;
                case 1: case 2:
                    pState.updateHealth(100);
                    StartCoroutine(phaseOneTwo(phase));
                    break;
            }
        }
    }

    public override void BattleEnd()
    {
        base.BattleEnd();
        hackButton.interactable = false;
        StartCoroutine(endDialogue());
    }

    IEnumerator endDialogue() {
        dbox.StartDialogue(endingText, hologramColor, "HOLOGRAM", hologramDialog);
        while (!dbox.GetDialogueComplete()) {
            yield return null;
        }
        battleEnded = true;
    }

    IEnumerator phaseZero() {
        hackButton.interactable = false;
        for (int i = 0; i < phaseZeroText.Length; i++) {
            if (i % 2 != 0) {
                dbox.StartDialogue(phaseZeroText[i], hologramColor, "HOLOGRAM", hologramDialog);
            }
            else {
                dbox.StartDialogue(phaseZeroText[i], lambdaManColor, "LAMBDA-MAN", lambdaManDialog);
            }
            if (i == 9) {
                AudioSource audioSource = GetComponent<AudioSource>();
                audioSource.Play();
            }
            while (!dbox.GetDialogueComplete()) {
                yield return null;
            }
        }
        hackButton.interactable = true;
    }

    IEnumerator phaseOneTwo(int phase) {
        hackButton.interactable = false;

        TextAsset[] text;
        if (phase == 1) {
            text = phaseOneText;
        }
        else {
            text = phaseTwoText;
        }
        for (int i = 0; i < text.Length; i++) {
            if (i % 2 == 0) {
                dbox.StartDialogue(text[i], hologramColor, "HOLOGRAM", hologramDialog);
            }
            else {
                dbox.StartDialogue(text[i], lambdaManColor, "LAMBDA-MAN", lambdaManDialog);
            }
            while (!dbox.GetDialogueComplete()) {
                yield return null;
            }
        }
        hackButton.interactable = true;
    }
}
