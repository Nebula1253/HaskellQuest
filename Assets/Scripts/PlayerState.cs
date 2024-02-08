using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public int health, maxHealth;
    public int codeScore = 5000, damageScore = 5000;
    public int incorrectCodePenalty, damagePenalty;
    private HealthBar playerHealthBar;
    public TMP_Text scoreText;
    private 

    // Start is called before the first frame update
    void Start()
    {
        playerHealthBar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
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
            Debug.Log("Game Over");
        }
    }

    public void CodePenalty() {
        codeScore -= incorrectCodePenalty;
    }

    public void DamagePenalty() {
        damageScore -= damagePenalty;
    }

    public void DisplayScore() {
        scoreText.gameObject.SetActive(true);
        scoreText.text = "Code Score: " + codeScore + "\nDamage Score: " + damageScore + "\nFinal Score: " + (codeScore + damageScore); 
    }
}
