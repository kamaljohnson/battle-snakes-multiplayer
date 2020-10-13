using Mirror;
using UnityEngine;

public class Snake : NetworkBehaviour
{
    public SnakeTail head;

    public GameObject tailPrefab;

    public int speed;

    public Direction movementDirection;
    public Direction nextDirection;

    public int pendingTailCount;

    [Server]
    public void InitMovement(Direction direction)
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
    public void ChangeNextDirection(Direction direction)
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
        GameObject tail = Instantiate(tailPrefab);

        tail.transform.position = head.GetNewEndTailLocation(tail.GetComponent<SnakeTail>());
        tail.GetComponent<SnakeTail>().speed = speed;
        NetworkServer.Spawn(tail);

        ClientSetSpawnedTailToEnd(tail.GetComponent<SnakeTail>().id);
        pendingTailCount = count - 1;
    }

    [ClientRpc]
    public void ClientSetSpawnedTailToEnd(int id)
    {
        head.ClientSetSpawnedTailToEnd(id);
    }

    [ClientRpc]
    public void ClientChangeHeadDirection(Direction direction)
    {
        movementDirection = direction;
        head.ChangeDirection(movementDirection);
    }

    [ClientRpc]
    public void ClientInitMovement(Direction direction)
    {
        head.InitMovement(direction);
    }

    [ClientRpc]
    public void ClientStartMoving()
    {
        head.StartMoving();
    }

}
