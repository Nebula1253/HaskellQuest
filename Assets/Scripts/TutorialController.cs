using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : EnemyController
{
    public GameObject dialog;
    private DialogBox dbox;
    public TextAsset phaseZeroText, phaseOneText;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        dbox = dialog.GetComponent<DialogBox>();
        PhaseTransition(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void PhaseTransition(int phase)
    {
        switch(phase) {
            case 0:
                dbox.StartDialogue(phaseZeroText, Color.cyan, "LAMBDA-MAN");
                break;
            case 1:
                dbox.StartDialogue(phaseOneText, Color.cyan, "LAMBDA-MAN");
                break;
            case 2:
                break;
        }
    }
}
