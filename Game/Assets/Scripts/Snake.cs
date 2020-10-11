using Mirror;
using UnityEngine;

public class Snake : NetworkBehaviour
{

    public void Update()
    {
        if (isServer)
        {
            Move();
        }
    }

    public void Control()
    {

    }

    public void Move()
    {
        transform.position += Vector3.forward * Random.Range(-10f, 10f) * Time.deltaTime;
    }

    [ClientRpc]
    public void ClientMove()
    {

    }
}
