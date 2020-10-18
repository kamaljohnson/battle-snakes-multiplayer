using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

[SerializeField]
public static class DirectionHelper
{
    public enum Directions
    {
        Forward,
        Back,
        Right,
        Left
    }

    public static Vector3 GetVector3ForDirection(Directions direction)
    {
        switch (direction)
        {
            case Directions.Forward:
                return Vector3.forward;
            case Directions.Back:
                return Vector3.back;
            case Directions.Right:
                return Vector3.right;
            case Directions.Left:
                return Vector3.left;
        }

        // this block is never reached
        return Vector3.zero;
    }
}

public class PlayerInput : NetworkBehaviour
{
    public bool isLocal = false;
    public InputAction wasd;
    public DirectionHelper.Directions inputDirection;
    private DirectionHelper.Directions tempInputDirection;
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
            tempInputDirection = input.x > 0 ? DirectionHelper.Directions.Right : DirectionHelper.Directions.Left; 

        } else if (Mathf.Abs(input.y) > 0)
        {
            tempInputDirection = input.y > 0 ? DirectionHelper.Directions.Forward : DirectionHelper.Directions.Back;
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
            case DirectionHelper.Directions.Forward:
                if(inputDirection == DirectionHelper.Directions.Back)
                {
                    directionChange = false;
                    return;
                }
                break;
            case DirectionHelper.Directions.Back:
                if (inputDirection == DirectionHelper.Directions.Forward)
                {
                    directionChange = false;
                    return;
                }
                break;
            case DirectionHelper.Directions.Right:
                if (inputDirection == DirectionHelper.Directions.Left)
                {
                    directionChange = false;
                    return;
                }
                break;
            case DirectionHelper.Directions.Left:
                if (inputDirection == DirectionHelper.Directions.Right)
                {
                    directionChange = false;
                    return;
                }
                break;
        }
        directionChange = true;
    }

    [Command]
    public void SerndInputDirectionToServer(DirectionHelper.Directions direction)
    {
        GetComponent<Player>().localSnake.ChangeNextDirection(direction);
    }
}
