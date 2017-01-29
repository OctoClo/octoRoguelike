using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    // Damage done to the player when the enemy attacks him
    public int playerDamage;

    // Used to store a reference to the Enemy's animator component
    private Animator animator;

    // Transform to attempt to move toward each turn
    private Transform target;

    // Boolean to determine whether or not enemy should skip a turn or move this turn
    private bool skipMove;
    
    protected override void Start ()
    {
        // Add this to the list of enemies
        GameManager.instance.AddEnemyToList(this);
        
        // Get a component reference to the Enemy's animator component
        animator = GetComponent<Animator>();

        // Find the Player GameObject using its tag and store a reference to its transform component
        target = GameObject.FindGameObjectWithTag("Player").transform;
        
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            // Have the enemy moves every other turn
            skipMove = false;
            return;
        }

        else
        {
            base.AttemptMove<T>(xDir, yDir);

            // Now that Enemy has moved, set skipMove to true to skip next move
            skipMove = true;
        }
    }

    public void MoveEnemy()
    {
        // Declare variables for X and Y axis move directions, these range from -1 to 1
        int xDir = 0;
        int yDir = 0;

        // VEEERY complicated calculations to determine the direction to move
        // If the difference in positions is approximately zero (Epsilon) do the following:
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            // If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
            yDir = target.position.y > transform.position.y ? 1 : -1;
        // If the difference in positions is not approximately zero (Epsilon) do the following:
        else
            // Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left)
            xDir = target.position.x > transform.position.x ? 1 : -1;

        // Call AttemptMove passing in the generic parameter Player to interact with and the direction
        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove <T> (T component)
    {
        // Set hitPlayer to equal the component passed in as a parameter
        Player hitPlayer = component as Player;

        //Set the attack trigger of the Enemy's animation Controller
        animator.SetTrigger("enemyAttack");

        // Call the LoseFood function of hitPlayer
        hitPlayer.LoseFood(playerDamage);
    }
}
