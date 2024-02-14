using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject enemySprite, background;
    // Start is called before the first frame update
    void Start()
    {
        enemySprite = transform.Find("EnemySprite").gameObject;
        background = transform.Find("Background").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Esplode() {
        enemySprite.SetActive(false);
    }

    public virtual void PhaseTransition(int phase) {
        Debug.Log("Current phase:" + phase);
    }
}
