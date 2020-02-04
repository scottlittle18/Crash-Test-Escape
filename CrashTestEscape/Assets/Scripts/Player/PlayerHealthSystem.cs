using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose:
///     Handles the Health of the Player Character as well as their knockback and respawn
/// </summary>
public class PlayerHealthSystem : MonoBehaviour, IDamagable
{
    /// <summary>
    /// The maximum amount of health the player can have when their health is full.
    /// </summary>
    [SerializeField, Tooltip("The maximum amount of health the player can have when their health is full.")]
    private int m_maxPlayerHealth = 0;

    /// <summary>
    /// How much health does the player have at this point in time?
    /// </summary>
    private int m_currentPlayerHealth;

    private bool m_isAlive;

    public bool IsAlive
    {
        get { return m_isAlive; }
        private set
        {
            m_isAlive = m_currentPlayerHealth > 0 ? true : false;
        }
    }

    private bool m_isBeingKnockedBack = false;

    /// <summary>
    /// Used by the Player Movement Handler to determine whether the player is in
    ///     the process of being knocked back and is unable to control their movement
    /// </summary>
    public bool IsBeingKnockedBack
    {
        get { return m_isBeingKnockedBack; }
        private set
        {
            m_isBeingKnockedBack = value;
        }
    }

    /// <summary>
    /// The position the player will start the level at.
    /// </summary>
    private Transform m_levelStartPointTransform;

    /// <summary>
    /// The position of the current Checkpoint.
    /// </summary>
    private Transform m_currentCheckpointTransform;

    private Rigidbody2D m_playerRigidbody;

    /// <summary>
    /// *USE CurrentCheckpoint PROPERTY INSTEAD*
    /// </summary>
    private Checkpoint m_currentCheckpoint;
    
    /// <summary>
    /// The Checkpoint that the player will respawn at upon death.
    /// </summary>
    public Checkpoint CurrentCheckpoint
    {
        get
        {
            return m_currentCheckpoint;
        }
        set
        {
            if (m_currentCheckpoint == null)
            {
                //If no checkpoint has been reached yet, then there is no need to deactivate an old checkpoint
                m_currentCheckpoint = value;
                m_currentCheckpoint.IsActivated = true;
            }
            else
            {
                //Deactivate old checkpoint
                m_currentCheckpoint.IsActivated = false;

                //Set to the new checkpoint
                m_currentCheckpoint = value;

                //Activate new checkpoint
                m_currentCheckpoint.IsActivated = true;
            }
        }
    }

    IEnumerator KnockbackTimer(float knockbackDuration)
    {
        IsBeingKnockedBack = true;
        yield return new WaitForSecondsRealtime(knockbackDuration);
        IsBeingKnockedBack = false;
    }

    private void Awake()
    {
        m_playerRigidbody = GetComponent<Rigidbody2D>();

        //Start player with a full health bar
        m_currentPlayerHealth = m_maxPlayerHealth;

        IsAlive = true;

        if (GameObject.FindGameObjectWithTag("SpawnPoint") == null)
        {
            Debug.Log("SpawnPoint not found!");
        }
        else
        {
            //Gets the starting point of the level in case the player dies without reaching the first checkpoint of the level
            m_levelStartPointTransform = GameObject.FindGameObjectWithTag("SpawnPoint").GetComponent<Transform>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_currentPlayerHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damageRecieved)
    {
        Debug.Log($"Player had {m_currentPlayerHealth}HP");

        m_currentPlayerHealth -= damageRecieved;

        //TODO: Debug PlayerHealthSystem TakeDamage()
        Debug.Log($"Player has taken damage!");

        Debug.Log($"Player now has {m_currentPlayerHealth}HP");
    }

    public void DetermineAndApplyKnockbackForce(float upwardKnockbackForce, float lateralKnockbackForce, bool isOnLeft, float knockbackDuration)
    {
        // If player position is LESS THAN the hazards position, apply the knockback to the left
        if (isOnLeft == true)
        {
            m_playerRigidbody.AddForce(Vector2.up * upwardKnockbackForce, ForceMode2D.Impulse);
            m_playerRigidbody.AddForce(Vector2.left * lateralKnockbackForce, ForceMode2D.Impulse);
        }
        // If player position is GREATER THAN the hazards position, apply the knockback to the right
        else if (isOnLeft == false)
        {
            m_playerRigidbody.AddForce(Vector2.up * upwardKnockbackForce, ForceMode2D.Impulse);
            m_playerRigidbody.AddForce(Vector2.right * lateralKnockbackForce, ForceMode2D.Impulse);
        }

        StartCoroutine("KnockbackTimer", knockbackDuration);
    }

    private void Die()
    {
        //TODO: Debug PlayerDeath
        Debug.Log("Player is dead.");

        //Have to manually set this because the damn teranary operator is being an ass
        IsAlive = false;
    }
}
