using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakeTail : NetworkBehaviour
{
    [SyncVar] public int playerIndex;

    public bool isAttached;     //Only used at the client side
    
    [SyncVar] public int speed;

    public static List<SnakeTail> allTails = new List<SnakeTail>();

    public GameObject tailPrefab;

    public Vector2 nextLoc;
    private Vector2 prevLoc;
    public SnakeTail nextTail;

    public bool isHead;
    private Snake snake;

    [SyncVar] public bool isMoving;

    public Direction movementDirection;

    NetworkMatchChecker networkMatchChecker;

    private void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();

        if (!isServer) allTails.Add(this);

        if (isHead)
        {
            snake = GetComponent<Snake>();
            isAttached = true;
        }
    }

    [Server]
    public void SetMatchId(Guid _matchId)
    {
        networkMatchChecker.matchId = _matchId;
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
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(nextLoc.x, transform.position.y, nextLoc.y), speed * Time.deltaTime);

        if (isServer && isHead)
        {
            CheckNextLocReached();
        }
    }

    [Server] // Only called if the tail snake head
    public void CheckNextLocReached()
    {
        if (Vector3.Distance(transform.position, new Vector3(nextLoc.x, transform.position.y, nextLoc.y)) <= 0.01f) 
        {
            snake.HeadReachedNextLoc();
        }
    }

    public void InitMovement(Direction direction)
    {
        movementDirection = direction;
        nextLoc = new Vector2(transform.position.x, transform.position.z);
        prevLoc = nextLoc;
        SetNewNextLoc();

        if(nextTail != null)
        {
            nextTail.InitMovement(direction);
        }
    }

    public void ChangeDirection(Direction direction)
    {
        if (!isMoving)
        {
            InitMovement(direction);
            StartMoving();
            return;
        } //If the tail is newly created then start moving

        transform.position = new Vector3(nextLoc.x, transform.position.y, nextLoc.y);
        
        if(nextTail != null)
        {
            nextTail.ChangeDirection(movementDirection);
        }
        movementDirection = direction;
        
        SetNewNextLoc();

    }

    public void SetNewNextLoc()
    {
        prevLoc = nextLoc;
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

    [Server]
    public Vector3 GetNewEndTailLocation(SnakeTail tail)
    {
        if(nextTail != null)
        {
            return nextTail.GetNewEndTailLocation(tail);
        } else
        {
            nextTail = tail;
            return (new Vector3(prevLoc.x, transform.position.y, prevLoc.y));
        }
    }

    [Client]
    public void ClientSetSpawnedTailToEnd(int _playerIndex)
    {
        if(nextTail != null)
        {
            nextTail.ClientSetSpawnedTailToEnd(_playerIndex);
        } else
        {
            Debug.Log("SNAKE SIZE: " + allTails.Count);
            nextTail = allTails.Find(x => (x.playerIndex == _playerIndex) && !x.isAttached);
            nextTail.isAttached = true;
        }
    }

}
