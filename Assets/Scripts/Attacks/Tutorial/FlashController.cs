using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashController : AttackController
{
    private int damage = 10;
    public float maxAlpha, timeForOneFlash;
    public Image image;
    private bool attackEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        maxAlpha = Mathf.Clamp(maxAlpha, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Trigger(bool result)
    {
        attackEnded = false;
        StartCoroutine(FlashRed());
    }

    public override void Trigger(bool result, string additionalConditions)
    {
        attackEnded = false;
        damage = int.Parse(additionalConditions);
        StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed() {
        yield return new WaitForSecondsRealtime(1.5f);
        float alpha;

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAvatar>().TakeDamage(damage);

        // red -> transparent
        for (float t = 0; t <= timeForOneFlash; t += Time.deltaTime) {
            alpha = Mathf.Lerp(maxAlpha, 0, t / timeForOneFlash);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        attackEnded = true;
    }

    public override bool AttackEnd() { return attackEnded; }
}
