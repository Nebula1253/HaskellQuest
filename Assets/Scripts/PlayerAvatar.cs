using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine;
using System;

public class PlayerAvatar : NetworkBehaviour
{
    
    float minX, minY, maxX, maxY;

    private PlayerState playerState;
    private Rigidbody2D rb;
    private Vector2 movementDir;
    private Animator animator;
    private AudioSource hurtSound;
    public float playerSpeed;
    public float movementMagnitude;
    public int whichPlayer;
    private NetworkVariable<bool> isFrozen = new NetworkVariable<bool>(false);

    public Color freezeColor;

    void Start()
    {
        GameObject battlefield = GameObject.Find("BattleField");

        var boxSize = battlefield.transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        var spriteSize = GetComponent<SpriteRenderer>().bounds.size;
        var boxCentre = battlefield.transform.position;

        var boxSizeXOffset = boxSize.x / 2;
        var boxSizeYOffset = boxSize.y / 2;
        var spriteSizeXOffset = spriteSize.x / 2;
        var spriteSizeYOffset = spriteSize.y / 2;

        minX = -boxSizeXOffset + boxCentre.x + spriteSizeXOffset;
        minY = -boxSizeYOffset + boxCentre.y + spriteSizeYOffset;
        maxX = boxSizeXOffset + boxCentre.x - spriteSizeXOffset;
        maxY = boxSizeYOffset + boxCentre.y - spriteSizeYOffset;

        rb = GetComponent<Rigidbody2D>();
        playerState = PlayerState.Instance;

        animator = GetComponent<Animator>();
        hurtSound = GetComponent<AudioSource>();

        isFrozen.OnValueChanged += FreezeChangeColor;
    }

    private void FreezeChangeColor(bool previousValue, bool newValue)
    {
        if (newValue) {
            GetComponent<SpriteRenderer>().color = freezeColor;
        }
        else {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        movementDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movementDir.Normalize();
        if (isFrozen.Value) {
            movementDir = Vector2.zero;
        }

        movementMagnitude = movementDir.magnitude;

        rb.velocity = movementDir * playerSpeed;

        if (movementDir != Vector2.zero) {
            animator.SetFloat("Horizontal", movementDir.x);
            animator.SetFloat("Vertical", movementDir.y);
        }
        animator.SetFloat("Speed", movementMagnitude);

        // make sure the player stays within the bounds of the box
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
    }

    public void TakeDamage(int damage) {
        playerState.updateHealth(-damage, whichPlayer);

        HurtSoundRpc();
    }

    public void TakeDamage(float damageRatio) {
        playerState.updateHealth(damageRatio, whichPlayer);

        HurtSoundRpc();
    }

    [Rpc(SendTo.Everyone)]
    void HurtSoundRpc() {
        if (hurtSound.isPlaying) {
            hurtSound.Stop();
        }
        hurtSound.Play();
    }

    public void Freeze(float freezeDuration) {
        StartCoroutine(FreezeCoroutine(freezeDuration));
    }

    IEnumerator FreezeCoroutine(float freezeDuration) {
        // GetComponent<SpriteRenderer>().color = freezeColor; // need to do this better
        isFrozen.Value = true;

        yield return new WaitForSeconds(freezeDuration);

        isFrozen.Value = false;
        // GetComponent<SpriteRenderer>().color = Color.white;
    }
}
