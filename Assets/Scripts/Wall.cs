using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Sprite used to show the Player has successfully attacked the wall
    public Sprite damageSprite;

    // Health points of the wall
    public int hp = 4;

    // Store a component reference to the attached SpriteRenderer
    private SpriteRenderer spriteRenderer;

    void Awake ()
    {
        // Get a component reference to the SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //DamageWall is called when the player attacks a wall.
    public void DamageWall (int loss)
    {
        // Set spriteRenderer to the damaged wall sprite
        spriteRenderer.sprite = damageSprite;

        // Subtract loss from total health points
        hp -= loss;

        if (hp <= 0)
            // If the wall is dead (;-;), disable it
            gameObject.SetActive(false);
    }
}