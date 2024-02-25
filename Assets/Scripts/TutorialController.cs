using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : EnemyController
{
    public GameObject dialog;
    private DialogBox dbox;
    public TextAsset[] phaseZeroText;
    public TextAsset[] phaseOneText;
    public TextAsset[] phaseTwoText;
    public AudioClip lambdaManDialog;
    public AudioClip hologramDialog;
    private PlayerState pState;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        dbox = dialog.GetComponent<DialogBox>();
        pState = GameObject.Find("PlayerState").GetComponent<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void PhaseTransition(int phase)
    {
        switch(phase) {
            case 0:
                StartCoroutine(phaseZero());
                break;
            case 1:
                pState.updateHealth(100);
                StartCoroutine(phaseOne());
                break;
            case 2:
                pState.updateHealth(100);
                StartCoroutine(phaseTwo());
                break;
        }
    }

    public override void Esplode()
    {
        // DO NOT DO ANYTHING
    }

    IEnumerator phaseZero() {
        for (int i = 0; i < phaseZeroText.Length; i++) {
            if (i % 2 != 0) {
                dbox.StartDialogue(phaseZeroText[i], Color.cyan, "HOLOGRAM", hologramDialog);
            }
            else {
                dbox.StartDialogue(phaseZeroText[i], Color.red, "LAMBDA-MAN", lambdaManDialog);
            }
            if (i == 9) {
                AudioSource audioSource = GetComponent<AudioSource>();
                audioSource.Play();
            }
            while (!dbox.GetDialogueComplete()) {
                yield return null;
            }
        }
    }

    IEnumerator phaseOne() {
        for (int i = 0; i < phaseOneText.Length; i++) {
            if (i % 2 == 0) {
                dbox.StartDialogue(phaseOneText[i], Color.cyan, "HOLOGRAM", hologramDialog);
            }
            else {
                dbox.StartDialogue(phaseOneText[i], Color.red, "LAMBDA-MAN", lambdaManDialog);
            }
            while (!dbox.GetDialogueComplete()) {
                yield return null;
            }
        }
    }

    IEnumerator phaseTwo() {
        for (int i = 0; i < phaseTwoText.Length; i++) {
            if (i % 2 == 0) {
                dbox.StartDialogue(phaseTwoText[i], Color.cyan, "HOLOGRAM", hologramDialog);
            }
            else {
                dbox.StartDialogue(phaseTwoText[i], Color.red, "LAMBDA-MAN", lambdaManDialog);
            }
            while (!dbox.GetDialogueComplete()) {
                yield return null;
            }
        }
    }
}
