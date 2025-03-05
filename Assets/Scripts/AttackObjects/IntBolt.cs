using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntBolt : MonoBehaviour
{
    private int dmg;
    private float speed;

    public void SetDamage(int dmg) {
        this.dmg = dmg;
    }

    public void SetSpeed(float speed) {
        this.speed = speed;
    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && NetworkHelper.Instance.IsPlayerOne) {
            Debug.Log(dmg);
            collision.gameObject.GetComponent<PlayerAvatar>().TakeDamage(dmg);

            Destroy(gameObject);
        }
    }

    void OnBecameInvisible() {
        if (NetworkHelper.Instance.IsPlayerOne) {
            Destroy(gameObject);
        }
    }
}
