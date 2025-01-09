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

public class JDoodleRequest {
    public string clientId = "190c8528c5ed8a609a6322fb00818260";
    public string clientSecret = "f61f2873be5015e976b49dc18c4f33ce45133494fc1dafeb60480fc20b9f40ac";
    public string script;
    public string language = "haskell";
    public string versionIndex = "3";

    public JDoodleRequest(string script) {
        this.script = script;
    }
}

public class JDoodleCreditsRequest {
    public string clientId = "190c8528c5ed8a609a6322fb00818260";
    public string clientSecret = "f61f2873be5015e976b49dc18c4f33ce45133494fc1dafeb60480fc20b9f40ac";
}

public class JDoodleResponse {
    public string output;
    public string statusCode;
    public string memory;
    public string cpuTime;

    public JDoodleResponse(string output, string statusCode, string memory, string cpuTime) {
        this.output = output;
        this.statusCode = statusCode;
        this.memory = memory;
        this.cpuTime = cpuTime;
    }
}

public class JDoodleCreditsResponse {
    public int used;
    public string error;
    public string statusCode;
    
    public JDoodleCreditsResponse(int used, string error, string statusCode) {
        this.used = used;
        this.error = error;
        this.statusCode = statusCode;
    }
}

public class CodeEditor : MonoBehaviour
{
    const int CREDIT_LIMIT = 200;
    private TMP_InputField codeField;
    private TMP_Text statusDisplay;
    private Button submitButton, resetButton, helpButton, errorsButton;
    public TextAsset[] codeFiles;
    private AttackController[] controllers;

    private string challengeCode;
    private string testCode;
    private bool interactable = true;
    public GameObject gameField, playerHUD, overheadEnemy;
    private GameObject filename;
    private PlayerHUD playerHUDScript;
    public Color commentColor;
    public bool commentHighlighting;
    private string colorCode;
    private float codeEditorXPos;
    public int currentScript = 0;

    public GameObject statusDisplayObj, helpScreen;
    public float time;
    private float distanceDelta, helpScreenDistanceDelta;

    private PlayerState playerState;
    private bool gameOver = false;
    private EnemyController enemyController;
    private int cachedCaretPosition = -1;
    private bool helpScreenActive = false;

    public float codeBoxHeightWithErrors, codeBoxHeightWithoutErrors, errorDisplayOffPosY, errorDisplayOnPosY;
    public GameObject codeFieldObj, codeFieldScrollBar;
    private bool errorDisplayOn = false;
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
        helpScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(960, helpScreen.GetComponent<RectTransform>().anchoredPosition.y);
        statusDisplayObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(statusDisplayObj.GetComponent<RectTransform>().anchoredPosition.x, errorDisplayOffPosY);

        codeField = GetComponentInChildren<TMP_InputField>();
        statusDisplay = statusDisplayObj.GetComponentInChildren<TMP_Text>();
        statusDisplay.text = errorPlaceholderText;
        
        submitButton = GameObject.FindGameObjectWithTag("Submit").GetComponent<Button>();
        submitButton.onClick.AddListener(Submit);

        resetButton = GameObject.FindGameObjectWithTag("Reset").GetComponent<Button>();
        resetButton.onClick.AddListener(Reset);

        helpButton = GameObject.FindGameObjectWithTag("Help").GetComponent<Button>();
        helpButton.onClick.AddListener(Help);

        errorsButton = GameObject.FindGameObjectWithTag("Error").GetComponent<Button>();
        errorsButton.onClick.AddListener(Errors);

        challengeCode = codeFiles[currentScript].text.Split("-- TEST CODE", StringSplitOptions.None)[0];
        testCode = codeFiles[currentScript].text.Split("-- TEST CODE", StringSplitOptions.None)[1];

        codeField.text = challengeCode;
        colorCode = "#" + ColorUtility.ToHtmlStringRGB(commentColor);

        codeEditorXPos = -960;

        DisableEditor();

        playerHUDScript = PlayerHUD.Instance;

        filename = GameObject.Find("Filename");

        distanceDelta = Mathf.Abs(codeEditorXPos) / time;

        if (commentHighlighting) {
            CommentHighlighting();
            codeField.onValueChanged.AddListener(delegate {CommentHighlighting(); });
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
            // CommentHighlighting();

            if (Input.GetKeyDown(KeyCode.Escape)) {
                DisableEditor();
                StartCoroutine(MoveOffScreen(false));
            }
        }

