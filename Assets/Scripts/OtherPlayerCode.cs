using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayerCode : NetworkBehaviour
{
    public GameObject playerCodeBlock, playerCodeScrollbar, otherCodeBlock;
    public RectTransform otherCodeRect;
    public float codeBlockHeightSingleplayer, codeBlockHeightMultiplayer, codeBlockYSingleplayer, codeBlockYMultiplayer;
    public float syncInterval;
    private float timer = 0f;
    private Button multiplayerButton;
    private bool isEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        otherCodeBlock.SetActive(false);
    }

    public void AssignButton(Button button) {
        multiplayerButton = button;
        multiplayerButton.onClick.AddListener(ActivateCodeBlock);

        if (NetworkHelper.Instance.IsPlayerOne) {
            multiplayerButton.gameObject.GetComponentInChildren<TMP_Text>().text = "2P CODE";
        }
        else {
            multiplayerButton.gameObject.GetComponentInChildren<TMP_Text>().text = "1P CODE";
        }
    }

    void ActivateCodeBlock() {
        if (!isEnabled) {
            otherCodeBlock.SetActive(true);

            playerCodeBlock.GetComponent<RectTransform>().sizeDelta = new Vector2(playerCodeBlock.GetComponent<RectTransform>().sizeDelta.x, codeBlockHeightMultiplayer);
            playerCodeScrollbar.GetComponent<RectTransform>().sizeDelta = new Vector2(playerCodeScrollbar.GetComponent<RectTransform>().sizeDelta.x, codeBlockHeightMultiplayer);

            playerCodeBlock.GetComponent<RectTransform>().anchoredPosition = new Vector3(playerCodeBlock.GetComponent<RectTransform>().anchoredPosition.x, codeBlockYMultiplayer);
            playerCodeScrollbar.GetComponent<RectTransform>().anchoredPosition = new Vector3(playerCodeScrollbar.GetComponent<RectTransform>().anchoredPosition.x, codeBlockYMultiplayer);

            isEnabled = true;
        }
        else {
            otherCodeBlock.SetActive(false);

            playerCodeBlock.GetComponent<RectTransform>().sizeDelta = new Vector2(playerCodeBlock.GetComponent<RectTransform>().sizeDelta.x, codeBlockHeightSingleplayer);
            playerCodeScrollbar.GetComponent<RectTransform>().sizeDelta = new Vector2(playerCodeScrollbar.GetComponent<RectTransform>().sizeDelta.x, codeBlockHeightSingleplayer);

            playerCodeBlock.GetComponent<RectTransform>().anchoredPosition = new Vector3(playerCodeBlock.GetComponent<RectTransform>().anchoredPosition.x, codeBlockYSingleplayer);
            playerCodeScrollbar.GetComponent<RectTransform>().anchoredPosition = new Vector3(playerCodeScrollbar.GetComponent<RectTransform>().anchoredPosition.x, codeBlockYSingleplayer);

            isEnabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkHelper.Instance.IsPlayerOne && NetworkHelper.Instance.IsMultiplayer && NetworkManager.Singleton.IsConnectedClient) { 
            // let's give SOMETHING to the client, ya know?
            timer += Time.deltaTime;
            if (timer >= syncInterval) {
                timer = 0f;

                P2SendCode();
            }
        }
    }

    void P2SendCode() {
        var p2Code = playerCodeBlock.GetComponent<TMP_InputField>().text; // shouldn't need to bother with clearing color formatting
        P1ReceiveAndSendCodeRpc(p2Code);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    void P1ReceiveAndSendCodeRpc(string p2Code) {
        otherCodeBlock.GetComponentInChildren<TMP_Text>().text = p2Code;
        LayoutRebuilder.ForceRebuildLayoutImmediate(otherCodeRect);

        var p1Code = playerCodeBlock.GetComponent<TMP_InputField>().text;

        P2ReceiveCodeRpc(p1Code);
    }

    [Rpc(SendTo.NotServer, RequireOwnership = false)]
    void P2ReceiveCodeRpc(string p1Code) {
        otherCodeBlock.GetComponentInChildren<TMP_Text>().text = p1Code;
        LayoutRebuilder.ForceRebuildLayoutImmediate(otherCodeRect);
    }

    public void ButtonInteractable(bool interactable) {
        multiplayerButton.interactable = interactable;
    }

    public string GetText() {
        return otherCodeBlock.GetComponentInChildren<TMP_Text>().text;
    }
}
