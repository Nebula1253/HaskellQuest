using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerHUD : NetworkBehaviour
{
    public int UIinitXPos, UIturnXPos;
    private float spriteInitXPos, spriteTurnXPos;
    public float time;
    private float uiDistanceDelta, spriteDistanceDelta;
    private GameObject hack, enemyView;
    private Button hackButton;
    private CodeEditor editor;

    public static PlayerHUD Instance { get; private set; }

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    

    // Start is called before the first frame update
    void Start()
    {
        hack = transform.Find("HackButton").gameObject;
        enemyView = GameObject.Find("EnemyView");

        editor = GameObject.FindGameObjectWithTag("CodeEditor").GetComponent<CodeEditor>();
        // editor = CodeEditor.Instance;

        hackButton = hack.GetComponent<Button>();

        hackButton.onClick.AddListener(PlayerActionHackRpc);

        uiDistanceDelta = Mathf.Abs(UIinitXPos - UIturnXPos) / time;
        spriteInitXPos = 0f;
        spriteTurnXPos = -8.88889f / 2;

        // Debug.Log("UIinitXPos: " + UIinitXPos + " UIturnXPos: " + UIturnXPos);
        // Debug.Log("spriteInitXPos: " + spriteInitXPos + " spriteTurnXPos: " + spriteTurnXPos);



        spriteDistanceDelta = Mathf.Abs(spriteInitXPos - spriteTurnXPos) / time;
    }

    IEnumerator moveToCentre(bool wonBattle, bool gameOver) {
        if (gameOver) {
            hackButton.gameObject.SetActive(false);
        }

        while (GetComponent<RectTransform>().anchoredPosition.x < UIinitXPos) {
            var newX = Mathf.Min(UIinitXPos, GetComponent<RectTransform>().anchoredPosition.x + uiDistanceDelta * Time.deltaTime);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(newX, GetComponent<RectTransform>().anchoredPosition.y);
            yield return null;
        }
        
        if (!gameOver & !wonBattle) {
            hackButton.interactable = true;
        }

    }

    IEnumerator moveSpriteToCentre(bool wonBattle = false) {
        while (enemyView.transform.position.x < spriteInitXPos) {
            var newX = Mathf.Min(spriteInitXPos, enemyView.transform.position.x + spriteDistanceDelta * Time.deltaTime);
            enemyView.transform.position = new Vector3(newX, enemyView.transform.position.y, enemyView.transform.position.z);
            yield return null;
        }

        Debug.Log("wonBattle: " + wonBattle);

        if (wonBattle) {
            enemyView.GetComponent<EnemyController>().BattleEnd();
        }
    }

    IEnumerator moveToTurnPos() {
        hackButton.interactable = false;
        // dialog.SetActive(false);

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

    public void moveToCentreCall(bool wonBattle = false, bool gameOver = false) {
        StartCoroutine(moveToCentre(wonBattle, gameOver));
        StartCoroutine(moveSpriteToCentre(wonBattle));
    }

    [Rpc(SendTo.Everyone)]
    private void PlayerActionHackRpc() {
        StartCoroutine(moveToTurnPos());
        StartCoroutine(moveSpriteToTurnPos());
        editor.ActivateEditor();
    }
}
