using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        setHealth(100, 100);
    }

    public Slider slider;
    // public Text healthText;
    public TMPro.TextMeshProUGUI healthTextMesh;

    public void setHealth(int health, int maxHealth) {
        slider.value = (float) health / maxHealth;
        healthTextMesh.text = health + "/" + maxHealth;
    }
}
