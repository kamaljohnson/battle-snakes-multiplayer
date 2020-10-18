using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : NetworkBehaviour
{

    NetworkMatchChecker networkMatchChecker;

    public GameObject foodPrefab;
    public static FoodManager instance;

    public void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    [Server]
    public void SetMatchId(Guid _matchId)
    {
        instance = this;
        networkMatchChecker.matchId = _matchId;
    }

    [Server]
    public void SpawnFood(int quantity = 1)
    {
        GameObject _newFood = Instantiate(foodPrefab, GameBoard.instance.GetFreeSpawnLocation(), Quaternion.identity);
        _newFood.GetComponent<Food>().SetMatchId(networkMatchChecker.matchId);

        NetworkServer.Spawn(_newFood);

        if(quantity > 1)
        {
            SpawnFood(quantity - 1);
        }
    }

    [Server]
    public void EatFood(GameObject food)
    {
        food.GetComponent<Food>().GetEaten();
    }
}
