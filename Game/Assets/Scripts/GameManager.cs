using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;


    List<Player> players = new List<Player>();
    int [,] gameBoard;

    // Start is called before the first frame update
    void Start()
    {
        if (!isServer) return;

        instance = this;
    }

    public void AddPlayer(Player _player)
    {
        Debug.Log($"Adding player");
        players.Add(_player);
    }

    public void SpawnAllPlayerSnakes()
    {
        Debug.Log($"Spawning snake: GameManager");
        foreach (var _localPlayer in players)
        {
            foreach(var _player in players)
            {
                
            }
        }
    }

    public Tuple<int, int> GetFreeLocationInGameBoard()
    {
        //TODO: get a non occupied point in game board
        return new Tuple<int, int>(0, 0);
    }
}
