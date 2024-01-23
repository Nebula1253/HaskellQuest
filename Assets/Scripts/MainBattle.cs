using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBattle : MonoBehaviour
{
    public int initXPos, turnXPos;
    public GameObject hack, items;
    private Button hackButton, itemsButton;

    // Start is called before the first frame update
    void Start()
    {
        hackButton = hack.GetComponent<Button>();
        itemsButton = items.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator moveToCentre() {
        while (GetComponent<RectTransform>().anchoredPosition.x <= initXPos) {
            GetComponent<RectTransform>().anchoredPosition += new Vector2(1, 0) * 5;
            yield return null;
        }
        hackButton.interactable = true;
        itemsButton.interactable = true;
    }

    IEnumerator moveToTurnPos() {
        hackButton.interactable = false;
        itemsButton.interactable = false;

        while (GetComponent<RectTransform>().anchoredPosition.x >= turnXPos) {
            GetComponent<RectTransform>().anchoredPosition -= new Vector2(1, 0) * 5;
            yield return null;
        }
    }

    public void moveToCentreCall() {
        StartCoroutine(moveToCentre());
    }

    public void moveToTurnPosCall() {
        StartCoroutine(moveToTurnPos());
    }
}
