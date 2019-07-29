using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitHeal : MonoBehaviour
{
    // The 'Player' object.
    public Player player;

    // Start is called before the first frame update.
    void Start()
    {
        //Finds the player object to reference it.
        player = FindObjectOfType<Player>();
    }

    // Finds the player attack (int pAttack) and returns it.
    public int PAttack()
    {
        return player.pAttack;
    }

    // Attack for an enemy when the script is a component of that enemy (default is 2).
    [SerializeField] int attack = 2;

    // Decreases player health based on the enemy's attack.
    public void DamageTaken()
    {
        player.pHealth -= attack;
    }
}
