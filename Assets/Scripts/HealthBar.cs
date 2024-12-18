using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public static HealthBar Instance { get; private set; }

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

    public Slider slider;
    public TMPro.TextMeshProUGUI healthTextMesh;

    public void setHealth(int health, int maxHealth) {
        slider.value = (float) health / maxHealth;
        healthTextMesh.text = health + "/" + maxHealth;
    }

    
}
