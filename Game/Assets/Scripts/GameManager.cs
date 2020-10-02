using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    List<Player> players = new List<Player>();
    public static GameManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (!isServer) return;

        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlayer(Player _player)
    {
        Debug.Log($"Adding player");
        players.Add(_player);
    }
}
