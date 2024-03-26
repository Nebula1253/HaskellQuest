using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColonelController : EnemyController
{
    public GameObject dialog;
    private DialogBox dbox;
    public TextAsset[] phaseZeroText, phaseOneText, phaseTwoText, endText;
    public AudioClip lambdaManDialog, colonelDialog, explosion;
    public RuntimeAnimatorController dmgBot, explBot;
    public Color lambdaManColor, colonelColor;
    public Image screenFlash;
    private Button hackButton;
    public float timeForOneFlash;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        // dbox = dialog.GetComponent<DialogBox>();
        dbox = DialogBox.Instance;
        hackButton = GameObject.Find("HackButton").GetComponent<Button>();
    }

    public override void BattleEnd()
    {
        base.BattleEnd();
        enemySprite.GetComponent<Animator>().runtimeAnimatorController = explBot;
        StartCoroutine(explodeBot());
    }

    public override void PhaseTransition(int phase)
    {
        StartCoroutine(dialogue(phase));
    }

    IEnumerator dialogue(int phase) {
        hackButton.interactable = false;

        TextAsset[] dialogue = phaseZeroText;
        switch(phase) {
            case 0:
                dialogue = phaseZeroText;
                break;
            case 1:
                dialogue = phaseOneText;
                enemySprite.GetComponent<Animator>().runtimeAnimatorController = dmgBot;
                break;
            case 2:
                dialogue = phaseTwoText;
                enemySprite.GetComponent<Animator>().speed *= 0.6f;
                break;
        }

        for (int i = 0; i < dialogue.Length; i++) {
            if (i % 2 == 0) {
                dbox.StartDialogue(dialogue[i], lambdaManColor, "LAMBDA-MAN", lambdaManDialog);
            }
            else {
                dbox.StartDialogue(dialogue[i], colonelColor, "COL. TRIGGER-FINGER", colonelDialog);
            }
            
            while (!dbox.GetDialogueComplete()) {
                yield return null;
            }
        }
        hackButton.interactable = true;
    }

    IEnumerator explodeBot() {
        hackButton.interactable = false;
        dbox.StartDialogue(endText[0], colonelColor, "COL. TRIGGER-FINGER", colonelDialog);
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

        dbox.StartDialogue(endText[1], lambdaManColor, "LAMBDA-MAN", lambdaManDialog);
        while (!dbox.GetDialogueComplete()) {
            yield return null;
        }
        battleEnded = true;
    }
}
