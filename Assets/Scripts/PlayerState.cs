using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
using System.Reflection;
using UnityEditor;

public class PlayerState : NetworkBehaviour
{
    public int maxHealth;
    private NetworkList<int> health;
    private List<HealthBar> bars = new List<HealthBar>();
    private Button screenClick;
    private bool battleDone = false;
    public GameObject gameOverOverlay, dialogBox;
    public GameObject singlePlayerHealthBar;
    public GameObject multiplayerHealthBars;
    private EnemyController enemyController;
    public static PlayerState Instance { get; private set; }

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        health ??= new NetworkList<int>();

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
        if (NetworkHelper.Instance.IsMultiplayer) {
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

            var bar = singlePlayerHealthBar.GetComponentInChildren<HealthBar>();
            bar.setHealth(maxHealth, maxHealth);
            bars.Add(bar);
        }

        screenClick = GameObject.Find("ScreenClick").GetComponent<Button>();
        screenClick.onClick.AddListener(AdvanceScene);

        enemyController = GameObject.Find("EnemyView").GetComponent<EnemyController>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (NetworkHelper.Instance.IsMultiplayer) {
            // multiplayer
            if (IsServer) {
                health.Add(maxHealth); // P1 health
                health.Add(maxHealth); // P2 health
            }
            else if (IsClient) {
                health.OnListChanged += OnListChanged;
            }
        }
        else {
            // singleplayer
            health.Add(maxHealth);
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
        var codeEditor = GameObject.FindGameObjectWithTag("CodeEditor").GetComponent<CodeEditor>();
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

    public void DisplayScore() {
        StartCoroutine(DisplayScoreCoroutine());
    }

    IEnumerator DisplayScoreCoroutine() {
        // if there's an ending dialogue, wait for it to finish
        while (!enemyController.IsBattleEnded()) {
            yield return null;
        }
        
        battleDone = true;
    }

    private void AdvanceScene() {
        if (battleDone) {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1), LoadSceneMode.Single);
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
