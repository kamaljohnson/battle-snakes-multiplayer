using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public List<Player> players = new List<Player>();

    public GameObject gameBoardPrefab;

    GameBoard gameBoard;
    NetworkMatchChecker networkMatchChecker;

    // Start is called before the first frame updatesss
    void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
        instance = this;
    }

    public void SetMatchId(Guid _matchId)
    {
        networkMatchChecker.matchId = _matchId;
    }

    public void InitGameBoard(Guid matchId)
    {
        GameObject gameBoardObj = Instantiate(gameBoardPrefab);

        gameBoard = gameBoardObj.GetComponent<GameBoard>();
        gameBoard.SetMatchId(matchId);

        NetworkServer.Spawn(gameBoardObj);
        
    }

    public void AddPlayer(Player _player)
    {
        Debug.Log($"Adding player");
        players.Add(_player);
        gameBoard.AddPlayer(_player);
        Debug.Log($"After adding player : " + players.Count);
    }

    public void RemovePlayer(Player _player)
    {
        Debug.Log($"Removeing player");
        players.Remove(_player);

    }

    [Server]
    public void SpawnAllPlayerSnakes()
    {
        Debug.Log($"Spawning snake: GameManager");
        foreach (var _player in players)
        {
            gameBoard.SpawnSnake(_player);
        }
    }
}
