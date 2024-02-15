using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : EnemyController
{
    public GameObject dialog;
    private DialogBox dbox;
    public TextAsset phaseZeroText, phaseOneText;
    // Start is called before the first frame update
    void Start()
    {
        dbox = dialog.GetComponent<DialogBox>();
        PhaseTransition(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void PhaseTransition(int phase)
    {
        base.PhaseTransition(phase);
        switch(phase) {
            case 0:
                dbox.StartDialogue(phaseZeroText, Color.blue, "LAMBDA-MAN");
                break;
            case 1:
                dbox.StartDialogue(phaseOneText, Color.blue, "LAMBDA-MAN");
                break;
            case 2:
                break;
        }
    }
}
