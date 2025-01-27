using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;
using TMPro;
using System.Runtime.ConstrainedExecution;
using System.Data.Common;
using Unity.Netcode;
using JDoodle;

public class CodeEditor : NetworkBehaviour
{
    const int CREDIT_LIMIT = 200;
    const string TEST_CODE_MARKER = "-- TEST CODE";
    const string PLAYER_CODE_MARKER = "-- PLAYER CODE";
    private TMP_InputField codeField;
    private TMP_Text errorDisplay;
    public TMP_Text nonEditableCode;

    private Button submitButton, resetButton;
    public TextAsset[] codeFiles;
    private AttackController[] controllers;

    private string challengeCode;
    private string testCode;
    private bool interactable = true;
    public GameObject battlefield, overheadEnemy;
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

    // public static CodeEditor Instance { get; private set; }
    // private void Awake() 
    // { 
    //     // If there is an instance, and it's not me, delete myself.
        
    //     if (Instance != null && Instance != this) 
    //     { 
    //         Destroy(this); 
    //     } 
    //     else 
    //     { 
    //         Instance = this; 
    //     } 
    // }

    // Start is called before the first frame update
    void Start()
    {
        codeField = GetComponentInChildren<TMP_InputField>();
        errorDisplay = statusDisplayObj.GetComponentInChildren<TMP_Text>();
        errorDisplay.text = errorPlaceholderText;
        
        submitButton = GameObject.FindGameObjectWithTag("Submit").GetComponent<Button>();
        submitButton.onClick.AddListener(Submit);

        resetButton = GameObject.FindGameObjectWithTag("Reset").GetComponent<Button>();
        resetButton.onClick.AddListener(Reset);

        colorCode = "#" + ColorUtility.ToHtmlStringRGB(commentColor);
        ChangeScript();

        codeEditorXPos = -960;

        DisableEditor();

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

        controllers = overheadEnemy.GetComponents<AttackController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                BackRpc();
            }
        }

        if (cachedCaretPosition != -1) {
            codeField.caretPosition = cachedCaretPosition;
            cachedCaretPosition = -1;
        }
    }

    void ChangeScript() {
        var testCodeDivvy = codeFiles[currentScript].text.Split(TEST_CODE_MARKER, StringSplitOptions.None);
        Debug.Log(testCodeDivvy[0]);

        challengeCode = testCodeDivvy[0];
        testCode = testCodeDivvy[1];

        var playerCodeDivvy = challengeCode.Split(PLAYER_CODE_MARKER, StringSplitOptions.None);

        string neCodeCommented = CommentHighlighting(playerCodeDivvy[0]);
        if (neCodeCommented != null) {
            nonEditableCode.text = neCodeCommented;
        }
        else {
            nonEditableCode.text = playerCodeDivvy[0];
        }
        codeField.text = playerCodeDivvy[1];
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
        DisableEditor();
        StartCoroutine(MoveOffScreen(false));
    }

    private string CommentHighlighting(string code) {
        bool commentChanged = false;
        var codeLines = code.Split('\n');
        string newCode = "";

        Debug.Log(codeLines.Length);

        for (int i = 0; i < codeLines.Length; i++) {
            string line = codeLines[i];

            if (line.Contains("--") && !line.Contains("<color=" + colorCode + ">")) {
                var regex = new Regex("--");
                line = regex.Replace(line, "<color=" + colorCode + ">--", 1);

                // line = line.Replace("--", "<color=" + colorCode + ">--");
                line += "</color>";
                commentChanged = true;
            }
            else if ((!line.Contains("--") && line.Contains("<color=" + colorCode + ">")) ||
                    (line.Contains("<color=" + colorCode + ">") && !line.Contains("</color>")) ||
                    (!line.Contains("<color=" + colorCode + ">") && line.Contains("</color>"))) {
                line = line.Replace("<color=" + colorCode + ">", "");
                line = line.Replace("</color>", "");
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

    void Submit() {
        CallJDoodle();
    }

    void Reset() {
        codeField.text = challengeCode;
    }

    private void DisableEditor() {
        interactable = false;
        codeField.interactable = false;
        codeField.transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;

        submitButton.interactable = false;
        resetButton.interactable = false;

        GetComponent<HelpScreen>().ButtonInteractable(false);
        GetComponent<ErrorScreen>().ButtonInteractable(false);
    }

    IEnumerator MoveOffScreen(bool changeScript) {
        if (GetComponent<HelpScreen>().helpScreenActive) {
            GetComponent<HelpScreen>().Deactivate();
        }
        if (GetComponent<ErrorScreen>().errorDisplayActive) {
            GetComponent<ErrorScreen>().Deactivate();
        }
        battlefield.SetActive(false);
        // statusDisplay.text = "";

        bool wonBattle = false;
        if (changeScript) {
           wonBattle = (currentScript + 1) >= codeFiles.Length; 
        }
        if (!gameOver) {
            playerHUDScript.moveToCentreCall(wonBattle);
        }
        
        // TODO replace this with Unity's own UI animation option
        while (GetComponent<RectTransform>().anchoredPosition.x < 0) {
            var newX = Mathf.Min(0, GetComponent<RectTransform>().anchoredPosition.x + distanceDelta * Time.deltaTime);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(newX, GetComponent<RectTransform>().anchoredPosition.y);
            yield return null;
        }

        if (wonBattle) {
            playerState.DisplayScore();
        }
        else if (changeScript) {
            currentScript++;
            
            ChangeScript();

            GetComponent<ScriptSync>().ChangeText();
            
            filename.GetComponent<TMP_Text>().text = codeFiles[currentScript].name + ".hs";
            enemyController.PhaseTransition(currentScript);
        }
    }

    IEnumerator EnemyMove(bool phaseOver) {
        DisableEditor();

        while (!controllers[currentScript].AttackEnd()) {
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1);

        if (!gameOver) {
            StartCoroutine(MoveOffScreen(phaseOver));
        }
    }

    public void MoveOffScreenGameOver() {
        gameOver = true;
        StartCoroutine(MoveOffScreen(false));
    }

    public void ActivateEditor() {
        StartCoroutine(MoveOnScreen());
    }

    private void EnemyMoveTrigger(bool result, string additionalConditions) {
        // battlefield.SetActive(true);
        battlefield.GetComponent<BattleField>().ActivateBattlefield();

        if (GetComponent<HelpScreen>().helpScreenActive) {
            GetComponent<HelpScreen>().ImmediateDeactivate();
        }
        if (GetComponent<ErrorScreen>().errorDisplayActive) {
            GetComponent<ErrorScreen>().ImmediateDeactivate();
        }

        if (IsServer) {
            if (additionalConditions == "") {
                controllers[currentScript].Trigger(result);
            }
            else {
                controllers[currentScript].Trigger(result, additionalConditions);
            }
        }
        

        StartCoroutine(EnemyMove(result));
    }

    IEnumerator MoveOnScreen() {
        Debug.Log("moving on screen");
        while (GetComponent<RectTransform>().anchoredPosition.x > codeEditorXPos) {
            var newX = GetComponent<RectTransform>().anchoredPosition.x - distanceDelta * Time.deltaTime;
            if (newX < codeEditorXPos) {
                newX = codeEditorXPos;
            }
            GetComponent<RectTransform>().anchoredPosition = new Vector2(newX, GetComponent<RectTransform>().anchoredPosition.y);
            yield return null;
        }

        codeField.interactable = true;
        interactable = true;
        codeField.transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.white;

        submitButton.interactable = true;
        resetButton.interactable = true;
        
        GetComponent<HelpScreen>().ButtonInteractable(true);
        GetComponent<ErrorScreen>().ButtonInteractable(true);
    }
    
    public string CleanColorFormatting(string code) {
        return code.Replace("<color=" + colorCode + ">", "").Replace("</color>", "");
    }

    private void EvaluateResult(string output) {
        string additional = "";
        if (output.Contains("Additional: ")) {
            additional = output.Split('\n')[3].Remove(0,13);
            additional = additional.Remove(additional.Length - 1, 1);
        }

        if (errorDisplay.text == errorPlaceholderText) {
            errorDisplay.text = "";
        }
        
        if (output.Contains("Timeout") || output == null) {
            if (controllers[currentScript].IsRecursionHandled()) {
                // statusDisplay.color = Color.red;
                errorDisplay.text += "<color=#ff0000>Your code recurses infinitely!</color>\n";

                playerState.CodePenalty();
                EnemyMoveTrigger(false, additional);
            }
            else {
                // // ask player to "try again - see if they've got an infinite loop": do nothing other than that, because 
                // // this could just be JDoodle screwing around
                // statusDisplay.color = Color.yellow;
                errorDisplay.text += "<color=#ffff00>Your code timed out! Try again - see if you've got an infinite loop.</color>\n";
            }
        }
        else if (output.Split('\n').Length >= 4 && output.Split('\n')[3].Contains("error")) {
            // put error details on screen, trigger enemy fire
            
            var outputSplit = output.Split('\n');
            var error = "DUMMY: SHOULD NEVER BE DISPLAYED";

            if (outputSplit[3].Contains("jdoodle.hs")) { // JDoodle errors
                error = outputSplit[4];
            }
            else { // custom error messages
                error = outputSplit[3].Replace("\"", "");
            }
            
            // statusDisplay.color = Color.red;
            errorDisplay.text += "<color=#ff0000>" + error + "</color>\n";
            playerState.CodePenalty();

            EnemyMoveTrigger(false, additional);
        }
        else if (output.Split('\n')[2].Contains("Non-exhaustive")) {
            var errorStart = output.Split('\n')[2].IndexOf("Non-exhaustive");
            var error = output.Split('\n')[2].Substring(errorStart);
            // statusDisplay.color = Color.red;
            errorDisplay.text += "<color=#ff0000>" + error + "</color>\n";
            playerState.CodePenalty();

            EnemyMoveTrigger(false, additional);
        }
        else {
            // trigger enemy fire depending on whether the code passed the test
            string result = output.Split('\n')[2].Replace("\"", "");
            Debug.Log(result);
            bool resultBool = result == "True";
            if (resultBool) {
                // statusDisplay.color = Color.green;
                errorDisplay.text += "<color=#00ff00>Test passed!</color>\n";
            }
            else {
                // statusDisplay.color = Color.red;
                errorDisplay.text += "<color=#ff0000>Test failed!</color>\n";
                playerState.CodePenalty();
            }

            EnemyMoveTrigger(resultBool, additional);
        }
    }

    void CallJDoodle() {
        string script = "{-# LANGUAGE ParallelListComp #-}\n" + CleanColorFormatting(codeField.text) + testCode;
        string executeURL = "https://api.jdoodle.com/v1/execute";
        string creditsURL = "https://api.jdoodle.com/v1/credit-spent";
        // Debug.Log(script);

        try {
            HttpWebRequest creditsHTTPRequest = (HttpWebRequest)WebRequest.Create(creditsURL);
            creditsHTTPRequest.Method = "POST";
            creditsHTTPRequest.ContentType = "application/json";

            JDoodleCreditsRequest creditsRequest = new JDoodleCreditsRequest();
            string creditsInput = JsonUtility.ToJson(creditsRequest);

            using (StreamWriter streamWriter = new StreamWriter(creditsHTTPRequest.GetRequestStream()))
            {
                streamWriter.Write(creditsInput);
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse creditsHTTPResponse = (HttpWebResponse)creditsHTTPRequest.GetResponse();

            if (creditsHTTPResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Please check your inputs: HTTP error code: " + (int)creditsHTTPResponse.StatusCode);
            }

            JDoodleCreditsResponse creditsResponse;
            using (Stream dataStream = creditsHTTPResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(dataStream))
            {
                string output = reader.ReadToEnd();
                creditsResponse = JsonUtility.FromJson<JDoodleCreditsResponse>(output);
                Debug.Log(output);
            }

            if (creditsResponse.error != null) {
                errorDisplay.color = Color.red;
                errorDisplay.text = creditsResponse.error;
            }
            else if (creditsResponse.used <= CREDIT_LIMIT) {
                HttpWebRequest executeRequest = (HttpWebRequest)WebRequest.Create(executeURL);
                executeRequest.Method = "POST";
                executeRequest.ContentType = "application/json";

                JDoodleRequest inputRequest = new JDoodleRequest(script);
                string input = JsonUtility.ToJson(inputRequest);

                using (StreamWriter streamWriter = new StreamWriter(executeRequest.GetRequestStream()))
                {
                    streamWriter.Write(input);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse response = (HttpWebResponse)executeRequest.GetResponse();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new WebException("Please check your inputs: HTTP error code: " + (int)response.StatusCode);
                }

                JDoodleResponse outputResponse;
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    string output = reader.ReadToEnd();
                    outputResponse = JsonUtility.FromJson<JDoodleResponse>(output);
                    Debug.Log(output);
                }

                EvaluateResult(outputResponse.output);
                
                response.Close();

            }
            else {
                errorDisplay.color = Color.yellow;
                errorDisplay.text = "There are no more compiler credits remaining today - please try again tomorrow.";
            }
            creditsHTTPResponse.Close();
        }
        catch (WebException e)
        {
            errorDisplay.color = Color.yellow;
            errorDisplay.text = "There was an error with the request. Please try again.\n" + e.Message;
        }
    } 
}
