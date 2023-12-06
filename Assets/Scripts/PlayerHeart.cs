using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeart : MonoBehaviour
{
    // Start is called before the first frame update
    public int health = 100;
    public int maxHealth = 100;
    float minX, minY, maxX, maxY;
    public HealthBar playerHealthBar;
    void Start()
    {
        // var boxSize = transform.parent.GetComponent<Renderer>().bounds.size;
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

        playerHealthBar.setHealth(health, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.position += new Vector3(horizontalInput, verticalInput, 0) * Time.deltaTime * 5;

        // make sure the player stays within the bounds of the box
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Projectile") {
            health -= 10;
            playerHealthBar.setHealth(health, maxHealth);
        }
    }
}
