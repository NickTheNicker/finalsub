using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityStandardAssets.CrossPlatformInput;


public class Player : MonoBehaviour
{
    // Player Variables/Status (p=player).
    // Maximum amount of health.
    [SerializeField] public int pMaxHealth = 10;
    // Current amount of health.
    // If current health <= 0 the player is dead
    [SerializeField] public int pHealth;
    // Amount of attack damage.
    [SerializeField] public int pAttack = 2;
    // Player's running speed.
    [SerializeField] public int pSpeed = 7;
    // After the player presses 'L' they move backwards quickly and set dodging as true for the duration of the movement.
    public bool dodging;
    // After the player pressed 'L' dodged will be set as true and prevent the player from dodging for a short period of time.
    public bool dodged;
    // After the player presses 'K' it starts the first attack animation and sets attacked as true.
    public bool attacked;
    // Set as true when you press 'K' during the first attack animation.
    public bool followup;
    // If true, the player cannot attack.
    // Set as true when you press 'K' during the first attack animation and didn't press 'K' again.
    // OR is set as true at the start of the second attack animation .
    public bool pause;
    // If the true, the player won't take damage.
    public bool invincible;


    // Cached References 
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
    // The scene changing script that is used to restart the scene afte the player dies.
    SceneChange sceneChange;
    // Trigger collider that the player uses to attack enemies.
    BoxCollider2D iBox;

    // Start is called before the first frame update.
    void Start()
    {
        iRigidbody = GetComponent<Rigidbody2D>();
        iCapsule = GetComponent<CapsuleCollider2D>();
        iAnimator = GetComponent<Animator>();
        iTransform = GetComponent<Transform>();
        hitHeal = GetComponent<HitHeal>();
        sceneChange = GetComponent<SceneChange>();
        iBox = GetComponent<BoxCollider2D>();

        // Player health is set equal max health at the start of a level/scene.
        pHealth = pMaxHealth;
        // Player is not dodging.
        dodging = false;
        // Player has not dodged.
        dodged = false;
        // Player's attack collider is off.
        iBox.enabled = false;
        // Player has not attacked.
        attacked = false;
        // Player has not attacked and pressed 'k' again.
        followup = false;
        // Player has not attacked.
        pause = false;
        // Player has not been hit yet.
        invincible = false;
}

    // !Methods!

    // !Status methods!

    // Returns the player pHealth int (used in CurrentHealth script).
    public int FindHealth()
    {
        return pHealth;
    }
    // Returns the player pMaxHealth int (used in MaxHealth script).
    public int FindMaxHealth()
    {
        return pMaxHealth;
    }

    // !Movement methods!

    // Makes the player run when "A"(left) or "D"(right) is pressed.
    private void Run()
    {
        // Direction = -1 when the player presses 'A'.
        // Direction = 1 when the player presses 'D'.
        float direction = CrossPlatformInputManager.GetAxis("Horizontal");
        // Changes the player's x velocity to equal direction x 'pSpeed'.
        iRigidbody.velocity = new Vector2(direction * pSpeed, iRigidbody.velocity.y);
    }
    // Makes the player jump when "W" is pressed.
    private void Jump()
    {
        if (iCapsule.IsTouchingLayers(LayerMask.GetMask("Ground")) & CrossPlatformInputManager.GetButtonDown("Jump"))
            {
            // The y velocity added when you jump
                Vector2 jump = new Vector2(0f, 7f);
                iRigidbody.velocity += jump;
                AttackReset();
            }
    }
    // Makes the player quickly move backwards when "L" is pressed then prevents the player from dodging for a while.
    private void Dodge()
    {
        if ((CrossPlatformInputManager.GetButtonDown("Dodge")) & (!dodged))
        {
            Dodging();
            Invoke("Dodging",0.1f);
            Dodged();
            Invoke("Dodged",2);
        }
        if (dodging)
        {
            iRigidbody.velocity += new Vector2(-18 * Mathf.Sign(transform.localScale.x), 0);
        }
        // Prevents the player from flying (very high) off slopes after dodging into them.
        if (dodging & iRigidbody.velocity.y>0)
        {
            iRigidbody.velocity = new Vector2(iRigidbody.velocity.x,0);
        }
    }
    // Sets dodging as false when true and true when false(Method for Invoke)
    private void Dodging()
    {
        if(dodging)
        {
            dodging = false;
        }
        else
        {
            dodging = true;
        }
    }
    // Sets dodged as false when true and true when false(Method for Invoke).
    private void Dodged()
    {
        if (dodged)
        {
            dodged = false;
        }
        else
        {
            dodged = true;
        }
    }

