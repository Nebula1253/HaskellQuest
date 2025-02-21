using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaletteChanger : MonoBehaviour
{
    public GameObject codeEditorBG, helpScreenBG, errorScreenBG;
    public GameObject[] buttons;

    public Sprite codeEditorP1Sprite, codeEditorP2Sprite;
    public Sprite helpScreenP1Sprite, helpScreenP2Sprite;
    public Sprite errorScreenP1Sprite, errorScreenP2Sprite;
    public Color buttonBGColorP1, buttonBGColorP2, buttonTextColorP1, buttonTextColorP2;

    // Start is called before the first frame update
    void Start()
    {
        if (NetworkHelper.Instance.IsMultiplayer) {
            if (NetworkHelper.Instance.IsPlayerOne) {
                codeEditorBG.GetComponent<Image>().sprite = codeEditorP1Sprite;
                helpScreenBG.GetComponent<Image>().sprite = helpScreenP1Sprite;
                errorScreenBG.GetComponent<Image>().sprite = errorScreenP1Sprite;

                foreach (var button in buttons)
                {
                    button.GetComponent<Image>().color = buttonBGColorP1;
                    button.GetComponentInChildren<TMP_Text>().color = buttonTextColorP1;
                }
            }
            else {
                codeEditorBG.GetComponent<Image>().sprite = codeEditorP2Sprite;
                helpScreenBG.GetComponent<Image>().sprite = helpScreenP2Sprite;
                errorScreenBG.GetComponent<Image>().sprite = errorScreenP2Sprite;

                foreach (var button in buttons) {
                    button.GetComponent<Image>().color = buttonBGColorP2;
                    button.GetComponentInChildren<TMP_Text>().color = buttonTextColorP2;
                }
            }
        }
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}
