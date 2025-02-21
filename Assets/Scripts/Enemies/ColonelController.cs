using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ColonelController : EnemyController
{
    public GameObject dialog;
    public TextAsset initDialogue, phase1Dialogue, phase2Dialogue, endDialogue;
    public AudioClip lambdaManSFX, colonelSFX, explosion;
    public RuntimeAnimatorController dmgBot, explBot;
    public Color lambdaManColor, colonelColor;
    public Image screenFlash;
    public float timeForOneFlash;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    public override void BattleEnd()
    {
        base.BattleEnd();
        enemySprite.GetComponent<Animator>().runtimeAnimatorController = explBot;
        StartCoroutine(explodeBot());
    }

    public override void PhaseTransition(int phase)
    {
        if (!startCalled) {
            Start();
        }

        if (!skipDialogue) {
            StartCoroutine(dialogue(phase));
        }
    }

    IEnumerator dialogue(int phase) {
        hackButton.interactable = false;

        TextAsset dialogue = initDialogue;
        switch (phase) {
            case 0:
                dialogue = initDialogue;
                break;
            case 1:
                dialogue = phase1Dialogue;
                enemySprite.GetComponent<Animator>().runtimeAnimatorController = dmgBot;
                break;
            case 2:
                dialogue = phase2Dialogue;
                enemySprite.GetComponent<Animator>().speed *= 0.6f;
                break;
        }
        var dialogueSegments = dialogue.text.Split(SWITCH_STR);

        for (int i = 0; i < dialogueSegments.Length; i++) {
            if (i % 2 == 0) {
                dbox.StartDialogue(dialogueSegments[i].Trim(), lambdaManColor, "LAMBDA-MAN", lambdaManSFX);
            }
            else {
                dbox.StartDialogue(dialogueSegments[i].Trim(), colonelColor, "COL. TRIGGER-FINGER", colonelSFX);
            }
            
            while (!dbox.GetDialogueComplete()) {
                yield return null;
            }
        }
        hackButton.interactable = true;
    }

    IEnumerator explodeBot() {
        hackButton.interactable = false;
        var endTextSplit = endDialogue.text.Split(SWITCH_STR);
        var colDialog = endTextSplit[0];
        var lambdaDialog = endTextSplit[1];

        dbox.StartDialogue(colDialog, colonelColor, "COL. TRIGGER-FINGER", colonelSFX);
        while (!dbox.GetDialogueComplete()) {
            yield return null;
        }

        // transparent -> white
        float alpha;
        for (float t = 0; t <= timeForOneFlash / 2; t += Time.deltaTime) {
            alpha = Mathf.Lerp(0, 1, t * 2 / timeForOneFlash);
            screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, alpha);
            yield return null;
        }
        screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, 1);

        enemySprite.SetActive(false);
        GetComponent<AudioSource>().PlayOneShot(explosion, 3);

        // white -> transparent
        for (float t = 0; t <= timeForOneFlash / 2; t += Time.deltaTime) {
            alpha = Mathf.Lerp(1, 0, t * 2 / timeForOneFlash);
            screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, alpha);
            yield return null;
        }
        screenFlash.color = new Color(screenFlash.color.r, screenFlash.color.g, screenFlash.color.b, 0);

        dbox.StartDialogue(lambdaDialog, lambdaManColor, "LAMBDA-MAN", lambdaManSFX);
        while (!dbox.GetDialogueComplete()) {
            yield return null;
        }
        battleEnded = true;
    }
}
