using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        movementDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movementDir.Normalize();
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

        if (hurtSound.isPlaying) {
            hurtSound.Stop();
        }
        hurtSound.Play();
        
        if (damage > 0) { // because the tutorial was screwing this up otherwise
            playerState.DamagePenalty();
        }
    }

}
