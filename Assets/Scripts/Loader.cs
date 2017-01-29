using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{
    // Take care of the singleton pattern
    public GameObject gameManager;

	private void Awake ()
    {
        if (!GameManager.instance)
            Instantiate(gameManager);
	}
}
