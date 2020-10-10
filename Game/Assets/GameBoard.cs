using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : NetworkBehaviour
{

    List<Snake> snakes = new List<Snake>();

    public void SpawnSnake(int _playerIndex)
    {
        Debug.Log("Spawning snake from game board");
        Tuple<int, int> loc = GetFreeSpawnLocation(_playerIndex);

        NetworkServer.Spawn();

        UpdateBoardSnakes();
    }

    public Tuple<int, int> GetFreeSpawnLocation(int _playerIndex)
    {
        return new Tuple<int, int>(0, _playerIndex);
    }

    [ClientRpc]
    public void UpdateBoardSnakes()
    {
        Debug.Log("Update board snakes");
    }
}
