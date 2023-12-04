using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject missile;
    public int nrMissiles;
    public float missileCircleRadius, missileDelay;
    public Vector3 firstMissilePos;

    private float missileDegreeIncrement;
    private Transform target;

    void Start()
    {
        missileDegreeIncrement = 360f / nrMissiles;
    }

    public void Trigger(bool correct) {
        if (correct) {
            target = GameObject.FindGameObjectWithTag("Enemy").transform;
        } else {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        StartCoroutine(fireMissiles());
    }

    IEnumerator fireMissiles() {
        Vector3 currMissilePos;
        for (int i=0; i < nrMissiles; i++) {
            // find current missile position using missileDegreeIncrement, missileCircleRadius and firstMissilePos
            currMissilePos = new Vector3(firstMissilePos.x + missileCircleRadius * Mathf.Cos(missileDegreeIncrement * i * Mathf.Deg2Rad), 
                                        firstMissilePos.y + missileCircleRadius * Mathf.Sin(missileDegreeIncrement * i * Mathf.Deg2Rad), 
                                        firstMissilePos.z);

            GameObject newMissile = Instantiate(missile, currMissilePos, Quaternion.identity);
            newMissile.GetComponent<HomingMissile>().SetTarget(target);
            yield return new WaitForSeconds(missileDelay);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
