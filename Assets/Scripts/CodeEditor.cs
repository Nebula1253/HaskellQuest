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
    private Button submitButton;
    public TextAsset codeFile;
    private EnemyController controller;

    private string challengeCode;
    private string testCode;
    private bool interactable = true;
    public GameObject gameField;

    // Start is called before the first frame update
    void Start()
    {
        codeField = GetComponentInChildren<TMP_InputField>();
        submitButton = GetComponentInChildren<Button>();
        submitButton.onClick.AddListener(Submit);

        challengeCode = codeFile.text.Split("-- TEST CODE", StringSplitOptions.None)[0];
        testCode = codeFile.text.Split("-- TEST CODE", StringSplitOptions.None)[1];

        codeField.text = challengeCode;
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable) {
            var codeText = codeField.text.Split('\n');
            string newCode = "";
            for (int i = 0; i < codeText.Length; i++) {
                string line = codeText[i];
                if (line.Contains("--") && !line.Contains("<color=green>")) {
                    line = line.Replace("--", "<color=green>--");
                    line += "</color>";
                }
                newCode += line + "\n";
            }
            codeField.text = newCode;
        }
    }

    void Submit() {
        CallJDoodle();
        StartCoroutine(RenderInactive());
    }

    IEnumerator RenderInactive() {
        interactable = false;
        codeField.interactable = false;
        codeField.transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;

        while (!controller.AttackEnd()) {
            yield return null;
        }
        
        codeField.interactable = true;
        interactable = true;
        codeField.transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.white;
        gameField.SetActive(false);
    }
    private string CleanColorFormatting(string code) {
        return code.Replace("<color=green>", "").Replace("</color>", "");
    }
    void CallJDoodle() {
        string script = CleanColorFormatting(codeField.text) + testCode;

        try {
            string url = "https://api.jdoodle.com/v1/execute";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            JDoodleRequest inputRequest= new JDoodleRequest(script);
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
            string result = outputResponse.output.Split('\n')[2];
            // Debug.Log(result);

            gameField.SetActive(true);
            controller = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyController>();

            response.Close();

            if (result == "True") {
                controller.Trigger(true); 
            } else {
                controller.Trigger(false);
            }
        }
        catch (WebException e)
        {
            Debug.Log(e.ToString());
        }
    } 
}
