using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    public int health, maxHealth;
    public int initCodeScore = 5000, initDamageScore = 5000;
    private int codeScore, damageScore;
    public int incorrectCodePenalty, damagePenalty;
    private HealthBar playerHealthBar;
    private Button screenClick;
    private bool battleDone = false;
    public GameObject scoreDisplay, gameOverOverlay;
    private 

    // Start is called before the first frame update
    void Start()
    {
        playerHealthBar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
        // Debug.Log(gameOverOverlay);
        health = maxHealth;
        playerHealthBar.setHealth(health, maxHealth);
        codeScore = initCodeScore;
        damageScore = initDamageScore;

        screenClick = GameObject.Find("ScreenClick").GetComponent<Button>();
        screenClick.onClick.AddListener(AdvanceScene);
    }

    public void updateHealth(int healthDelta) {
        health = Mathf.Clamp(health + healthDelta, 0, maxHealth);
        playerHealthBar.setHealth(health, maxHealth);
        
        if (health <= 0) {
            GameOver();
        }
    }

    private void GameOver() {
        var mainBattle = GameObject.Find("MainBattle").GetComponent<MainBattle>();
        mainBattle.moveToCentreCall(false, true);

        var codeEditor = GameObject.Find("CodeEditor").GetComponent<CodeEditor>();
        codeEditor.MoveOffScreenGameOver();

        gameOverOverlay.SetActive(true);

        GameObject.Find("EnemyView").GetComponent<EnemyController>().muteMusic();

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
        codeScore -= incorrectCodePenalty;
    }

    public void DamagePenalty() {
        damageScore -= damagePenalty;
    }

    public void DisplayScore() {
        StartCoroutine(DisplayScoreCoroutine());
    }

    IEnumerator DisplayScoreCoroutine() {
        scoreDisplay.SetActive(true);
        var codeScoreText = scoreDisplay.transform.Find("CodeScore").GetComponent<TMP_Text>();
        var damageScoreText = scoreDisplay.transform.Find("DamageScore").GetComponent<TMP_Text>();
        var finalScoreText = scoreDisplay.transform.Find("FinalScore").GetComponent<TMP_Text>();

        codeScoreText.text = "CODE SCORE: " + codeScore;
        if (codeScore == initCodeScore)
            codeScoreText.text += "<color=#FFFF00> PERFECT!</color>";
        yield return new WaitForSeconds(0.6f);

        damageScoreText.text = "DAMAGE SCORE: " + damageScore;
        if (damageScore == initDamageScore)
            damageScoreText.text += "<color=#FFFF00> PERFECT!</color>";
        yield return new WaitForSeconds(0.6f);
        
        finalScoreText.text = "FINAL SCORE: " + (codeScore + damageScore);
        if (codeScore + damageScore == initCodeScore + initDamageScore)
            finalScoreText.text += "<color=#FFFF00> PERFECT!</color>";

        battleDone = true;
    }

    private void AdvanceScene() {
        if (battleDone) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
