using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHeart : MonoBehaviour
{
    
    float minX, minY, maxX, maxY;

    private PlayerState playerState;
    private Rigidbody2D rb;
    private Vector2 movementDir;
    private Animator animator;
    public float playerSpeed;
    public float movementMagnitude;

    void Start()
    {
        // var boxSize = transform.parent.GetComponent<Renderer>().bounds.size

        var boxSize = transform.parent.GetChild(0).GetComponent<Renderer>().bounds.size;
        var spriteSize = GetComponent<SpriteRenderer>().bounds.size;
        var boxCentre = transform.parent.position;

        var boxSizeXOffset = boxSize.x / 2;
        var boxSizeYOffset = boxSize.y / 2;
        var spriteSizeXOffset = spriteSize.x / 2;
        var spriteSizeYOffset = spriteSize.y / 2;

        minX = -boxSizeXOffset + boxCentre.x + spriteSizeXOffset;
        minY = -boxSizeYOffset + boxCentre.y + spriteSizeYOffset;
        maxX = boxSizeXOffset + boxCentre.x - spriteSizeXOffset;
        maxY = boxSizeYOffset + boxCentre.y - spriteSizeYOffset;

        rb = GetComponent<Rigidbody2D>();

        playerState = GameObject.Find("PlayerState").GetComponent<PlayerState>();

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
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
        // health -= damage;
        // playerController.health -= damage;
        // playerHealthBar.setHealth(health, maxHealth);
        playerState.updateHealth(-damage);
        playerState.DamagePenalty();
    }

}
