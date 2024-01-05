using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBattle : MonoBehaviour
{
    public int initXPos, turnXPos;
    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    IEnumerator moveToTurnPos() {
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
