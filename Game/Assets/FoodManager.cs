using Mirror;
using System;
using UnityEngine;

public class FoodManager : NetworkBehaviour
{

    NetworkMatchChecker networkMatchChecker;

    public GameObject foodPrefab;
    public static FoodManager instance;

    public void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();

        if (isServer) instance = this;
    }

    [Server]
    public void SetMatchId(Guid _matchId)
    {
        networkMatchChecker.matchId = _matchId;
    }

    [Server]
    public void SpawnFood(int quantity = 1)
    {
        GameObject _newFood = Instantiate(foodPrefab, GameBoard.instance.GetFreeSpawnLocation(), Quaternion.identity);
        _newFood.GetComponent<Food>().SetMatchId(networkMatchChecker.matchId);

        NetworkServer.Spawn(_newFood);
        quantity--;

        if(quantity > 0)
        {
            SpawnFood(quantity - 1);
        }
    }
}
