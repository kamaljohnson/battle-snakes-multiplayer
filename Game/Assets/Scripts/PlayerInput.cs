using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

[SerializeField]
public enum Direction
{
    Forward,
    Back,
    Right,
    Left
}

public class PlayerInput : NetworkBehaviour
{
    public bool isLocal = false;
    public InputAction wasd;
    public Direction inputDirection;
    private Direction tempInputDirection;
    public bool directionChange;

    public int playerIndex;

    void OnEnable()
    {
        wasd.Enable();
    }

    void OnDisable()
    {
        wasd.Disable();
    }

    public void Start()
    {
        playerIndex = GetComponent<Player>().playerIndex;
        if (!isLocal) return;
        SerndInputDirectionToServer(inputDirection);
    }

    void Update()
    {
        if (!isLocal) return;
        HandleMovementInput();
        CheckDirectionChange();
        if(directionChange)
        {
            directionChange = false;
            inputDirection = tempInputDirection;
            SerndInputDirectionToServer(inputDirection);
        }
    }

    void HandleMovementInput()
    {
        Vector2 input = wasd.ReadValue<Vector2>();

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            tempInputDirection = input.x > 0 ? Direction.Right : Direction.Left; 

        } else if (Mathf.Abs(input.y) > 0)
        {
            tempInputDirection = input.y > 0 ? Direction.Forward : Direction.Back;
        } else
        {
            //Do noting
        }
    }

    void CheckDirectionChange()
    {
        if (tempInputDirection == inputDirection)   
        {
            directionChange = false;
            return;
        }

        switch (tempInputDirection)
        {
            case Direction.Forward:
                if(inputDirection == Direction.Back)
                {
                    directionChange = false;
                    return;
                }
                break;
            case Direction.Back:
                if (inputDirection == Direction.Forward)
                {
                    directionChange = false;
                    return;
                }
                break;
            case Direction.Right:
                if (inputDirection == Direction.Left)
                {
                    directionChange = false;
                    return;
                }
                break;
            case Direction.Left:
                if (inputDirection == Direction.Right)
                {
                    directionChange = false;
                    return;
                }
                break;
        }
        directionChange = true;
    }

    [Command]
    public void SerndInputDirectionToServer(Direction direction)
    {
        Debug.Log("Player : input direction changed to : " + direction);

        GetComponent<Player>().localSnake.nextDirection = direction;
    }
}
