using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    protected GameObject enemySprite, background;
    protected bool battleEnded = false;
    // Start is called before the first frame update
    protected void Start()
    {
        enemySprite = transform.Find("EnemySprite").gameObject;
        background = transform.Find("Background").gameObject;
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
