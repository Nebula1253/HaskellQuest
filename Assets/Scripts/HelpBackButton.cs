using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpBackButton : MonoBehaviour
{
    public GameObject helpOptions;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ShowHelpOptions);
    }

    void ShowHelpOptions() {
        transform.parent.gameObject.SetActive(false);
        helpOptions.SetActive(true);
    }
}
