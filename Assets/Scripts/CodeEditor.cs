using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using JDoodle;
using UnityEngine.Networking;

public class CodeEditor : NetworkBehaviour
{
    const int CREDIT_LIMIT = 200;
    const string TEST_CODE_MARKER = "-- TEST CODE";
    const string SINGLEPLAYER_CODE_MARKER = "-- PLAYER CODE";
    const string MULTIPLAYER_P1_CODE_MARKER = "-- PLAYER 1 CODE";
    const string MULTIPLAYER_P2_CODE_MARKER = "-- PLAYER 2 CODE";
    private TMP_InputField codeField;
    private TMP_Text errorDisplay;
    public TMP_Text nonEditableCode;
    public RectTransform nonEditableRectTransform;

    private Button submitButton, resetButton;
    public TextAsset[] singleplayerCodeFiles;
    public TextAsset[] multiplayerCodeFiles;
    private TextAsset[] codeFiles;
    private AttackController[] controllers;

    private string challengeCode, playerEditableCode;
    private string testCode;
    private bool interactable = true;
    public GameObject battlefield, overheadEnemy1P, overheadEnemy2P;
    private GameObject filename;
    private PlayerHUD playerHUDScript;
    public Color commentColor;
    public bool commentHighlighting;
    private string colorCode;
    private float codeEditorXPos;
    public int currentScript = 0;

    public GameObject statusDisplayObj;
    public float time;
    private float distanceDelta;

    private PlayerState playerState;
    private bool gameOver = false;
    private EnemyController enemyController;
    private int cachedCaretPosition = -1;
    public string errorPlaceholderText;
    private int disableInteractAcksReceived = 0;
    public GameObject singleplayerButtons, multiplayerButtons;
    public bool useLocalCompiler = false;
    private PauseScreen pauseScreen;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        codeField = GetComponentInChildren<TMP_InputField>();
        errorDisplay = statusDisplayObj.GetComponentInChildren<TMP_Text>();
        errorDisplay.text = errorPlaceholderText;

        if (NetworkHelper.Instance.IsMultiplayer) {
            singleplayerButtons.SetActive(false);
            multiplayerButtons.SetActive(true);

            AssignButtons(multiplayerButtons);

            codeFiles = multiplayerCodeFiles;

            controllers = overheadEnemy2P.GetComponents<AttackController>();
        }
        else {
            singleplayerButtons.SetActive(true);
            multiplayerButtons.SetActive(false);

            AssignButtons(singleplayerButtons);

            codeFiles = singleplayerCodeFiles;

            controllers = overheadEnemy1P.GetComponents<AttackController>();
        }

        colorCode = "#" + ColorUtility.ToHtmlStringRGB(commentColor);
        
        ChangeScript();

        codeEditorXPos = -960;

        DisableInteract();

        playerHUDScript = PlayerHUD.Instance;

        filename = GameObject.Find("Filename");

        distanceDelta = Mathf.Abs(codeEditorXPos) / time;

        if (commentHighlighting) {
            EditableCodeHighlighting();
            codeField.onValueChanged.AddListener(delegate {EditableCodeHighlighting(); });
        }

        filename.GetComponent<TMP_Text>().text = codeFiles[currentScript].name + ".hs";

        playerState = PlayerState.Instance;

        enemyController = EnemyController.Instance;

        enemyController.PhaseTransition(currentScript);

