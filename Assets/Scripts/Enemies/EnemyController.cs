using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    protected const string SWITCH_STR = "--- SWITCH ---";
    protected GameObject enemySprite, background;
    protected Button hackButton;
    protected bool battleEnded = false;
    protected DialogBox dbox;
    public static EnemyController Instance { get; private set; }
    public bool skipDialogue;
    protected bool startCalled = false;

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
    protected void Start()
    {
        enemySprite = transform.Find("EnemySprite").gameObject;
        background = transform.Find("Background").gameObject;
        dbox = DialogBox.Instance;
        hackButton = GameObject.FindGameObjectWithTag("HackButton").GetComponent<Button>();
        Debug.Log(hackButton);

        startCalled = true;
    }

    public virtual void BattleEnd() {
        stopMusic();
    }

    public virtual void PhaseTransition(int phase) {
        Debug.Log("Current phase:" + phase);
    }

    public void stopMusic() {
        var source = GetComponent<AudioSource>();
        source.Stop();
    }

    public bool IsBattleEnded() {
        return battleEnded;
    }
}
