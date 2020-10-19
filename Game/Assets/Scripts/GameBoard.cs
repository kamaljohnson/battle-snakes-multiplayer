using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoard : NetworkBehaviour
{
    NetworkMatchChecker networkMatchChecker;
    public List<Player> players = new List<Player>();
    public List<Snake> snakes = new List<Snake>();

    public Vector2 boardSize;
    public float spawnHeightOffset;

    public static GameBoard instance;

    public void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    [Server]
    public void SetMatchId(Guid _matchId)
    {
        networkMatchChecker.matchId = _matchId;
        instance = this;
    }

    [Server]
    public void InitPlayers()
    {
        players = FindObjectsOfType<Player>().OrderBy(x => x.playerIndex).ToList();
    }

    [Server]
    public void AddPlayer(Player _player)
    {
        players.Add(_player);
    }

    [Server]
    public void SpawnSnake(Player _player)
    {
        Vector3 loc = GetFreeSpawnLocation();

        snakes.Add(_player.SpawnSnake(loc, DirectionHelper.Directions.Forward));
    }

    [Server]
    public Vector3 GetFreeSpawnLocation()
    {
        //TODO: change the random function to the actual get location
        return new Vector3(UnityEngine.Random.Range(-(int)boardSize.x, (int)boardSize.x - 1),
            transform.position.y + spawnHeightOffset,
            UnityEngine.Random.Range(-(int)boardSize.y, (int)boardSize.y - 1));
    }
}