        if (cachedCaretPosition != -1) {
            codeField.caretPosition = cachedCaretPosition;
            cachedCaretPosition = -1;
        }
    }

    private void CommentHighlighting() {
        bool commentChanged = false;
        var codeLines = codeField.text.Split('\n');
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
            cachedCaretPosition = codeField.caretPosition;
            codeField.text = newCode;

            // don't know why this would work, but let's hope it somehow does??
            codeField.caretPosition = cachedCaretPosition;
        }
    }

    void Submit() {
        CallJDoodle();
    }

    void Reset() {
        codeField.text = challengeCode;
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

    IEnumerator ErrorScreenActivate() {
        var statusDisplayRect = statusDisplayObj.GetComponent<RectTransform>();

        while (statusDisplayRect.anchoredPosition.y < errorDisplayOnPosY) {
            var newY = statusDisplayRect.anchoredPosition.y + (distanceDelta * Time.deltaTime);
            if (newY > errorDisplayOnPosY) {
                newY = errorDisplayOnPosY;
            }
            statusDisplayRect.anchoredPosition = new Vector2(statusDisplayRect.anchoredPosition.x, newY);
            yield return null;
        }

        errorDisplayOn = true;
    }

    IEnumerator ErrorScreenDeactivate() {
        var statusDisplayRect = statusDisplayObj.GetComponent<RectTransform>();

        while (statusDisplayRect.anchoredPosition.y > errorDisplayOffPosY) {
            var newY = statusDisplayRect.anchoredPosition.y - (distanceDelta * Time.deltaTime);
            if (newY < errorDisplayOffPosY) {
                newY = errorDisplayOffPosY;
            }
            statusDisplayRect.anchoredPosition = new Vector2(statusDisplayRect.anchoredPosition.x, newY);
            yield return null;
        }

        errorDisplayOn = false;
    }

    void Errors() {
        var sizeDelta = Math.Abs(codeBoxHeightWithErrors - codeBoxHeightWithoutErrors) / 2;

        var codeFieldRect = codeFieldObj.GetComponent<RectTransform>();
        var codeFieldScrollBarRect = codeFieldScrollBar.GetComponent<RectTransform>();

        if (errorDisplayOn) {
            codeFieldRect.sizeDelta = new Vector2(codeFieldRect.sizeDelta.x, codeBoxHeightWithoutErrors);
            codeFieldRect.anchoredPosition -= new Vector2(0, sizeDelta);

            codeFieldScrollBarRect.sizeDelta = new Vector2(codeFieldScrollBarRect.sizeDelta.x, codeBoxHeightWithoutErrors);
            codeFieldScrollBarRect.anchoredPosition -= new Vector2(0, sizeDelta);

            StartCoroutine(ErrorScreenDeactivate());
        }
        else {
            codeFieldRect.sizeDelta = new Vector2(codeFieldRect.sizeDelta.x, codeBoxHeightWithErrors);
            codeFieldRect.anchoredPosition += new Vector2(0, sizeDelta);

            codeFieldScrollBarRect.sizeDelta = new Vector2(codeFieldScrollBarRect.sizeDelta.x, codeBoxHeightWithErrors);
            codeFieldScrollBarRect.anchoredPosition += new Vector2(0, sizeDelta);

            StartCoroutine(ErrorScreenActivate());
        }
    }

    private void DisableEditor() {
        interactable = false;
        codeField.interactable = false;
        codeField.transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;

        submitButton.interactable = false;
        resetButton.interactable = false;
        helpButton.interactable = false;
        errorsButton.interactable = false;
    }

    IEnumerator MoveOffScreen(bool changeScript) {
        if (helpScreenActive) {
            StartCoroutine(HelpScreenDeactivate());
        }
        gameField.SetActive(false);
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
            
            challengeCode = codeFiles[currentScript].text.Split("-- TEST CODE", StringSplitOptions.None)[0];
            testCode = codeFiles[currentScript].text.Split("-- TEST CODE", StringSplitOptions.None)[1];

            codeField.text = challengeCode;
            
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
        gameField.SetActive(true);

        // controllers = GameObject.FindGameObjectWithTag("Enemy").GetComponents<AttackController>();
        if (additionalConditions == "") {
            controllers[currentScript].Trigger(result);
        }
        else {
            controllers[currentScript].Trigger(result, additionalConditions);
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
        helpButton.interactable = true;
        errorsButton.interactable = true;
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

        if (statusDisplay.text == errorPlaceholderText) {
            statusDisplay.text = "";
        }
        
        if (output.Contains("Timeout") || output == null) {
            if (controllers[currentScript].IsRecursionHandled()) {
                // statusDisplay.color = Color.red;
                statusDisplay.text += "<color=#ff0000>Your code recurses infinitely!</color>\n";

                playerState.CodePenalty();
                EnemyMoveTrigger(false, additional);
            }
            else {
                // // ask player to "try again - see if they've got an infinite loop": do nothing other than that, because 
                // // this could just be JDoodle screwing around
                // statusDisplay.color = Color.yellow;
                statusDisplay.text += "<color=#ffff00>Your code timed out! Try again - see if you've got an infinite loop.</color>\n";
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
            statusDisplay.text += "<color=#ff0000>" + error + "</color>\n";
            playerState.CodePenalty();

            EnemyMoveTrigger(false, additional);
        }
        else if (output.Split('\n')[2].Contains("Non-exhaustive")) {
            var errorStart = output.Split('\n')[2].IndexOf("Non-exhaustive");
            var error = output.Split('\n')[2].Substring(errorStart);
            // statusDisplay.color = Color.red;
            statusDisplay.text += "<color=#ff0000>" + error + "</color>\n";
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
                statusDisplay.text += "<color=#00ff00>Test passed!</color>\n";
            }
            else {
                // statusDisplay.color = Color.red;
                statusDisplay.text += "<color=#ff0000>Test failed!</color>\n";
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
                statusDisplay.color = Color.red;
                statusDisplay.text = creditsResponse.error;
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
                statusDisplay.color = Color.yellow;
                statusDisplay.text = "There are no more compiler credits remaining today - please try again tomorrow.";
            }
            creditsHTTPResponse.Close();
        }
        catch (WebException e)
        {
            statusDisplay.color = Color.yellow;
            statusDisplay.text = "There was an error with the request. Please try again.\n" + e.Message;
        }
    } 
}
