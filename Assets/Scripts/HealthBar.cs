using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private PlayerState player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerState").GetComponent<PlayerState>();
        setHealth(player.health, player.maxHealth);
    }

    public Slider slider;
    public TMPro.TextMeshProUGUI healthTextMesh;

    private void setHealth(int health, int maxHealth) {
        slider.value = (float) health / maxHealth;
        healthTextMesh.text = health + "/" + maxHealth;
    }

    public void updateHealth(int healthDelta) {
        player.health = Mathf.Clamp(player.health + healthDelta, 0, player.maxHealth);
        setHealth(player.health, player.maxHealth);
        // setHealth(player.health, player.maxHealth);
    }
}
