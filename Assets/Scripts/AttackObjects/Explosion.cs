using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginExplosion() {
        if (NetworkHelper.Instance.IsPlayerOne) {
            GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
            StartCoroutine(explode());
        }
    }

    IEnumerator explode() {
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}
