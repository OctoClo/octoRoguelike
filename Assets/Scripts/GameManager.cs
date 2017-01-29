using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Declare Managers
    public static GameManager instance = null; // Singleton pattern
    public BoardManager boardScript;

    // Player's variables
    public int playerFoodPoints = 100;
    [HideInInspector]
    public bool playersTurn = true;

    // Delay between each Player turn
    public float turnDelay = 0.1f;

    // Time to wait before starting level, in seconds
    public float levelStartDelay = 2f;

    private Text levelText;
    private GameObject levelImage;

    private int level = 1;

    private bool doingSetup = true;

    // Enemy's variables
    private List<Enemy> enemies;
    private bool enemiesMoving;

    private void Awake ()
    {
        // Singleton pattern
        if (!instance)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // Set this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        enemies = new List<Enemy>();

        // Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();

        InitGame();
	}

    void OnLevelWasLoaded(int index)
    {
        level++;

        InitGame();
    }

    private void InitGame ()
    {
        doingSetup = true;

        // Get a reference to our image LevelImage by finding it by name
        levelImage = GameObject.Find("LevelImage");

        // Get a reference to our text LevelText's text component by finding it by name
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        levelText.text = "Day " + level;

        levelImage.SetActive(true);

        // Call the HideLevelImage function with a delay in seconds of levelStartDelay
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();

        // Set up the scene with the level created before
        boardScript.SetupScene(level);
    }

    void HideLevelImage()
    {
        levelImage.SetActive(false);

        doingSetup = false;
    }

    public void GameOver ()
    {
        // Set levelText to display number of levels passed and game over message
        levelText.text = "After " + level + " days, you starved.";

        levelImage.SetActive(true);

        // Disable the GameManager
        enabled = false;
    }

    private void Update ()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            // If it's the player's turn or if the enemies are already moving or if we are doing setup, return
            return;

        else
            // Start moving enemies
            StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList (Enemy script)
    {
        enemies.Add(script);
    }

    // Coroutine to move enemies in sequence
    IEnumerator MoveEnemies ()
    {
        enemiesMoving = true;

        // Wait for turnDelay seconds
        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
        {
            // If there are no enemies (ie in first level), wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0 ; i < enemies.Count ; i++)
        {
            // For each enemy, call MoveEnemy
            enemies[i].MoveEnemy();

            // Wait for Enemy's moveTime before moving next Enemy
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}
