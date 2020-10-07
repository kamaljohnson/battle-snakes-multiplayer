﻿using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    private void Update()
    {
        if (localPlayer)
        {
            transform.position += Vector3.forward * Time.deltaTime * Input.GetAxis("Horizontal");
        }
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            Debug.Log($"Spawning local player UI Prefab"); ;
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

    public void BeginGame()
    {
        CmdBeginGame();
    }

    [Command]
    void CmdBeginGame()
    {
        MatchMaker.instance.BeginGame(matchID);
        Debug.Log($"<color=red>Game Beginning</color>");
    }

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


}