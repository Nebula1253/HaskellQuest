using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalController : EnemyController
{
    public GameObject dialog;
    public TextAsset[] phaseZeroText, phaseOneText, endText;
    public AudioClip lambdaManDialog, fractalDialog, kowalewskiDialog;
    public Color lambdaManColor, fractalColor, kowalewskiColor;
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
        if (!skipDialogue) {
            StartCoroutine(phaseDialogue(phase));
        }
    }

    IEnumerator phaseDialogue(int phase) {
        TextAsset[] dialogue;
        if (phase == 0) {
            dialogue = phaseZeroText;
        }
        else {
            dialogue = phaseOneText;
        }

        for (int i = 0; i < dialogue.Length; i++) {
            if (i % 2 == 0) {
                dbox.StartDialogue(dialogue[i], lambdaManColor, "LAMBDA-MAN", lambdaManDialog);
            }
            else {
                dbox.StartDialogue(dialogue[i], fractalColor, "DR. FRACTAL", fractalDialog);
            }
            
            while (!dbox.GetDialogueComplete()) {
                yield return null;
            }
        }
    }

    IEnumerator saveKowalewski() {
        dbox.StartDialogue(endText[0], fractalColor, "DR. FRACTAL", fractalDialog);

        while (!dbox.GetDialogueComplete()) {
            yield return null;
        }

        enemySprite.GetComponent<Animator>().runtimeAnimatorController = kowalewski;
        yield return new WaitForSeconds(2f);    

        dbox.StartDialogue(endText[1], kowalewskiColor, "DR. KOWALEWSKI", kowalewskiDialog);
        while (!dbox.GetDialogueComplete()) {
            yield return null;
        }

        dbox.StartDialogue(endText[2], lambdaManColor, "LAMBDA-MAN", lambdaManDialog);
        while (!dbox.GetDialogueComplete()) {
            yield return null;
        }

        battleEnded = true;
    }
}
