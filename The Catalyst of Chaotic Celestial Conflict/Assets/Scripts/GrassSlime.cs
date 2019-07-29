using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GrassSlime : MonoBehaviour
{
    // !Variables/Status!
    // The enemy's current health.
    [SerializeField] public int health = 4;
    // The enemy's movement speed.
    [SerializeField] int speed = 1;

    // !Cached Redferences!
    // Controls the object's physics.
    Rigidbody2D iRigidbody;
    // Collider that acts as the main body that will touch the ground and walls.
    CapsuleCollider2D iCapsule;
    // The animator controller that handles animation playing and switching.
    Animator iAnimator;
    // Controls the object's position, rotation and scale.
    Transform iTransform;
    // Script that determines damage taken by enemies and the player.
    HitHeal hitHeal;
    // Ground detection collider responsible for turning on the 'Sensor' object childed to the enemy.
    BoxCollider2D iBox;
    // Damage collider responsible for triggering 'EnemyHurt()' on the 'Sensor' object childed to the enemy.
    CapsuleCollider2D iCapsule2; 

    // Start is called before the first frame update.
    void Start()
    {
        // Gets the component assigned to the reference.
        iRigidbody = GetComponent<Rigidbody2D>();
        iCapsule = GetComponent<CapsuleCollider2D>();
        iAnimator = GetComponent<Animator>();
        iTransform = GetComponent<Transform>();
        iBox = GetComponentInChildren<BoxCollider2D>();
        iCapsule2 = GetComponentInChildren<CapsuleCollider2D>();
    }

    // !Methods!
    // Causes the enemy to start moving.
    private void Moving()
    {
        iRigidbody.velocity = new Vector2(speed, iRigidbody.velocity.y);
    }

    // Makes the enemy change directions when it's about to go off a ledge or after touching a wall.
    private void Turn()
    {
        if ((!iBox.IsTouchingLayers(LayerMask.GetMask("Ground"))) & (iRigidbody.velocity.y == 0)
            || iCapsule.IsTouchingLayers(LayerMask.GetMask("Wall")))
        {
            // Makes the enemy jump in the opposite direction really quickly to avoid falling off a ledge.
            iRigidbody.velocity = new Vector2(-Mathf.Sign(iRigidbody.velocity.x) * 5f, 1f);
            // Flips the sign off speed by multiplying it with -1.
            speed = -speed;
        }
    }

    // Plays the hurt animation and causes the enemy to fly back when hit.
    private void EnemyHurt()
    {
        iAnimator.SetTrigger("Hurt");
        iRigidbody.velocity = new Vector2(-(Mathf.Sign(transform.localScale.x) * 3f), 1.5f);
    }

    // Damage taken: Decreases enemy health when player's trigger collider touches enemy's trigger collider. 
    // Placed here instead of HitHeal.cs for more complex damage taken conditions.
    private void OnTriggerEnter2D(Collider2D player)
    {
        if (iCapsule2.IsTouchingLayers(LayerMask.GetMask("Player")))// Only takes damage if the player's collider triggered the enemy collider.
        {
            hitHeal = player.gameObject.GetComponent<HitHeal>();
            EnemyHurt();
            // Decreases health by 2 only not the player's 'pAttack' int.
            health -= 2;
        }
    }
    // Plays the death animation and deletes the enemy.
    private void Death()
    {
        iAnimator.SetTrigger("Death");
        // Turns off colliders so the enemy cannot hurt the player anymore.
        iCapsule.enabled = false;
        iBox.enabled = false;
        iCapsule2.enabled = false;
        // Stops the enemy from falling through the ground too fast.
        iRigidbody.velocity = new Vector2(iRigidbody.velocity.x, 0.06f);
        // Destroys the enemy object
        Destroy(gameObject, 0.1f);
    }

    // Sets the animation parameter bools as true or false and flips the sprite when changing directions.
    private void Animations()
    {
        if (!(iRigidbody.velocity.x == 0))
        {
            iAnimator.SetBool("Running", true);
        }
        else
        {
            iAnimator.SetBool("Running", false);
        }
        // Flips the enemy sprite to face the direction they are moving towards.
        if (!(iRigidbody.velocity.x == 0) & health > 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(iRigidbody.velocity.x), 1f);
        }
    }


    // Update is called once per frame.
    void Update()
    {
        Animations();

        // If health is larger than 0 (Enemy is alive) allows enemy to continue carrying out it's commands.
        if (health > 0)
        {
            Moving();
            Turn();
        }
        // If health is 0 or less (Enemy is dead) starts the death method.
        else
        {
            Death();
        }
    }

}