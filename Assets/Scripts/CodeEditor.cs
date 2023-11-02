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
    public string clientSecret = "feab49325586de1e07f91127b9680952abdaf9de99bd7bb6f959508cdd904d84";
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

    private string challengeCode;
    private string testCode;

    // Start is called before the first frame update
    void Start()
    {
        codeField = GetComponentInChildren<TMP_InputField>();
        submitButton = GetComponentInChildren<Button>();
        submitButton.onClick.AddListener(CallJDoodle);

        challengeCode = codeFile.text.Split("-- TEST CODE", StringSplitOptions.None)[0];
        testCode = codeFile.text.Split("-- TEST CODE", StringSplitOptions.None)[1];

        codeField.text = challengeCode;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CallJDoodle() {
        string script = codeField.text + testCode;

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
                // Debug.Log("Output from JDoodle ...." + output);
            }
            string result = outputResponse.output.Split('\n')[2];
            Debug.Log(result);

            response.Close();
        }
        catch (WebException e)
        {
            Debug.Log(e.ToString());
        }
    }   
}
