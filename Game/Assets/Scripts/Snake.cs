using Mirror;
using UnityEngine;

public class Snake : NetworkBehaviour
{
    public SnakeTail head;

    public Direction movementDirection;
    public Direction nextDirection;

    public void InitMovement(Direction direction)
    {
        head.InitMovement(direction);

        ClientInitMovement(direction);
    }

    public void StartMoving()
    {
        head.StartMoving();

        ClientStartMoving();
    }

    public void ChangeNextDirection(Direction direction)
    {
        nextDirection = direction;
    }

    public void HeadReachedNextLoc()
    {
        movementDirection = nextDirection;
        head.ChangeDirection(movementDirection);

        ClientChangeHeadDirection(movementDirection);
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
