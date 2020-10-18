using Mirror;
using System;
using UnityEngine;

public class Food : NetworkBehaviour
{
    NetworkMatchChecker networkMatchChecker;

    public void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    [Server]
    public void SetMatchId(Guid _matchId)
    {
        networkMatchChecker.matchId = _matchId;
    }

    [Server]
    public void GetEaten()
    {
        Destroy(gameObject);
    }
}