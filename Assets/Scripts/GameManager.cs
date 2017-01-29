using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public BoardManager boardScript;

    private int level = 3;    

	private void Awake ()
    {
        if (!instance)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();
        InitGame();
	}

    private void InitGame()
    {
        boardScript.SetupScene(level);
    }

    private void Update ()
    {
	
	}
}
