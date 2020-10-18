using Mirror;
using System;
using UnityEngine;

public class Snake : NetworkBehaviour
{
    public SnakeTail head;
    [SyncVar] public int playerIndex;

    public GameObject tailPrefab;

    public int speed;

    public int initialSize;

    public DirectionHelper.Directions movementDirection;
    public DirectionHelper.Directions nextDirection;

    public int pendingTailCount;

    public Health health;

    NetworkMatchChecker networkMatchChecker;

    public void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
        health = GetComponent<Health>();
    }

    [Server]
    public void SetMatchId(Guid _matchId)
    {
        networkMatchChecker.matchId = _matchId;
    }

    [Server]
    public void InitMovement(DirectionHelper.Directions direction)
    {
        head.speed = speed;
        head.InitMovement(direction);

        ClientInitMovement(direction);
    }

    [Server]
    public void StartMoving()
    {
        head.StartMoving();

        ClientStartMoving();
    }

    [Server]
    public void ChangeNextDirection(DirectionHelper.Directions direction)
    {
        nextDirection = direction;
    }

    [Server]
    public void HeadReachedNextLoc()
    {
        movementDirection = nextDirection;
        head.ChangeDirection(movementDirection);

        ClientChangeHeadDirection(movementDirection);
        
        if (pendingTailCount > 0) SpawnTail(pendingTailCount);
    }

    [Server]
    public void SpawnTail(int count = 1)
    {
        GameObject tailObj = Instantiate(tailPrefab);

        tailObj.transform.position = head.GetNewEndTailLocation(tailObj.GetComponent<SnakeTail>());
        tailObj.GetComponent<SnakeTail>().speed = speed;
        
        SnakeTail tail = tailObj.GetComponent<SnakeTail>();
        tail.SetMatchId(networkMatchChecker.matchId);
        tail.playerIndex = playerIndex;
        
        NetworkServer.Spawn(tailObj);
        
        ClientSetSpawnedTailToEnd(playerIndex);

        pendingTailCount = count - 1;
    }

    [Server]
    public void SpawnInitTail()
    {
        SpawnTail(initialSize);
    }

    public void OnTriggerEnter(Collider other)
    {
        // if (!isServer) return;

        switch (other.tag)
        {
            case "Food":
                Eat(other.gameObject);
                break;
        }
    }

    [Server]
    public void Eat(GameObject food)
    {
        SpawnTail();
        FoodManager.instance.RemoveFood(food);
        FoodManager.instance.SpawnFood();
    }

    [Server]
    public void HitWall()
    {
        Debug.Log("HIT WALL");
        health.GetHit();
        if (health.isDead)
        {
            head.StopMoving();
        }
    }

    [ClientRpc]
    public void ClientSetSpawnedTailToEnd(int _playerIndex)
    {
        head.ClientSetSpawnedTailToEnd(_playerIndex);
    }

    [ClientRpc]
    public void ClientChangeHeadDirection(DirectionHelper.Directions direction)
    {
        movementDirection = direction;
        head.ChangeDirection(movementDirection);
    }

    [ClientRpc]
    public void ClientInitMovement(DirectionHelper.Directions direction)
    {
        head.InitMovement(direction);
    }

    [ClientRpc]
    public void ClientStartMoving()
    {
        head.StartMoving();
    }
}
