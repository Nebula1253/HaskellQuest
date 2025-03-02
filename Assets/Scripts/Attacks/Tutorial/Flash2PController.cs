using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Flash2PController : AttackController
{
    private int damage = 10;
    private float damageRatio = 0.8f;
    public float maxAlpha, timeForOneFlash;
    public Image image;
    private bool attackEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        maxAlpha = Mathf.Clamp(maxAlpha, 0, 1);
    }

    public override void Trigger(bool result)
    {
        attackEnded = false;
        StartFlashRpc();
    }

    public override void Trigger(bool result, string additionalConditions)
    {
        attackEnded = false;
        ParseDamageVals(additionalConditions);
        StartFlashRpc();
    }

    private void ParseDamageVals(string additionalConditions) {
        var split = additionalConditions.Split(",");

        var flatDamage = split[0].Trim().Replace("(", "");
        Debug.Log(flatDamage);
        damage = int.Parse(flatDamage);

        var ratio = split[1].Trim().Replace(")", "");
        Debug.Log(ratio);
        damageRatio = float.Parse(ratio);
    }

    [Rpc(SendTo.Everyone)]
    void StartFlashRpc() {
        StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed() {
        yield return new WaitForSecondsRealtime(1.5f);
        float alpha;

        // first attack
        if (NetworkHelper.Instance.IsPlayerOne) {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player")) {
                player.GetComponent<PlayerAvatar>().TakeDamage(damage);
            }
        }
        
        // red -> transparent
        for (float t = 0; t <= timeForOneFlash; t += Time.deltaTime) {
            alpha = Mathf.Lerp(maxAlpha, 0, t / timeForOneFlash);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

        yield return new WaitForSecondsRealtime(1f);

        // second attack
        if (NetworkHelper.Instance.IsPlayerOne) {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player")) {
                player.GetComponent<PlayerAvatar>().TakeDamage(damageRatio);
            }
        }

        // red -> transparent
        for (float t = 0; t <= timeForOneFlash; t += Time.deltaTime) {
            alpha = Mathf.Lerp(maxAlpha, 0, t / timeForOneFlash);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

        attackEnded = true;
    }

    public override bool AttackEnd()
    {
        return attackEnded;
    }
}
