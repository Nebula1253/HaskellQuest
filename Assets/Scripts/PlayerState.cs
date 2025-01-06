using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerState : NetworkBehaviour
{
    public int maxHealth;
    private NetworkList<int> health = new NetworkList<int>();
    private List<HealthBar> bars = new List<HealthBar>();
    public int initCodeScore = 5000, initDamageScore = 5000;
    private int codeScore, damageScore;
    public int incorrectCodePenalty, damagePenalty;
    private Button screenClick;
    private bool battleDone = false;
    public GameObject scoreDisplay, gameOverOverlay, dialogBox;
    public GameObject singlePlayerHealthBar;
    public GameObject multiplayerHealthBars;
    private EnemyController enemyController;
    public static PlayerState Instance { get; private set; }

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }


    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.ConnectedClientsList.Count > 1) {
        // if (true) { // debugging purposes
            // multi-player
            multiplayerHealthBars.SetActive(true);
            singlePlayerHealthBar.SetActive(false);

            foreach (var bar in multiplayerHealthBars.GetComponentsInChildren<HealthBar>())
            {
                bar.setHealth(maxHealth, maxHealth);
                bars.Add(bar);
            }
        }
        else {
            // single-player
            singlePlayerHealthBar.SetActive(true);
            multiplayerHealthBars.SetActive(false);

            health.Add(maxHealth); // P1 health

            var bar = singlePlayerHealthBar.GetComponentInChildren<HealthBar>();
            bar.setHealth(maxHealth, maxHealth);
            bars.Add(bar);
        }
        codeScore = initCodeScore;
        damageScore = initDamageScore;

        screenClick = GameObject.Find("ScreenClick").GetComponent<Button>();
        screenClick.onClick.AddListener(AdvanceScene);

        enemyController = GameObject.Find("EnemyView").GetComponent<EnemyController>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Debug.Log("ON NETWORK SPAWN");

        if (IsServer) {
            health.Add(maxHealth); // P1 health
            health.Add(maxHealth); // P2 health
        }
        else if (IsClient) {
            health.OnListChanged += OnListChanged;
        }
    }

    private void OnListChanged(NetworkListEvent<int> changeEvent) {
        bars[changeEvent.Index].setHealth(changeEvent.Value, maxHealth);

        if (changeEvent.Value <= 0) {
            GameOver();
        }
    }

    public void updateHealth(int healthDelta, int player) {
        if (IsServer) {
            health[player] = Mathf.Clamp(health[player] + healthDelta, 0, maxHealth);
            bars[player].setHealth(health[player], maxHealth);
        }
        
        if (health[player] <= 0) { // TODO probably needs to be changed
            GameOver();
        }
    }

    private void GameOver() {
        var mainBattle = PlayerHUD.Instance;
        mainBattle.moveToCentreCall(false, true);
;
        var codeEditor = CodeEditor.Instance;
        codeEditor.MoveOffScreenGameOver();

        gameOverOverlay.SetActive(true);

        GameObject.Find("EnemyView").GetComponent<EnemyController>().stopMusic();

        try {
            var effects = GameObject.FindGameObjectsWithTag("EffectUI");
            Debug.Log("Effects found");
            foreach (var item in effects)
            {
                item.SetActive(false);
            }
        }
        catch (System.Exception) {
            Debug.Log("No effects found");
        }
    }

    public void CodePenalty() {
        codeScore = Mathf.Max(0, codeScore - incorrectCodePenalty);
    }

    public void DamagePenalty() {
        damageScore = Mathf.Max(0, damageScore - damagePenalty);
    }

    public void DisplayScore() {
        StartCoroutine(DisplayScoreCoroutine());
    }

    IEnumerator DisplayScoreCoroutine() {
        // if there's an ending dialogue, wait for it to finish
        while (!enemyController.IsBattleEnded()) {
            yield return null;
        }

        scoreDisplay.SetActive(true);
        var codeScoreText = scoreDisplay.transform.Find("CodeScore").GetComponent<TMP_Text>();
        var damageScoreText = scoreDisplay.transform.Find("DamageScore").GetComponent<TMP_Text>();
        var finalScoreText = scoreDisplay.transform.Find("FinalScore").GetComponent<TMP_Text>();
        var clickToContinue = scoreDisplay.transform.Find("ClickToContinue").GetComponent<TMP_Text>();

        codeScoreText.text = "CODE SCORE: " + codeScore;
        if (codeScore == initCodeScore)
            codeScoreText.text += "<color=#FFFF00> PERFECT!</color>";
        yield return new WaitForSecondsRealtime(0.6f);

        damageScoreText.text = "DAMAGE SCORE: " + damageScore;
        if (damageScore == initDamageScore)
            damageScoreText.text += "<color=#FFFF00> PERFECT!</color>";
        yield return new WaitForSecondsRealtime(0.6f);
        
        finalScoreText.text = "FINAL SCORE: " + (codeScore + damageScore);
        if (codeScore + damageScore == initCodeScore + initDamageScore)
            finalScoreText.text += "<color=#FFFF00> PERFECT!</color>";
        yield return new WaitForSecondsRealtime(0.6f);

        clickToContinue.text = "CLICK TO CONTINUE";
        
        battleDone = true;
    }

    private void AdvanceScene() {
        if (battleDone) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
