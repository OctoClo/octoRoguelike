using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;

	private void Awake ()
    {
        if (!GameManager.instance)
            Instantiate(gameManager);
	}
}
