using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose:
///     Handles the Health of the Player Character as well as their respawn
/// </summary>
public class PlayerHealthSystem : MonoBehaviour, IDamagable
{
    /// <summary>
    /// The maximum amount of health the player can have when their health is full.
    /// </summary>
    [SerializeField, Tooltip("The maximum amount of health the player can have when their health is full.")]
    private int m_maxPlayerHealth;

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

    /// <summary>
    /// The position the player will start the level at.
    /// </summary>
    private Transform m_levelStartPointTransform;

    /// <summary>
    /// The position of the current Checkpoint.
    /// </summary>
    private Transform m_currentCheckpointTransform;

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

    private void Awake()
    {
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
        if (!IsAlive)
        {
            Die();
        }
    }

    public void TakeDamage(int damageRecieved)
    {
        //TODO: Debug PlayerHealthSystem TakeDamage()
        Debug.Log($"Player has taken damage!");
        
        m_currentPlayerHealth -= damageRecieved;

        Debug.Log($"Player now has {m_currentPlayerHealth}HP");
    }

    public void Die()
    {
        //TODO: Debug PlayerDeath
        Debug.Log("Player is dead.");
    }
}
