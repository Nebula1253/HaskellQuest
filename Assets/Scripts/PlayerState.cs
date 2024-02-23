using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerState : MonoBehaviour
{
    public int health, maxHealth;
    public int codeScore = 5000, damageScore = 5000;
    public int incorrectCodePenalty, damagePenalty;
    private HealthBar playerHealthBar;
    public GameObject scoreDisplay, gameOverOverlay;
    private 

    // Start is called before the first frame update
    void Start()
    {
        playerHealthBar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
        // Debug.Log(gameOverOverlay);
        health = maxHealth;
        playerHealthBar.setHealth(health, maxHealth);
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

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
        var codeScore = scoreDisplay.transform.Find("CodeScore").GetComponent<TextMeshProUGUI>();
        Debug.Log(codeScore);
        var damageScore = scoreDisplay.transform.Find("DamageScore").GetComponent<TextMeshProUGUI>();
        var finalScore = scoreDisplay.transform.Find("FinalScore").GetComponent<TextMeshProUGUI>();

        codeScore.text = "CODE SCORE: " + this.codeScore;
        yield return new WaitForSeconds(0.3f);

        damageScore.text = "DAMAGE SCORE: " + this.damageScore;
        yield return new WaitForSeconds(0.3f);
        
        finalScore.text = "FINAL SCORE: " + (this.codeScore + this.damageScore);
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
