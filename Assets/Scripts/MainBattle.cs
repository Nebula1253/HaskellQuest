using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBattle : MonoBehaviour
{
    public int UIinitXPos, UIturnXPos;
    private float spriteInitXPos, spriteTurnXPos;
    public float time;
    private float uiDistanceDelta, spriteDistanceDelta;
    private GameObject hack, items, dialog, enemyView;
    private Button hackButton, itemsButton;
    private CodeEditor editor;

    // Start is called before the first frame update
    void Start()
    {
        hack = transform.Find("HackButton").gameObject;
        items = transform.Find("ItemButton").gameObject;
        dialog = transform.Find("DialogBox").gameObject;
        enemyView = GameObject.Find("EnemyView");

        editor = GameObject.FindGameObjectWithTag("CodeEditor").GetComponent<CodeEditor>();

        hackButton = hack.GetComponent<Button>();
        itemsButton = items.GetComponent<Button>();

        hackButton.onClick.AddListener(PlayerActionHack);

        uiDistanceDelta = Mathf.Abs(UIinitXPos - UIturnXPos) / time;
        spriteInitXPos = 0f;
        spriteTurnXPos = -8.88889f / 2;

        Debug.Log("UIinitXPos: " + UIinitXPos + " UIturnXPos: " + UIturnXPos);
        Debug.Log("spriteInitXPos: " + spriteInitXPos + " spriteTurnXPos: " + spriteTurnXPos);

        spriteDistanceDelta = Mathf.Abs(spriteInitXPos - spriteTurnXPos) / time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator moveToCentre(bool wonBattle = false) {
        while (GetComponent<RectTransform>().anchoredPosition.x < UIinitXPos) {
            var newX = Mathf.Min(UIinitXPos, GetComponent<RectTransform>().anchoredPosition.x + uiDistanceDelta * Time.deltaTime);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(newX, GetComponent<RectTransform>().anchoredPosition.y);
            yield return null;
        }
        
        if (!wonBattle) {
            hackButton.interactable = true;
            itemsButton.interactable = true;
            dialog.SetActive(true);
        }
    }

    IEnumerator moveSpriteToCentre(bool wonBattle = false) {
        while (enemyView.transform.position.x < spriteInitXPos) {
            var newX = Mathf.Min(spriteInitXPos, enemyView.transform.position.x + spriteDistanceDelta * Time.deltaTime);
            enemyView.transform.position = new Vector3(newX, enemyView.transform.position.y, enemyView.transform.position.z);
            yield return null;
        }

        if (wonBattle) {
            enemyView.GetComponent<EnemyController>().Esplode();
        }
    }

    IEnumerator moveToTurnPos() {
        hackButton.interactable = false;
        itemsButton.interactable = false;
        dialog.SetActive(false);

        while (GetComponent<RectTransform>().anchoredPosition.x > UIturnXPos) {
            var newX = Mathf.Max(UIturnXPos, GetComponent<RectTransform>().anchoredPosition.x - uiDistanceDelta * Time.deltaTime);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(newX, GetComponent<RectTransform>().anchoredPosition.y);
            yield return null;
        }
        
    }

    IEnumerator moveSpriteToTurnPos() {
        while (enemyView.transform.position.x > spriteTurnXPos) {
            var newX = Mathf.Max(spriteTurnXPos, enemyView.transform.position.x - spriteDistanceDelta * Time.deltaTime);
            enemyView.transform.position = new Vector3(newX, enemyView.transform.position.y, enemyView.transform.position.z);
            yield return null;
        }
    }

    public void moveToCentreCall(bool wonBattle = false) {
        StartCoroutine(moveToCentre(wonBattle));
        StartCoroutine(moveSpriteToCentre());
    }

    private void PlayerActionHack() {
        StartCoroutine(moveToTurnPos());
        StartCoroutine(moveSpriteToTurnPos());
        editor.ActivateEditor();
    }
}
