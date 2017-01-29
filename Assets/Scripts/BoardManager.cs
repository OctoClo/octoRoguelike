using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    // Serializable class to hold the minimum and maximum number of each GameObject
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // Dimensions of the game board (outer walls not included)
    public int columns = 8;
    public int rows = 8;

    // Lower and upper limit for the random number of walls/food items per level
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);

    // Single GameObject exit
    public GameObject exit;

    // Arrays of GameObjects : we will choose one of the prefab's variations we want to spawn
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    // All spawned objects will be children of boardHolder to keep a clean hierarchy
    private Transform boardHolder;

    // A list of vector3 to track all the different possible positions on the gameboard (third parameter always equals to zero because of 2D)
    private List<Vector3> gridPositions = new List<Vector3>();

    private void InitialiseList ()
    {
        gridPositions.Clear();

        // Fill the list with all possible positions as vector3s, leaving a border to be sure levels are not impossible
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
                gridPositions.Add(new Vector3(x, y, 0f));
        }
    }

    private void BoardSetup ()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                // Choose a random tile from the array of floor tile prefabs and preparing to instantiate it
                GameObject objectToInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                // If we are in one of the outer walls position, choose a random tile from the array of outer wall tile prefabs
                if (x == -1 || x == columns || y == -1 || y == rows)
                    objectToInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                // Instantiate the object with the vector3 of its position (Quaternion.identity means no rotation) and cast it to a GameObject
                GameObject instance = Instantiate(objectToInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                // Set the instance's parent to boardHolder
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    private Vector3 RandomPosition ()
    {
        // Randomly choose an index in the gridPositions list
        int randomIndex = Random.Range(0, gridPositions.Count);

        // Get the vector3 at this index
        Vector3 randomPosition = gridPositions[randomIndex];

        // Remove the entry at randomIndex so it can't be re-used
        gridPositions.RemoveAt(randomIndex);

        // Return the randomly chosen vector3
        return randomPosition;
    }

    private void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        // Choose a random number of objects to instantiate within the minimum and maximum limits
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            // Choose a random position from our list of available positions gridPositions
            Vector3 randomPosition = RandomPosition();

            // Choose a random tile from the array of tile prefabs given
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            // Instantiate the object with the randomly chosen vector3 of its position
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene (int level)
    {
        // Create the floor and the outer walls of the game board (background)
        BoardSetup();

        InitialiseList();

        // Lay out walls and food items randomly
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        // Determine number of enemies based on current level number (logarithmic progression)
        int enemyCount = (int)Mathf.Log(level, 2f);

        // Lay out enemies randomly
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        // Instantiate the exit tile in the upper right hand corner of the game board
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
