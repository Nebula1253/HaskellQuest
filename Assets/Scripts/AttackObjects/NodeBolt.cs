using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NodeBolt : MonoBehaviour
{
    public GameObject nodeBolt, intBolt, floatBolt;
    private float angle, speed;
    private int whichLayer, intDamage;
    private float floatDamage;

    public void SetLayer (int layer) {
        whichLayer = layer;
    }

    public void SetAngleSpeed(float angle, float speed) {
        this.angle = angle;
        this.speed = speed;
    }

    public void SetDamageValues(int intDamage, float floatDamage) {
        this.intDamage = intDamage;
        this.floatDamage = floatDamage;

        // given this is called last by anything spawning a NodeBolt, start the split countdown here
        StartCoroutine(SplitIntoChildNodes());
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

    IEnumerator SplitIntoChildNodes() {
        yield return new WaitForSecondsRealtime(0.85f);

        if (NetworkHelper.Instance.IsPlayerOne) {
            if (whichLayer == 1) {
                GameObject iBolt = Instantiate(intBolt, transform.position, Quaternion.AngleAxis(-angle, Vector3.forward.normalized));
                iBolt.GetComponent<IntBolt>().SetDamage(intDamage);
                iBolt.GetComponent<IntBolt>().SetSpeed(speed);

                iBolt.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);

                GameObject fBolt = Instantiate(floatBolt, transform.position, Quaternion.AngleAxis(angle, Vector3.forward.normalized));
                fBolt.GetComponent<FloatBolt>().SetDamageFactor(floatDamage);
                fBolt.GetComponent<FloatBolt>().SetSpeed(speed);

                fBolt.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
            }
            else {
                GameObject leftNode = Instantiate(nodeBolt, transform.position, Quaternion.AngleAxis(angle, Vector3.forward.normalized));

                var leftNodeBolt = leftNode.GetComponent<NodeBolt>();
                leftNodeBolt.SetLayer(whichLayer - 1);
                leftNodeBolt.SetAngleSpeed(angle / 2, speed);
                leftNodeBolt.SetDamageValues(intDamage, floatDamage);

                leftNode.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);

                GameObject rightNode = Instantiate(nodeBolt, transform.position, Quaternion.AngleAxis(-angle, Vector3.forward.normalized));

                var rightNodeBolt = rightNode.GetComponent<NodeBolt>();
                rightNodeBolt.SetLayer(whichLayer - 1);
                rightNodeBolt.SetAngleSpeed(angle / 2, speed);
                rightNodeBolt.SetDamageValues(intDamage, floatDamage);

                rightNode.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
            }

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // what to do here? pretty sure i want it to damage the player, but how severely?
        if (collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerAvatar>().TakeDamage(intDamage);
            collision.gameObject.GetComponent<PlayerAvatar>().TakeDamage(floatDamage);
        }
    }

    void OnBecameInvisible() {
        if (NetworkHelper.Instance.IsPlayerOne) {
            Destroy(gameObject);
        }
    }
}
