using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Landmine : NetworkBehaviour
{
    public GameObject explosionPrefab;
    private Vector3 targetPos, targetIncrementPerSecond;
    private bool targetSet = false;
    public float settingTime;
    private bool isDamaging;
    public Color freezeColor;

    public void setTargetPos(Vector3 targetPos) {
        targetIncrementPerSecond = (targetPos - transform.position) / settingTime;
        this.targetPos = targetPos;
        targetSet = true;
    }

    public void setIsDamaging(bool isDamaging) {
        this.isDamaging = isDamaging;
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetSet) return;

        Vector3 newPos = transform.position + (targetIncrementPerSecond * Time.deltaTime);

        if ((newPos - targetPos).magnitude < (transform.position - targetPos).magnitude) {
            transform.position = newPos;
        }
        else {
            transform.position = targetPos;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player") && isDamaging) {
            var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            
            explosion.GetComponent<Explosion>().BeginExplosion();

            other.gameObject.GetComponent<PlayerAvatar>().TakeDamage(5);

            if (NetworkHelper.Instance.IsPlayerOne) {
                Destroy(gameObject);
            }
        }
    }

    [Rpc(SendTo.Everyone)] 
    public void FreezeRpc() {
        isDamaging = false;
        GetComponent<SpriteRenderer>().color = freezeColor;
    }
}
