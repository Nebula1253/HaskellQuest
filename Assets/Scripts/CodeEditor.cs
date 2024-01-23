using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text;
using System;
using UnityEngine.UI;
using System.Runtime.ConstrainedExecution;
using TMPro;

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
    private MainBattle mainBattleScript;
    public Color commentColor;
    private string colorCode;
    private float codeEditorXPos;
    public int currentScript = 0;

    public GameObject statusDisplayText;

    // Start is called before the first frame update
    void Start()
    {
        codeField = GetComponentInChildren<TMP_InputField>();
        statusDisplay = statusDisplayText.GetComponent<TMP_Text>();
        
        submitButton = GameObject.FindGameObjectWithTag("Submit").GetComponent<Button>();
        submitButton.onClick.AddListener(Submit);

        resetButton = GameObject.FindGameObjectWithTag("Reset").GetComponent<Button>();
        resetButton.onClick.AddListener(Reset);

        challengeCode = codeFiles[currentScript].text.Split("-- TEST CODE", StringSplitOptions.None)[0];
        testCode = codeFiles[currentScript].text.Split("-- TEST CODE", StringSplitOptions.None)[1];

        codeField.text = challengeCode;
        colorCode = "#" + ColorUtility.ToHtmlStringRGB(commentColor);

        codeEditorXPos = -964;
        // Debug.Log("Transform position x at start:" + transform.position.x);

        interactable = false;
        codeField.interactable = false;
        codeField.transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;

        mainBattleScript = mainBattle.GetComponent<MainBattle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable) {
            var codeText = codeField.text.Split('\n');
            string newCode = "";
            
            for (int i = 0; i < codeText.Length; i++) {
                string line = codeText[i];
                if (line.Contains("--") && !line.Contains("<color=" + colorCode + ">")) {
                    line = line.Replace("--", "<color=" + colorCode + ">--");
                    line += "</color>";
                }
                newCode += line + "\n";
            }
            codeField.text = newCode;

            if (Input.GetKeyDown(KeyCode.Escape)) {
                StartCoroutine(RenderInactiveEscape());
            }
        }
    }

    void Submit() {
        CallJDoodle();
    }

    void Reset() {
        codeField.text = challengeCode;
    }

    IEnumerator RenderInactiveEscape() {
        interactable = false;
        codeField.interactable = false;
        codeField.transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;

        submitButton.interactable = false;
        resetButton.interactable = false;

        statusDisplay.text = "";

        mainBattleScript.moveToCentreCall();

        while (GetComponent<RectTransform>().anchoredPosition.x <= 0) {
            GetComponent<RectTransform>().anchoredPosition += new Vector2(1, 0) * 5;
            yield return null;
        }
    }

    IEnumerator RenderInactive(bool phaseOver) {
        interactable = false;
        codeField.interactable = false;
        codeField.transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;

        submitButton.interactable = false;
        resetButton.interactable = false;

        while (!controllers[currentScript].AttackEnd()) {
            yield return null;
        }

        gameField.SetActive(false);

        statusDisplay.text = "";

        mainBattleScript.moveToCentreCall();

        while (GetComponent<RectTransform>().anchoredPosition.x <= 0) {
            GetComponent<RectTransform>().anchoredPosition += new Vector2(1, 0) * 5;
            yield return null;
        }

        if (phaseOver) {
            currentScript++;
            challengeCode = codeFiles[currentScript].text.Split("-- TEST CODE", StringSplitOptions.None)[0];
            testCode = codeFiles[currentScript].text.Split("-- TEST CODE", StringSplitOptions.None)[1];

            codeField.text = challengeCode;
            if (currentScript >= codeFiles.Length) {
                // CONGRATS YOU WON!!!!!
            }
        }
    }

    public void RenderActive() {
        StartCoroutine(MoveOnScreen());
        mainBattleScript.moveToTurnPosCall();
    }

    IEnumerator MoveOnScreen() {
        while (GetComponent<RectTransform>().anchoredPosition.x >= codeEditorXPos) {
            GetComponent<RectTransform>().anchoredPosition -= new Vector2(1, 0) * 5;
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
    void CallJDoodle() {
        string script = CleanColorFormatting(codeField.text) + testCode;

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
            //
            if (outputResponse.output.Contains("Timeout")) {
                // ask player to "try again - see if they've got an infinite loop": do nothing other than that, because 
                // this could just be JDoodle screwing around
                statusDisplay.color = Color.yellow;
                statusDisplay.text = "Your code timed out! Try again - see if you've got an infinite loop.";
            }
            else if (outputResponse.output.Contains("error")) {
                // put error details on screen, trigger enemy fire
                var error = outputResponse.output.Split('\n')[4];

                statusDisplay.color = Color.red;
                statusDisplay.text = error;

                gameField.SetActive(true);

                controllers = GameObject.FindGameObjectWithTag("Enemy").GetComponents<EnemyController>();
                controllers[currentScript].Trigger(false);

                StartCoroutine(RenderInactive(false));
            }
            else {
                // trigger enemy fire depending on whether the code passed the test
                string result = outputResponse.output.Split('\n')[2];
                bool resultBool = result == "True";
                if (resultBool) {
                    statusDisplay.color = Color.green;
                    statusDisplay.text = "Test passed!";
                }
                else {
                    statusDisplay.color = Color.red;
                    statusDisplay.text = "Test failed!";
                }

                gameField.SetActive(true);

                controllers = GameObject.FindGameObjectWithTag("Enemy").GetComponents<EnemyController>();
                controllers[currentScript].Trigger(resultBool);

                StartCoroutine(RenderInactive(resultBool));
            }
            response.Close();
        }
        catch (WebException e)
        {
            Debug.Log(e.ToString());
        }
    } 
}
