using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HomingMissileController : AttackController
{
    public GameObject missile;
    public int nrMissiles;
    public float missileSpeed, missileRotateSpeed, missileLockDelay;
    public float missileAngleRange;
    private Transform target, enemyTransform;
    private List<Transform> playerTransforms = new List<Transform>();
    private GameObject gameField;
    private bool missilesFired = false;
    
    // Start is called before the first frame update
    void Start()
    {
        gameField = GameObject.FindGameObjectWithTag("GameField");
        // playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        enemyTransform = gameObject.transform;
    }

    public override void Trigger(bool result) {
        if (IsServer) {
            StartCoroutine(fireMissiles(result));
        }
    }

    private GameObject InstantiateMissile(Vector3 position, Transform target) {
        var missileAngle = 180 + Random.Range(-missileAngleRange, missileAngleRange);
        Debug.Log(missileAngle);
        var missileRotation = Quaternion.AngleAxis(missileAngle, Vector3.forward.normalized);
        Vector3 missileStartOffset = missileRotation * new Vector3(0, 1.5f, 0);

        GameObject newMissile = Instantiate(missile, position + missileStartOffset, missileRotation, gameField.transform);
        newMissile.GetComponent<HomingMissile>().SetTarget(target);
        newMissile.GetComponent<HomingMissile>().SetSpeed(missileSpeed);
        newMissile.GetComponent<HomingMissile>().SetRotateSpeed(missileRotateSpeed);
        newMissile.GetComponent<HomingMissile>().SetLockDelay(missileLockDelay);

        newMissile.GetComponent<NetworkObject>().Spawn();

        return newMissile;
    }

    IEnumerator fireMissiles(bool redirect = false) {
        yield return new WaitForSecondsRealtime(1f);

        if (playerTransforms.Count == 0) {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                playerTransforms.Add(player.transform);
            }
        }

        int nrPlayers = playerTransforms.Count;
        int whichPlayer = 0;

        for (int i=0; i < (nrMissiles * nrPlayers); i++) {
            target = playerTransforms[whichPlayer];
            whichPlayer = whichPlayer == 0 ? nrPlayers - 1 : 0;
            Debug.Log(target);

            var missile = InstantiateMissile(transform.position, target);
            yield return new WaitForSecondsRealtime(0.5f);
            if (redirect) {
                target = enemyTransform;
                missile.GetComponent<HomingMissile>().SetTarget(target);
            }
        }
        
        missilesFired = true;
    }

    public override bool AttackEnd() {
        if (missilesFired) {
            if (GameObject.FindGameObjectsWithTag("Projectile").Length == 0) {
                missilesFired = false;
            }
            return GameObject.FindGameObjectsWithTag("Projectile").Length == 0;
        }
        else {
            return false;
        }
    }
}
