using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorScreen : MonoBehaviour
{
    public float time;
    private Button errorsButton;
    public bool errorDisplayActive = false;
    private float distanceDelta;

    public GameObject errorDisplay;

    // Start is called before the first frame update
    void Start()
    {
        errorDisplay.GetComponent<RectTransform>().anchoredPosition = new Vector2(960f, errorDisplay.GetComponent<RectTransform>().anchoredPosition.y);
        distanceDelta = 960f / time;
    }

    public void AssignButton(Button button) {
        errorsButton = button;
        errorsButton.onClick.AddListener(Errors);
    }

    void Errors() {
        if (errorDisplayActive) {
            StartCoroutine(ErrorScreenDeactivate());
        }
        else {
            StartCoroutine(ErrorScreenActivate());
        }
    }

    IEnumerator ErrorScreenDeactivate() {
        while (errorDisplay.GetComponent<RectTransform>().anchoredPosition.x < 960f) {
            var newX = errorDisplay.GetComponent<RectTransform>().anchoredPosition.x + (distanceDelta * Time.deltaTime);
            if (newX > 960f) {
                newX = 960f;
            }
            errorDisplay.GetComponent<RectTransform>().anchoredPosition = new Vector2(newX, errorDisplay.GetComponent<RectTransform>().anchoredPosition.y);
            yield return null;
        }
        errorDisplayActive = false;
    }

    IEnumerator ErrorScreenActivate() {
        if (GetComponent<HelpScreen>().helpScreenActive) {
            GetComponent<HelpScreen>().Deactivate();
        }
        while (errorDisplay.GetComponent<RectTransform>().anchoredPosition.x > 0f) {
            var newX = errorDisplay.GetComponent<RectTransform>().anchoredPosition.x - (distanceDelta * Time.deltaTime);
            if (newX < 0f) {
                newX = 0f;
            }
            errorDisplay.GetComponent<RectTransform>().anchoredPosition = new Vector2(newX, errorDisplay.GetComponent<RectTransform>().anchoredPosition.y);
            yield return null;
        }
        errorDisplayActive = true;
    }

    public void ButtonInteractable(bool interactable) {
        errorsButton.interactable = interactable;
    }

    public void Deactivate() {
        StartCoroutine(ErrorScreenDeactivate());
    }

    public void ImmediateDeactivate() {
        errorDisplay.GetComponent<RectTransform>().anchoredPosition = new Vector2(960f, errorDisplay.GetComponent<RectTransform>().anchoredPosition.y);
    }
}
