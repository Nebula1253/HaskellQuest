using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpTopicOption : MonoBehaviour
{
    public TextAsset helpText;
    public GameObject helpView;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ShowHelpText);
    }

    void ShowHelpText() {
        helpView.SetActive(true);
        transform.parent.gameObject.SetActive(false);
        
        helpView.GetComponentInChildren<TMP_Text>().text = helpText.text;
        helpView.GetComponentInChildren<Scrollbar>().value = 1f;
    }
}
