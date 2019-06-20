using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerHandler : MonoBehaviour
{
    private GameManager gm;

    private void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void LoadGame()
    {
        gm.LoadGame();
    }
}
