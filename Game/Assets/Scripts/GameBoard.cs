using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoard : NetworkBehaviour
{
    NetworkMatchChecker networkMatchChecker;
    public List<Player> players = new List<Player>();

    public void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
        InitPlayers();
    }

    public void SetMatchId(Guid _matchId)
    {
        networkMatchChecker.matchId = _matchId;
    }

    public void InitPlayers()
    {
        players = FindObjectsOfType<Player>().OrderBy(x => x.playerIndex).ToList();
    }

    public void SpawnSnake(int _playerIndex)
    {
        Debug.Log("Spawning snake from game board " + GameManager.instance.players);
        Tuple<int, int> loc = GetFreeSpawnLocation(_playerIndex);

        players.Find(x => x.playerIndex == _playerIndex).SpawnSnake();
    }

    public Tuple<int, int> GetFreeSpawnLocation(int _playerIndex)
    {
        return new Tuple<int, int>(0, _playerIndex);
    }
}
