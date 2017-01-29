using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    // Damage done to the walls when the player chops them
    public int wallDamage = 1;

    // Player's variables
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;

    // Delay time in seconds to restart level
    public float restartLevelDelay = 1f;

    public Text foodText;

    // Used to store player food points total during level
    private int food;

    // Used to store a reference to the Player's animator component
    private Animator animator;

    // Used to store location of screen touch origin for mobile controls (initially evaluates to false because off the screen)
    private Vector2 touchOrigin = -Vector2.one;

    protected override void Start ()
    {
        // Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();

        // Get the current food point total stored in GameManager.instance between levels
        food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: " + food;

        // Call the Start function of the MovingObject base class
        base.Start();
    }

    private void OnDisable ()
    {
        // When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level
        GameManager.instance.playerFoodPoints = food;
    }

    void Update()
    {
        // If it's not the player's turn, exit the function
        if (!GameManager.instance.playersTurn) return;

        // Horizontal and vertical move directions
        int horizontal = 0;
        int vertical = 0;

#if UNITY_STANDALONE || UNITY_WEBPLAYER

        // Get inputs from the input manager, round them to integers and store them
        horizontal = (int) (Input.GetAxisRaw("Horizontal"));
        vertical = (int) (Input.GetAxisRaw("Vertical"));
        
        if (horizontal != 0)
            // If moving horizontally, set vertical to zero (to prevent the Player from moving diagonally)
            vertical = 0;

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

        // Check if Input has registered more than zero touches
        if (Input.touchCount > 0)
        {
            // Store the first touch detected
            Touch myTouch = Input.touches[0];

            if (myTouch.phase == TouchPhase.Began)
                touchOrigin = myTouch.position;

            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;

                // Calculate the difference between the beginning and end of the touch on the x and y axis
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;

                // Set touchOrigin.x to -1 so that the else if statement will evaluate false and not repeat immediately.
                touchOrigin.x = -1;

                // Check if the difference along the x axis is greater than the difference along the y axis.
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    // If x is greater than zero, set horizontal to 1, otherwise set it to -1
                    horizontal = x > 0 ? 1 : -1;
                else
                    // If y is greater than zero, set horizontal to 1, otherwise set it to -1
                    vertical = y > 0 ? 1 : -1;
            }
        }
#endif
        //Check if we have a non-zero value for horizontal or vertical
        if (horizontal != 0 || vertical != 0)
        {
            // Call AttemptMove passing in the generic parameter Wall to interact with and the direction
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        // Every time the Player moves, subtract 1 from food points total
        food--;
        foodText.text = "Food: " + food;

        // Call the AttemptMove method of the MovingObject base class
        base.AttemptMove<T>(xDir, yDir);

        // Hit allows us to reference the result of the Linecast done in Move
        RaycastHit2D hit;
        
        CheckIfGameOver();

        // Set the playersTurn boolean of GameManager to false
        GameManager.instance.playersTurn = false;
    }

    protected override void OnCantMove <T> (T component)
    {
        // Set hitWall to equal the component passed in as a parameter
        Wall hitWall = component as Wall;

        // Call the DamageWall function of the Wall we are hitting.
        hitWall.DamageWall(wallDamage);

        // Set the attack trigger of the Player's animation Controller
        animator.SetTrigger("playerChop");
    }

    private void OnTriggerEnter2D (Collider2D otherObject)
    {
        if (otherObject.tag == "Exit")
        {
            // If the Player collided with the Exit, invoke the Restart function with a delay of restartLevelDelay
            Invoke("Restart", restartLevelDelay);

            // Disable the player object since level is over
            enabled = false;
        }

        else if (otherObject.tag == "Food" || otherObject.tag == "Soda")
        {
            int pointsToAdd = (otherObject.tag == "Food" ? pointsPerFood : pointsPerSoda);

            // Add pointsPerFood or pointsPerSoda to the Player's current food total depending on the object
            food += pointsToAdd;

            foodText.text = "+" + pointsToAdd + " Food: " + food;

            // Disable the object
            otherObject.gameObject.SetActive(false);
        }
    }

    private void Restart ()
    {
        // Load the next scene, which is always Main because levels are being generated procedurally
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseFood (int loss)
    {
        // Set the hit trigger of the Player's animation Controller
        animator.SetTrigger("playerHit");

        // Subtract lost food points from the Player's total food
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;

        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
            // If total food points is less than or equal to zero, call the GameOver function of GameManager
            GameManager.instance.GameOver();
    }
}