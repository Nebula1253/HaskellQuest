using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private PlayerAvatar playerToDamage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginExplosion(PlayerAvatar player) {
        playerToDamage = player;
        StartCoroutine(explode());
    }

    IEnumerator explode() {
        playerToDamage.TakeDamage(5);

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}
