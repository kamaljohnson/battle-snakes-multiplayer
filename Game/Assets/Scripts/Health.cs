using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SyncVar] public int health;
    [SyncVar] public bool isDead;

    public void GetHit(int damage = 1)
    {
        Debug.Log("Hit");
        health -= damage;
        CheckIfDead();
    }

    public void Revive(int energy)
    {
        health += energy;
    }

    public void CheckIfDead()
    {
        isDead = health > 0 ? false : true;
        if (isDead)
        {
            Death();
        }
    }

    public void Death()
    {
        Debug.Log("The snake died");
    }
}
