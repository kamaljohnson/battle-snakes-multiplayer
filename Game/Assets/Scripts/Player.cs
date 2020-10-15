using Mirror;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{

    public static Player localPlayer;
    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex;
    
    NetworkMatchChecker networkMatchChecker;

    [SyncVar] public Match currentMatch;

    [SerializeField] GameObject playerLobbyUI;

    [SerializeField] public GameObject snakePrefab;
    public Snake localSnake;

    void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            Debug.Log($"Spawning local player UI Prefab");
            localPlayer = this;
        }
        else
        {
            Debug.Log($"Spawning other player UI Prefab");
            playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab(this);
        }
    }

    public override void OnStopClient()
    {
        Debug.Log($"Client Stopped");
        ClientDisconnect();
    }

    public override void OnStopServer()
    {
        Debug.Log($"Client Stopped on Server");
        ServerDisconnect();
    }

    /* 
        HOST MATCH
    */

    public void HostGame(bool publicMatch)
    {
        string matchID = MatchMaker.GetRandomMatchID();
        CmdHostGame(matchID, publicMatch);
    }

    [Command]
    void CmdHostGame(string _matchID, bool publicMatch)
    {
        matchID = _matchID;
        if (MatchMaker.instance.HostGame(_matchID, gameObject, publicMatch, out playerIndex))
        {
            Debug.Log($"<color=green>Game hosted successfully</color>");
            networkMatchChecker.matchId = _matchID.ToGuid();
            TargetHostGame(true, _matchID, playerIndex);
        }
        else
        {
            Debug.Log($"<color=red>Game hosted failed</color>");
            TargetHostGame(false, _matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetHostGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log($"MatchID: {matchID} == {_matchID}");
        UILobby.instance.HostSuccess(success, _matchID);
    }

    /* 
        JOIN MATCH
    */

    public void JoinGame(string _inputID)
    {
        CmdJoinGame(_inputID);
    }

    [Command]
    void CmdJoinGame(string _matchID)
    {
        matchID = _matchID;
        if (MatchMaker.instance.JoinGame(_matchID, gameObject, out playerIndex))
        {
            Debug.Log($"<color=green>Game Joined successfully</color>");
            networkMatchChecker.matchId = _matchID.ToGuid();
            TargetJoinGame(true, _matchID, playerIndex);
        }
        else
        {
            Debug.Log($"<color=red>Game Joined failed</color>");
            TargetJoinGame(false, _matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetJoinGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log($"MatchID: {matchID} == {_matchID}");
        UILobby.instance.JoinSuccess(success, _matchID);
    }

    /* 
        DISCONNECT
    */

    public void DisconnectGame()
    {
        CmdDisconnectGame();
    }

    [Command]
    void CmdDisconnectGame()
    {
        ServerDisconnect();
    }

    void ServerDisconnect()
    {
        MatchMaker.instance.PlayerDisconnected(this, matchID);
        NetworkServer.UnSpawn(localSnake.gameObject);
        GameManager.instance.RemovePlayer(this);
        RpcDisconnectGame();
        networkMatchChecker.matchId = string.Empty.ToGuid();
    }

    [ClientRpc]
    void RpcDisconnectGame()
    {
        ClientDisconnect();
    }

    void ClientDisconnect()
    {
        if (playerLobbyUI != null)
        {
            Destroy(playerLobbyUI);
        }
    }

    /* 
        SEARCH MATCH
    */

    [Client]
    public void SearchGame()
    {
        Debug.Log("Player: SearchGame()");
        CmdSearchGame();
    }

    [Command]
    void CmdSearchGame()
    {
        Debug.Log("Player: CmdSerchGame()");
        if (MatchMaker.instance.SearchGame(gameObject, out playerIndex, out matchID))
        {
            Debug.Log($"<color=green>Game Found Successfully</color>");
            networkMatchChecker.matchId = matchID.ToGuid();
            TargetSearchGame(true, matchID, playerIndex);
        }
        else
        {
            Debug.Log($"<color=red>Game Search Failed</color>");
            TargetSearchGame(false, matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetSearchGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log($"MatchID: {matchID} == {_matchID} | {success}");
        UILobby.instance.SearchGameSuccess(success, _matchID);
    }

    /* 
        BEGIN MATCH
    */
    [Client]
    public void BeginGame()
    {
        CmdBeginGame();
    }

    [Command]
    void CmdBeginGame()
    {
        Debug.Log($"<color=red>GAME BEGINING</color> MATCH ID: " + matchID.ToGuid());
        MatchMaker.instance.BeginGame(matchID);
    }

    [Server]
    public void StartGame()
    { //Server
        TargetBeginGame();
    }

    [TargetRpc]
    void TargetBeginGame()
    {
        Debug.Log($"MatchID: {matchID} | Beginning");
        //Additively load game scene
        UILobby.instance.DisableLobbyUI();
        SceneManager.LoadScene(2, LoadSceneMode.Additive);

    }

    // Game
    [Server]
    public Snake SpawnSnake(Tuple<int, int> loc, DirectionHelper.Directions movementDirection)
    {
        Debug.Log("SPAWN SNAKE : PLAYER INDEX : " + playerIndex + " MATCH ID: " + networkMatchChecker.matchId);
        GameObject localSnakeObj = Instantiate(snakePrefab);
        localSnakeObj.transform.position = new Vector3(loc.Item1, localSnakeObj.transform.position.y, loc.Item2);
        
        localSnake = localSnakeObj.GetComponent<Snake>();
        localSnake.SetMatchId(networkMatchChecker.matchId);

        localSnake.playerIndex = playerIndex;

        NetworkServer.Spawn(localSnakeObj);
        
        TargetAddInputHandler();


        localSnake.InitMovement(movementDirection);
        localSnake.StartMoving();

        localSnake.SpawnTail(20);

        return localSnake;
    }

    [TargetRpc]
    public void TargetAddInputHandler()
    {
        gameObject.GetComponent<PlayerInput>().isLocal = true;
    }
}