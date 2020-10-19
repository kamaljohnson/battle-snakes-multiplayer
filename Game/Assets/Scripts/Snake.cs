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
        // check if the next direction can be in the provided direction
        switch (direction)
        {
            case DirectionHelper.Directions.Forward:
                if (movementDirection == DirectionHelper.Directions.Back)
                {
                    return;
                }
                break;
            case DirectionHelper.Directions.Back:
                if (movementDirection == DirectionHelper.Directions.Forward)
                {
                    return;
                }
                break;
            case DirectionHelper.Directions.Right:
                if (movementDirection == DirectionHelper.Directions.Left)
                {
                    return;
                }
                break;
            case DirectionHelper.Directions.Left:
                if (movementDirection == DirectionHelper.Directions.Right)
                {
                    return;
                }
                break;
        }

        nextDirection = direction;
    }

    [Server]
    public void HeadReachedNextLoc()
    {
        movementDirection = nextDirection;
        
        head.ChangeDirection(movementDirection);

        ClientChangeHeadDirection(movementDirection);
        
        if (pendingTailCount > 0) SpawnTail(pendingTailCount, false);
    }

    [Server]
    public void SpawnTail(int count = 1, bool ext = true)
    {
        if (ext)
        {
            pendingTailCount += count;
            return;
        }

        GameObject tailObj = Instantiate(tailPrefab);

        tailObj.transform.position = head.GetNewEndTailLocation(tailObj.GetComponent<SnakeTail>());
        tailObj.GetComponent<SnakeTail>().speed = speed;
        
        SnakeTail tail = tailObj.GetComponent<SnakeTail>();
        tail.SetMatchId(networkMatchChecker.matchId);
        tail.playerIndex = playerIndex;
        
        NetworkServer.Spawn(tailObj);
        
        ClientSetSpawnedTailToEnd(playerIndex);
        
        pendingTailCount -= 1;
    }

    [Server]
    public void SpawnInitTail()
    {
        SpawnTail(initialSize);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        switch (other.tag)
        {
            case "Food":
                Eat(other.gameObject);
                break;
            case "Snake":
                if (other.GetComponent<SnakeTail>().isMoving)
                {
                    HitWall();
                }
                break;
        }
    }

    [Server]
    public void Eat(GameObject food)
    {
        FoodManager.instance.EatFood(food);
        FoodManager.instance.SpawnFood();
        SpawnTail();
    }

    [Server]
    public void HitWall()
    {
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
