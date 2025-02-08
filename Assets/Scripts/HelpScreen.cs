using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpScreen : MonoBehaviour
{
    public GameObject helpScreen;
    public float time;
    private float distanceDelta;
    private Button helpButton;
    public bool helpScreenActive = false;

    // Start is called before the first frame update
    void Start()
    {
        helpScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(960, helpScreen.GetComponent<RectTransform>().anchoredPosition.y);
        distanceDelta = 960f / time;
    }

    public void AssignButton(Button button) {
        helpButton = button;
        helpButton.onClick.AddListener(Help);
    }

    void Help() {
        if (helpScreenActive) {
            StartCoroutine(HelpScreenDeactivate());
        }
        else {
            StartCoroutine(HelpScreenActivate());
        }
    }

    IEnumerator HelpScreenDeactivate() {
        while (helpScreen.GetComponent<RectTransform>().anchoredPosition.x < 960f) {
            var newX = helpScreen.GetComponent<RectTransform>().anchoredPosition.x + (distanceDelta * Time.deltaTime);
            if (newX > 960f) {
                newX = 960f;
            }
            helpScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(newX, helpScreen.GetComponent<RectTransform>().anchoredPosition.y);
            yield return null;
        }
        helpScreenActive = false;
    }

    IEnumerator HelpScreenActivate() {
        if (GetComponent<ErrorScreen>().errorDisplayActive) {
            GetComponent<ErrorScreen>().Deactivate(); 
        }

        while (helpScreen.GetComponent<RectTransform>().anchoredPosition.x > 0f) {
            var newX = helpScreen.GetComponent<RectTransform>().anchoredPosition.x - (distanceDelta * Time.deltaTime);
            if (newX < 0f) {
                newX = 0f;
            }
            helpScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(newX, helpScreen.GetComponent<RectTransform>().anchoredPosition.y);
            yield return null;
        }
        helpScreenActive = true;
    }

    public void Deactivate() {
        StartCoroutine(HelpScreenDeactivate());
    }

    public void ImmediateDeactivate() {
        helpScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(960, helpScreen.GetComponent<RectTransform>().anchoredPosition.y);
    }

    public void ButtonInteractable(bool interactable) {
        helpButton.interactable = interactable;
    }
}