        pauseScreen = GameObject.Find("PauseOverlay").GetComponent<PauseScreen>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable) {
            if (Input.GetKeyDown(KeyCode.Escape) && !pauseScreen.canBePaused) {
                BackRpc();
            }
        }

        if (cachedCaretPosition != -1) {
            codeField.caretPosition = cachedCaretPosition;
            cachedCaretPosition = -1;
        }
    }

    void AssignButtons(GameObject buttons) {
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            GameObject button = buttons.transform.GetChild(i).gameObject;
            if (button.CompareTag("Submit")) {
                submitButton = button.GetComponent<Button>();
                submitButton.onClick.AddListener(Submit);
            }
            else if (button.CompareTag("Reset")) {
                resetButton = button.GetComponent<Button>();
                resetButton.onClick.AddListener(Reset);
            }
            else if (button.CompareTag("Help")) {
                GetComponent<HelpScreen>().AssignButton(button.GetComponent<Button>());
            }
            else if (button.CompareTag("Error")) {
                GetComponent<ErrorScreen>().AssignButton(button.GetComponent<Button>());
            }
            else if (button.CompareTag("Multiplayer")) {
                GetComponent<OtherPlayerCode>().AssignButton(button.GetComponent<Button>());
            }
        }
    }

    void ChangeScript() {
        Debug.Log(codeFiles);
        var testCodeDivvy = codeFiles[currentScript].text.Split(TEST_CODE_MARKER);
        Debug.Log(testCodeDivvy[0]);

        challengeCode = testCodeDivvy[0];
        testCode = testCodeDivvy[1];

        if (NetworkHelper.Instance.IsMultiplayer) {
            var playerCodeDivvy = challengeCode.Split(MULTIPLAYER_P1_CODE_MARKER);

            string neCodeCommented = CommentHighlighting(playerCodeDivvy[0].Trim());
            if (neCodeCommented != null) {
                nonEditableCode.text = neCodeCommented;
            }
            else {
                nonEditableCode.text = playerCodeDivvy[0].Trim();
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(nonEditableRectTransform);
            var p1p2Code = playerCodeDivvy[1].Split(MULTIPLAYER_P2_CODE_MARKER);

            var p1Code = p1p2Code[0].Trim();
            var p2Code = p1p2Code[1].Trim();

            if (NetworkHelper.Instance.IsPlayerOne) {
                codeField.text = p1Code;
                playerEditableCode = p1Code;
            }
            else {
                codeField.text = p2Code;
                playerEditableCode = p2Code;
            }
        }
        else {
            var playerCodeDivvy = challengeCode.Split(SINGLEPLAYER_CODE_MARKER);

            string neCodeCommented = CommentHighlighting(playerCodeDivvy[0]);
            if (neCodeCommented != null) {
                nonEditableCode.text = neCodeCommented;
            }
            else {
                nonEditableCode.text = playerCodeDivvy[0].Trim();
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(nonEditableRectTransform);
            codeField.text = playerCodeDivvy[1].Trim();
            playerEditableCode = playerCodeDivvy[1].Trim();
        }
    }

    void EditableCodeHighlighting() {
        string newCode = CommentHighlighting(codeField.text);

        if (newCode != null) {
            cachedCaretPosition = codeField.caretPosition;
            codeField.text = newCode;

            // don't know why this would work, but let's hope it somehow does??
            codeField.caretPosition = cachedCaretPosition;
        }
    }

    [Rpc(SendTo.Everyone)]
    void BackRpc() {
        DisableInteractRpc(NetworkManager.Singleton.LocalClientId);
        StartCoroutine(MoveOffScreen(false));
    }

    private string CommentHighlighting(string code) {
        bool commentChanged = false;
        var codeLines = code.Split('\n');
        string newCode = "";

        string openTag = "<color=" + colorCode + ">";
        string closeTag = "</color>";

        // Debug.Log(codeLines.Length);

        for (int i = 0; i < codeLines.Length; i++) {
            string line = codeLines[i];

            if (line.Contains("--") && !line.Contains(openTag)) {
                var regex = new Regex("--");
                line = regex.Replace(line, openTag + "--", 1);

                // line = line.Replace("--", "<color=" + colorCode + ">--");
                line += closeTag;
                commentChanged = true;
            }
            else if ((!line.Contains("--") && line.Contains(openTag)) || // highlighted, but not a comment
                    (line.Contains(openTag) && !line.Contains(closeTag)) || // has opening tag but not the closing tag
                    (!line.Contains(openTag) && line.Contains(closeTag))) { // has closing tag but not the opening tag
                    
                line = line.Replace(openTag, "").Replace(closeTag, "");
                commentChanged = true;
            }
            else if (line.Contains(openTag + "--") && line.Contains(closeTag)
                    && line.Substring(Math.Max(0, line.Length - closeTag.Length)) != closeTag) // line is a comment and has tags, but the closing tag isn't at the end of the line
            {
                line = line.Replace(closeTag, "");
                line += closeTag;
                commentChanged = true;
            }

            newCode += line;
            if (i != codeLines.Length - 1) {
                newCode += "\n";
            }
        }
        
        if (commentChanged) {
            return newCode;
        }
        else {
            return null;
        }
    }

    void Reset() {
        codeField.text = playerEditableCode;
    }

    void Submit() {
        StartCoroutine(SubmitCoroutine());
    }

    IEnumerator SubmitCoroutine() {
        DisableInteractRpc(NetworkManager.Singleton.LocalClientId, true);
        
        // NEED to make sure UI is disabled on all game instances before call to JDoodle is made
        // therefore, we have this fucking bullshit, because Unity Netcode does not make RPCs awaitable

        var desiredNrAcks = NetworkHelper.Instance.IsMultiplayer ? 2 : 1;
        Debug.Log(desiredNrAcks);
        while (disableInteractAcksReceived != desiredNrAcks) {
            yield return null;
        }
        disableInteractAcksReceived = 0;

        StartCoroutine(SubmitToJDoodle());
    }

    // functionality separated purely for the call in Start to make sense
    private void DisableInteract() {
        interactable = false;
        codeField.interactable = false;
        // codeField.transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;

        submitButton.interactable = false;
        resetButton.interactable = false;

        GetComponent<HelpScreen>().ButtonInteractable(false);
        GetComponent<ErrorScreen>().ButtonInteractable(false);

        if (NetworkHelper.Instance.IsMultiplayer) {
            GetComponent<OtherPlayerCode>().ButtonInteractable(false);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void DisableInteractRpc(ulong callingClientID, bool isSubmittingCode = false) {
        DisableInteract();

        if (isSubmittingCode) {
            AcknowledgeDisableInteractRpc(callingClientID);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void AcknowledgeDisableInteractRpc(ulong originalClientID) {
        if (NetworkManager.Singleton.LocalClientId == originalClientID) {
            Debug.Log("ACK RECEIVED");
            disableInteractAcksReceived += 1;
        } 
    }

    [Rpc(SendTo.Everyone)]
    private void EnableInteractRpc() {
        codeField.interactable = true;
        interactable = true;
        codeField.transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.white;

        submitButton.interactable = true;
        resetButton.interactable = true;
        
        GetComponent<HelpScreen>().ButtonInteractable(true);
        GetComponent<ErrorScreen>().ButtonInteractable(true);

        if (NetworkHelper.Instance.IsMultiplayer) {
            GetComponent<OtherPlayerCode>().ButtonInteractable(true);
        }
    }

    IEnumerator MoveOffScreen(bool changeScript) {
        if (GetComponent<HelpScreen>().helpScreenActive) {
            GetComponent<HelpScreen>().Deactivate();
        }
        if (GetComponent<ErrorScreen>().errorDisplayActive) {
            GetComponent<ErrorScreen>().Deactivate();
        }
        battlefield.GetComponent<BattleField>().DeactivateBattlefield();

        bool wonBattle = false;
        if (changeScript) {
           wonBattle = (currentScript + 1) >= codeFiles.Length; 
        }
        if (!gameOver) {
            playerHUDScript.moveToCentreCall(wonBattle);
        }
        
        while (GetComponent<RectTransform>().anchoredPosition.x < 0) {
            var newX = Mathf.Min(0, GetComponent<RectTransform>().anchoredPosition.x + distanceDelta * Time.deltaTime);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(newX, GetComponent<RectTransform>().anchoredPosition.y);
            yield return null;
        }

        if (wonBattle) {
            playerState.LoadNextScene();
        }
        else if (changeScript) {
            currentScript++;
            
            ChangeScript();
            errorDisplay.text = errorPlaceholderText;

            filename.GetComponent<TMP_Text>().text = codeFiles[currentScript].name + ".hs";
            enemyController.PhaseTransition(currentScript);
        }
        pauseScreen.canBePaused = true;
    }

    [Rpc(SendTo.Everyone)]
    private void EndOfEnemyMoveRpc(bool phaseOver) {
        if (!gameOver) {
            StartCoroutine(MoveOffScreen(phaseOver));
        }
    }

    IEnumerator EnemyMove(bool phaseOver) {
        // DisableEditorRpc();

        while (!controllers[currentScript].AttackEnd()) {
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1);

        EndOfEnemyMoveRpc(phaseOver);
    }

    public void MoveOffScreenGameOver() {
        gameOver = true;
        controllers[currentScript].StopAllCoroutines();
        StartCoroutine(MoveOffScreen(false));
    }

    public void ActivateEditor() {
        StartCoroutine(MoveOnScreen());
    }

    [Rpc(SendTo.Everyone)]
    private void EnemyMoveTriggerRpc(bool result, string additionalConditions) {
        if (GetComponent<HelpScreen>().helpScreenActive) {
            GetComponent<HelpScreen>().ImmediateDeactivate();
        }
        if (GetComponent<ErrorScreen>().errorDisplayActive) {
            GetComponent<ErrorScreen>().ImmediateDeactivate();
        }
        battlefield.GetComponent<BattleField>().ActivateBattlefield();
        
        // needs to wait for players to have finished spawning before attack coroutine is triggered

        while (!battlefield.GetComponent<BattleField>().IsActive) {
            // do ABSOLUTELY NOTHING
        }
        
        if (NetworkHelper.Instance.IsPlayerOne) {

            if (additionalConditions == "") {
                controllers[currentScript].Trigger(result);
            }
            else {
                controllers[currentScript].Trigger(result, additionalConditions);
            }

            StartCoroutine(EnemyMove(result));
        }
        
    }

    IEnumerator MoveOnScreen() {
        Debug.Log("moving on screen");
        pauseScreen.canBePaused = false;
        while (GetComponent<RectTransform>().anchoredPosition.x > codeEditorXPos) {
            var newX = GetComponent<RectTransform>().anchoredPosition.x - distanceDelta * Time.deltaTime;
            if (newX < codeEditorXPos) {
                newX = codeEditorXPos;
            }
            GetComponent<RectTransform>().anchoredPosition = new Vector2(newX, GetComponent<RectTransform>().anchoredPosition.y);
            yield return null;
        }

        EnableInteractRpc();
    }
    
    public string CleanColorFormatting(string code) {
        return code.Replace("<color=" + colorCode + ">", "").Replace("</color>", "");
    }

    [Rpc(SendTo.Everyone)]
    private void AddErrorToDisplayRpc(string error) {
        // if (errorDisplay.text == errorPlaceholderText) {
        //     errorDisplay.text = "";
        // }
        errorDisplay.text = error;
    }

    private void EvaluateResult(string output) {
        string additional = "";
        Debug.Log(output);
        if (output != null && output.Contains("Additional: ")) {
            additional = output.Split('\n')[3].Remove(0,13);
            additional = additional.Remove(additional.Length - 1, 1);
        }
        
        if (output.Contains("Timeout") || string.IsNullOrEmpty(output) || output.Equals("[1 of 1] Compiling Main             ( jdoodle.hs, jdoodle.o )\n")) {
            if (controllers[currentScript].IsRecursionHandled()) {

                AddErrorToDisplayRpc("<color=#ff0000>Your code recurses infinitely!</color>\n");

                EnemyMoveTriggerRpc(false, additional);
            }
            else {
                // ask player to "try again - see if they've got an infinite loop": do nothing other than that, because 
                // this could just be JDoodle screwing around
                
                AddErrorToDisplayRpc("<color=#ffff00>Your code timed out! Try again - see if you've got an infinite loop.</color>\n");

                //EnableEditor here
                EnableInteractRpc();
            }
        }
        else if (output.Contains("error") || output.Contains("Non-exhaustive")) {
            var outputSplit = output.Split('\n');
            string errMsg = "";

            foreach (string line in outputSplit) {
                string trimLine = line.Trim();
                if (!trimLine.Contains("Additional: ") && 
                    !line.Equals("[1 of 1] Compiling Main             ( jdoodle.hs, jdoodle.o )") &&
                    !line.Equals("Linking jdoodle ...") &&
                    !line.Equals(""))
                {
                    errMsg += line + '\n';
                } 
            }

            errMsg = errMsg.Replace("\"", "");

            AddErrorToDisplayRpc("<color=#ff0000>" + errMsg + "</color>\n");
            EnemyMoveTriggerRpc(false, additional);
        }
        else {
            // trigger enemy fire depending on whether the code passed the test
            string result = output.Split('\n')[2].Replace("\"", "");
            Debug.Log(result);

            if (result == "True") {
                AddErrorToDisplayRpc("<color=#00ff00>Test passed!</color>\n");
            }
            else if (result == "False") {
                AddErrorToDisplayRpc("<color=#ff0000>Test failed!</color>\n");
            }
            // else if (result.Contains("error")) {
            //     AddErrorToDisplayRpc("<color=#ff0000>" + result + "</color>\n");
            // }

            EnemyMoveTriggerRpc(result == "True", additional);
        }
    }

    IEnumerator SubmitToJDoodle() {
        string script = "{-# LANGUAGE ParallelListComp #-}\n" + CleanColorFormatting(nonEditableCode.text);
        if (NetworkHelper.Instance.IsMultiplayer) {
            script += CleanColorFormatting(codeField.text) + CleanColorFormatting(GetComponent<OtherPlayerCode>().GetText()) + testCode;
        }
        else {
            script += CleanColorFormatting(codeField.text) + testCode;
        }

        string executeURL = useLocalCompiler ? "https://oxrush.tardis.ac/haskellquest/api/execute" : "https://api.jdoodle.com/v1/execute";
        string creditsURL = "https://api.jdoodle.com/v1/credit-spent";

        if (!useLocalCompiler) {
            JDoodleCreditsRequest creditsRequest = new JDoodleCreditsRequest();
            string creditsInput = JsonUtility.ToJson(creditsRequest);

            using (UnityWebRequest creditsWebRequest = UnityWebRequest.Post(creditsURL, creditsInput, "application/json")) {
                yield return creditsWebRequest.SendWebRequest();

                if (creditsWebRequest.result != UnityWebRequest.Result.Success) {
                    AddErrorToDisplayRpc("<color=#ffff00>" + creditsWebRequest.error + "</color>\n");
                    EnableInteractRpc();
                    yield break;
                }
                Debug.Log(creditsWebRequest.downloadHandler.text);
                JDoodleCreditsResponse creditsResponse = JsonUtility.FromJson<JDoodleCreditsResponse>(creditsWebRequest.downloadHandler.text);

                if (creditsResponse.error != null) {
                    AddErrorToDisplayRpc("<color=#ffff00>" + creditsResponse.error + "</color>\n");
                    EnableInteractRpc();
                    yield break;
                }
                else if (creditsResponse.used > CREDIT_LIMIT) {
                    AddErrorToDisplayRpc("<color=#ffff00>There are no more compiler credits remaining today - please try again tomorrow.</color>\n");
                    EnableInteractRpc();
                    yield break;
                }
            }
        }

        JDoodleRequest inputRequest = new JDoodleRequest(script);
        string input = JsonUtility.ToJson(inputRequest);

        using (UnityWebRequest executeWebRequest = UnityWebRequest.Post(executeURL, input, "application/json")) {
            yield return executeWebRequest.SendWebRequest();

            if (executeWebRequest.result != UnityWebRequest.Result.Success) {
                AddErrorToDisplayRpc("<color=#ffff00>" + executeWebRequest.error + "</color>\n");
                EnableInteractRpc();
                yield break;         
            }

            Debug.Log(executeWebRequest.downloadHandler.text);
            JDoodleResponse outputResponse = JsonUtility.FromJson<JDoodleResponse>(executeWebRequest.downloadHandler.text);

            EvaluateResult(outputResponse.output);
        }
    } 
}
