using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackButton : MonoBehaviour
{
    private CodeEditor editor;
    private Button hackButton;
    // Start is called before the first frame update
    void Start()
    {
        editor = GameObject.FindGameObjectWithTag("CodeEditor").GetComponent<CodeEditor>();
        hackButton = GetComponent<Button>();
        hackButton.onClick.AddListener(OnClick);
    }

    void OnClick() {
        Debug.Log("lol");
        editor.MoveOnScreenDummy();
    }
}
