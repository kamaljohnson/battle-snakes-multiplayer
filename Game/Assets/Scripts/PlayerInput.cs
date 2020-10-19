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
    public PlayerAction playerAction;

    public DirectionHelper.Directions inputDirection;
    private DirectionHelper.Directions tempInputDirection;
    public bool directionChange;

    public int playerIndex;

    public void Awake()
    {
        playerAction = new PlayerAction();
    }

    void OnEnable()
    {
        playerAction.Enable();
    }

    void OnDisable()
    {
        playerAction.Disable();
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
            SerndInputDirectionToServer(tempInputDirection);
        }
    }

    void HandleMovementInput()
    {
        Vector2 input = playerAction.Main.Move.ReadValue<Vector2>();

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

        directionChange = true;
    }

    [Command]
    public void SerndInputDirectionToServer(DirectionHelper.Directions direction)
    {
        GetComponent<Player>().localSnake.ChangeNextDirection(direction);
    }
}
