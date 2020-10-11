using Mirror;
using UnityEngine;

public class Snake : NetworkBehaviour
{

    public void Update()
    {

    }

    public void HandleInput()
    {

    }

    // Called from the local player to control the movement of the snake
    public void Control(Direction direction)
    {
        ServerControl(direction);
    }

    [Command]
    public void ServerControl(Direction direction)
    {
        Debug.Log("control received at the server : " + direction);
    }

    public void Move()
    {
        TargetMove();
    }

    [TargetRpc]
    public void TargetMove()
    {

    }
}
