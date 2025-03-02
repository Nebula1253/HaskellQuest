using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial2Controller : EnemyController
{
    public TextAsset initDialogue1P, endText1P;
    public TextAsset initDialogue2P, endText2P;
    public AudioClip lambdaManDialog;
    public AudioClip hologramDialog;
    public Color lambdaManColor, hologramColor;
    public Color player1Color, player2Color;
    private PlayerState pState;
    
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        pState = PlayerState.Instance;
    }

    public override void PhaseTransition(int phase)
    {
        if (!startCalled) {
            Start();
        }

        if (!skipDialogue) {
            StartCoroutine(phaseDialogue(phase));
        }
    }

    public override void BattleEnd()
    {
        base.BattleEnd();
        StartCoroutine(phaseDialogue(1));
    }
    
    IEnumerator phaseDialogue(int phase) {
        hackButton.interactable = false;
        TextAsset dialogue = initDialogue1P;
        bool isMultiplayer = NetworkHelper.Instance.IsMultiplayer;
        
        switch (phase) {
            case 0:
                dialogue = isMultiplayer ? initDialogue2P : initDialogue1P;
                break;
            case 1:
                dialogue = isMultiplayer ? endText2P : endText1P;
                HealPlayers();
                break;
        }
        var dialogueSegments = dialogue.text.Split(SWITCH_STR);

        if (isMultiplayer) {
            for (int i = 0; i < dialogueSegments.Length; i++) { 
                if (i % 3 == 0) {
                    dbox.StartDialogue(dialogueSegments[i].Trim(), player1Color, "LAMBDA-MAN 1", lambdaManDialog);
                }
                else if (i % 3 == 1) {
                    dbox.StartDialogue(dialogueSegments[i].Trim(), player2Color, "LAMBDA-MAN 2", lambdaManDialog);
                }
                else {
                    dbox.StartDialogue(dialogueSegments[i].Trim(), hologramColor, "HOLOGRAM", hologramDialog);
                }

                while (!dbox.GetDialogueComplete()) {
                    yield return null;
                }
            }
        }
        else {
            for (int i = 0; i < dialogueSegments.Length; i++) {
                if (i % 2 != 0) {
                    dbox.StartDialogue(dialogueSegments[i].Trim(), hologramColor, "HOLOGRAM", hologramDialog);
                }
                else {
                    dbox.StartDialogue(dialogueSegments[i].Trim(), lambdaManColor, "LAMBDA-MAN", lambdaManDialog);
                }

                while (!dbox.GetDialogueComplete()) {
                    yield return null;
                }
            }
        }
        
        if (phase == 1) {
            battleEnded = true;
        }
        else {
            hackButton.interactable = true;
        }
    }

    void HealPlayers() {
        if (NetworkHelper.Instance.IsPlayerOne) {
            pState.updateHealth(100, 0);

            if (NetworkHelper.Instance.IsMultiplayer) pState.updateHealth(100, 1);
        }
    }
}