    // !Methods for attacking enemies!
    // The user presses k and the player attacks: starts the attack animation and switches on/off the attack collider
    private void Attack()
    {
        if ((CrossPlatformInputManager.GetButtonDown("Attack")) & (!attacked) & (!pause))
        {
            // Animation events trigger box collider on then off and carries out Followup on the last frame.
            iAnimator.SetTrigger("Attack1"); 
            attacked = true;
        }
        // If the player has attacked once and presses k during the animation the bool followup is set as true.
        else if ((CrossPlatformInputManager.GetButtonDown("Attack")) & (attacked) & (!pause))
        {
            followup = true;
        }
    }
    // In animation events, followups with the second attack if followup is true.
    private void Attack2()
    {
        if (followup)
        {
            iAnimator.SetTrigger("Attack2");
            pause = true;
            Invoke("AttackReset",1);
        }
        else
        {
            pause = true;
            Invoke("AttackReset", 1);
        }
    }
    // Sets attacked, followup and pause all as false.
    private void AttackReset()
    {
        attacked = false;
        followup = false;
        pause = false;
    }
    // Turns off the box collider responisble for damage/Used in aniamtion events.
    private void BoxSwitch()
    {
        if (iBox.enabled)
        {
            iBox.enabled = false;
        }
        else
        {
            iBox.enabled = true;
        }
        
    }
    // Sets the animator controller parameter bools as true or false and flips the sprite.
    private void Animations()
    {
        // Jumping: If player vertical velocity is larger than 0 sets "Jumping as true" and if not sets as false.
        if (iRigidbody.velocity.y > 0)
        {
            iAnimator.SetBool("Jumping", true);
        }
        else iAnimator.SetBool("Jumping", false);
        // Falling: If player vertical velocity is smaller than 0 sets "Falling" as true and if not sets as false.
        if (iRigidbody.velocity.y < 0)
        {
            iAnimator.SetBool("Falling", true);
        }
        else iAnimator.SetBool("Falling", false);
        // Idle: Sets 'Running' as false when the player has no horizontal velocity.
        if (iRigidbody.velocity.x == 0)
        {
            iAnimator.SetBool("Running", false);
        }
        else
        {
            iAnimator.SetBool("Running", true);
        }
        // Dodge: Sets the animator 'Dodging' bool as true if script bool 'dodging' is true.
        // Sets the animator 'Dodging' bool as false if script bool 'dodging' is false.
        if (dodging)
        {
            iAnimator.SetBool("Dodging", true);
        }
        else
        {
            iAnimator.SetBool("Dodging", false);
        }

        // Flip sprite: Flips the player sprite to face the direction they are moving towards.
        if ((!(iRigidbody.velocity.x == 0 )) & (!dodging))
        {
            transform.localScale = new Vector2(Mathf.Sign(iRigidbody.velocity.x), 1f);
        }
    }
    // !Damage (Taking) Methods!
    // If invincible is true, switches it to false or if false, switches it to true.
    private void Invincible()
    {
        if (invincible)
        {
            invincible = false;
        }
        else
        {
            invincible = true;
        }
    }

    private void TouchDamage()// Activates when an enemy attacks the player.
    { 
       if ((iCapsule.IsTouchingLayers(LayerMask.GetMask("Enemy")) & (!invincible)))
        {
            // Decreases player health.
            hitHeal.DamageTaken();
            // Plays the hurt animation.
            iAnimator.SetTrigger("Hurt");
            // Allows the player to attack immediately after being hit.
            AttackReset();
            // Knocks the player back.
            iRigidbody.velocity = new Vector2(-(Mathf.Sign(transform.localScale.x) * 4f), 2f);
            // Makes the player invincible.
            Invincible();
            // Stops the player from being invincible after 2 seconds.
            Invoke("Invincible", 2);
        }
    }
    // Restarts the level.
    private void RestartLevel()
    {
        sceneChange.Restart();
    }

    // Plays the death animation and resets the level and player to the last checkpoint.
    private void Death()
    {
        iAnimator.SetTrigger("Death");
        Invoke("RestartLevel",4);
    }


    // Update is called once per frame.
    void Update()
    {
        // If health is bigger than 0 (Player is alive) allows control of player.
        if (pHealth > 0)
        {
            Run();
            Jump();
            Dodge();
            Animations();
            TouchDamage();
            Attack();
        }
        // If health is 0 or less (Player is dead) stops all player control and starts death method.
        else Death();
    }
}
