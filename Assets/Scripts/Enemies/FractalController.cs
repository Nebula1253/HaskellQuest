using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalController : EnemyController
{
    public GameObject dialog;
    public TextAsset phase0Text1P, phase1Text1P, phase2Text1P, endText1P;
    public TextAsset phase0Text2P, phase1Text2P, phase2Text2P, endText2P;
    public AudioClip lambdaManDialog, fractalDialog, kowalewskiDialog;
    public Color lambdaManColor, fractalColor, kowalewskiColor, player1Color, player2Color;
    public RuntimeAnimatorController kowalewski;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    public override void BattleEnd()
    {
        base.BattleEnd();
        StartCoroutine(saveKowalewski());
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

    IEnumerator phaseDialogue(int phase) {
        var isMultiplayer = NetworkHelper.Instance.IsMultiplayer;

        TextAsset dialogue = isMultiplayer ? phase0Text2P : phase0Text1P;
        switch (phase) {
            case 0:
                dialogue = isMultiplayer ? phase0Text2P : phase0Text1P;
                break;
            case 1:
                dialogue = isMultiplayer ? phase1Text2P : phase1Text1P;
                break;
            case 2:
                dialogue = isMultiplayer ? phase2Text2P : phase2Text1P;
                break;
        }
        var dialogueSegments = dialogue.text.Split(SWITCH_STR);

        for (int i = 0; i < dialogueSegments.Length; i++) {
            if (isMultiplayer) {
                if (i % 3 == 0) {
                    dbox.StartDialogue(dialogueSegments[i].Trim(), player1Color, "LAMBDA-MAN 1", lambdaManDialog);
                }
                else if (i % 3 == 1) {
                    dbox.StartDialogue(dialogueSegments[i].Trim(), player2Color, "LAMBDA-MAN 2", lambdaManDialog);
                }
                else {
                    dbox.StartDialogue(dialogueSegments[i].Trim(), fractalColor, "DR. FRACTAL", fractalDialog); 
                }
            }
            else {
                if (i % 2 == 0) {
                    dbox.StartDialogue(dialogueSegments[i].Trim(), lambdaManColor, "LAMBDA-MAN", lambdaManDialog);
                }
                else {
                    dbox.StartDialogue(dialogueSegments[i].Trim(), fractalColor, "DR. FRACTAL", fractalDialog);
                }
            }
            
            while (!dbox.GetDialogueComplete()) {
                yield return null;
            }
        }
    }

    IEnumerator saveKowalewski() {
        var isMultiplayer = NetworkHelper.Instance.IsMultiplayer;

        var endTextSplit = isMultiplayer ? endText2P.text.Split(SWITCH_STR) : endText1P.text.Split(SWITCH_STR);

        dbox.StartDialogue(endTextSplit[0].Trim(), fractalColor, "DR. FRACTAL", fractalDialog);

        while (!dbox.GetDialogueComplete()) {
            yield return null;
        }

        enemySprite.GetComponent<Animator>().runtimeAnimatorController = kowalewski;
        yield return new WaitForSeconds(2f);

        dbox.StartDialogue(endTextSplit[1].Trim(), kowalewskiColor, "DR. KOWALEWSKI", kowalewskiDialog);
        while (!dbox.GetDialogueComplete()) {
            yield return null;
        }

        if (isMultiplayer) {
            dbox.StartDialogue(endTextSplit[2].Trim(), player1Color, "LAMBDA-MAN 1", lambdaManDialog);
            while (!dbox.GetDialogueComplete()) {
                yield return null;
            }

            dbox.StartDialogue(endTextSplit[3].Trim(), player2Color, "LAMBDA-MAN 2", lambdaManDialog);
            while (!dbox.GetDialogueComplete()) {
                yield return null;
            }
        }
        else {
            dbox.StartDialogue(endTextSplit[2].Trim(), lambdaManColor, "LAMBDA-MAN", lambdaManDialog);
            while (!dbox.GetDialogueComplete()) {
                yield return null;
            }
        }
        

        battleEnded = true;
    }
}
