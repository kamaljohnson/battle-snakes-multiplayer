using Mirror;
using UnityEngine;

public class SnakeTail : NetworkBehaviour
{
    public Vector2 nextLoc;
    public SnakeTail nextTail;

    public bool isHead;
    private Snake snake;

    [SyncVar] public bool isMoving;

    public Direction movementDirection;

    private void Awake()
    {
        if (isHead)
        {
            snake = GetComponent<Snake>();
        }
    }

    void Update()
    {
        if (!isMoving) return;
        Move();
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public void Move()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(nextLoc.x, transform.localPosition.y, nextLoc.y), 5 * Time.deltaTime);

        if (isServer && isHead)
        {
            CheckNextLocReached();
        }
    }

    [Server] // Only called if the tail snake head
    public void CheckNextLocReached()
    {
        if (Vector3.Distance(transform.localPosition, new Vector3(nextLoc.x, transform.localPosition.y, nextLoc.y)) <= 0.01f) 
        {
            snake.HeadReachedNextLoc();
        }
    }

    public void InitMovement(Direction direction)
    {
        movementDirection = direction;
        nextLoc = new Vector2(transform.localPosition.x, transform.localPosition.z);
        SetNewNextLoc();

        if(nextTail != null)
        {
            nextTail.InitMovement(direction);
        }
    }

    public void ChangeDirection(Direction direction)
    {
        transform.localPosition = new Vector3(nextLoc.x, transform.localPosition.y, nextLoc.y);
        
        if(nextTail != null)
        {
            nextTail.ChangeDirection(movementDirection);
        }
        movementDirection = direction;
        
        SetNewNextLoc();

    }

    public void SetNewNextLoc()
    {
        switch (movementDirection)
        {
            case Direction.Forward:
                nextLoc += new Vector2(0, 1);
                break;
            case Direction.Back:
                nextLoc += new Vector2(0, -1);
                break;
            case Direction.Right:
                nextLoc += new Vector2(1, 0);
                break;
            case Direction.Left:
                nextLoc += new Vector2(-1, 0);
                break;
        }
    }
}
