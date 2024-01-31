using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;
using System.Runtime.ConstrainedExecution;
using TMPro;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

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

public class CodeEditor : MonoBehaviour
{
    private TMP_InputField codeField;
    private TMP_Text statusDisplay;
    private Button submitButton, resetButton;
    public TextAsset[] codeFiles;
    private EnemyController[] controllers;

    private string challengeCode;
    private string testCode;
    private bool interactable = true;
    public GameObject gameField, mainBattle;
    private GameObject dialog, filename;
    private MainBattle mainBattleScript;
    public Color commentColor;
    private string colorCode;
    private float codeEditorXPos;
    public int currentScript = 0;

    public GameObject statusDisplayText;
    public float time;
    private float distanceDelta;

    // Start is called before the first frame update
    void Start()
    {
        codeField = GetComponentInChildren<TMP_InputField>();
        statusDisplay = statusDisplayText.GetComponent<TMP_Text>();
        
        submitButton = GameObject.FindGameObjectWithTag("Submit").GetComponent<Button>();
        submitButton.onClick.AddListener(Submit);

        resetButton = GameObject.FindGameObjectWithTag("Reset").GetComponent<Button>();
        resetButton.onClick.AddListener(Reset);

        Debug.Log(codeFiles[currentScript].name);

        challengeCode = codeFiles[currentScript].text.Split("-- TEST CODE", StringSplitOptions.None)[0];
        testCode = codeFiles[currentScript].text.Split("-- TEST CODE", StringSplitOptions.None)[1];

        codeField.text = challengeCode;
        colorCode = "#" + ColorUtility.ToHtmlStringRGB(commentColor);

        codeEditorXPos = -960;

        DisableEditor();

        mainBattleScript = mainBattle.GetComponent<MainBattle>();

        // dialog = transform.Find("DialogBox").gameObject;
        dialog = GameObject.Find("DialogBox");
        filename = GameObject.Find("Filename");

        distanceDelta = Mathf.Abs(codeEditorXPos) / time;

        // codeField.onValueChanged.AddListener(delegate {CommentHighlighting(); });

        filename.GetComponent<TMP_Text>().text = codeFiles[currentScript].name + ".hs";
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable) {
            CommentHighlighting();

            if (Input.GetKeyDown(KeyCode.Escape)) {
                DisableEditor();
                StartCoroutine(MoveOffScreen(false));
            }
        }
    }

    private void CommentHighlighting() {
        bool commentChanged = false;
        var codeText = codeField.text.Split('\n');
        string newCode = "";

        for (int i = 0; i < codeText.Length; i++) {
            string line = codeText[i];
            if (line.Contains("--") && !line.Contains("<color=" + colorCode + ">")) {
                var regex = new Regex("--");
                line = regex.Replace(line, "<color=" + colorCode + ">--", 1);

                // line = line.Replace("--", "<color=" + colorCode + ">--");
                line += "</color>";
                commentChanged = true;
            }
            else if (!line.Contains("--") && line.Contains("<color=" + colorCode + ">")) {
                line = line.Replace("<color=" + colorCode + ">", "");
                line = line.Replace("</color>", "");
                commentChanged = true;
            }
            newCode += line + "\n";
        }
        if (commentChanged) codeField.text = newCode;
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
    }

    IEnumerator MoveOffScreen(bool changeScript) {
        gameField.SetActive(false);
        statusDisplay.text = "";

        mainBattleScript.moveToCentreCall();

        while (GetComponent<RectTransform>().anchoredPosition.x < 0) {
            var newX = Mathf.Min(0, GetComponent<RectTransform>().anchoredPosition.x + distanceDelta * Time.deltaTime);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(newX, GetComponent<RectTransform>().anchoredPosition.y);
            yield return null;
        }

        if (changeScript) {
            currentScript++;
            if (currentScript >= codeFiles.Length) {
                // CONGRATS YOU WON!!!!!
                dialog.transform.Find("Text").GetComponent<TMP_Text>().text = "CONGRATULATIONS! YOU WON!";
            }
            else {
                challengeCode = codeFiles[currentScript].text.Split("-- TEST CODE", StringSplitOptions.None)[0];
                testCode = codeFiles[currentScript].text.Split("-- TEST CODE", StringSplitOptions.None)[1];

                codeField.text = challengeCode;
                filename.GetComponent<TMP_Text>().text = codeFiles[currentScript].name + ".hs";
            }
        }
    }

    IEnumerator EnemyMove(bool phaseOver) {
        DisableEditor();

        while (!controllers[currentScript].AttackEnd()) {
            yield return null;
        }

        StartCoroutine(MoveOffScreen(phaseOver));
    }

    public void ActivateEditor() {
        StartCoroutine(MoveOnScreen());
    }

    private void EnemyMoveTrigger(bool result) {
        gameField.SetActive(true);

        controllers = GameObject.FindGameObjectWithTag("Enemy").GetComponents<EnemyController>();
        controllers[currentScript].Trigger(result);

        StartCoroutine(EnemyMove(result));
    }

    IEnumerator MoveOnScreen() {
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
    }
    
    private string CleanColorFormatting(string code) {
        return code.Replace("<color=" + colorCode + ">", "").Replace("</color>", "");
    }

    private void EvaluateResult(string output) {
        if (output.Contains("Timeout") || output == null) {
            // ask player to "try again - see if they've got an infinite loop": do nothing other than that, because 
            // this could just be JDoodle screwing around
            statusDisplay.color = Color.yellow;
            statusDisplay.text = "Your code timed out! Try again - see if you've got an infinite loop.";
        }
        else if (output.Contains("error")) {
            // put error details on screen, trigger enemy fire
            var error = output.Split('\n')[4];

            statusDisplay.color = Color.red;
            statusDisplay.text = error;

            EnemyMoveTrigger(false);
        }
        else {
            // trigger enemy fire depending on whether the code passed the test
            string result = output.Split('\n')[2];
            bool resultBool = result == "True";
            if (resultBool) {
                statusDisplay.color = Color.green;
                statusDisplay.text = "Test passed!";
            }
            else {
                statusDisplay.color = Color.red;
                statusDisplay.text = "Test failed!";
            }

            EnemyMoveTrigger(resultBool);
        }
    }

    void CallJDoodle() {
        string script = CleanColorFormatting(codeField.text) + testCode;
        Debug.Log(script);

        try {
            string url = "https://api.jdoodle.com/v1/execute";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            JDoodleRequest inputRequest = new JDoodleRequest(script);
            string input = JsonUtility.ToJson(inputRequest);

            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(input);
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Please check your inputs : HTTP error code : " + (int)response.StatusCode);
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
        catch (WebException e)
        {
            Debug.Log(e.ToString());
        }
    } 
}
