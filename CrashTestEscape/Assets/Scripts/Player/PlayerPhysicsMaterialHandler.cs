using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to manage the settings of the Physics Materials associated with the player
/// </summary>
public class PlayerPhysicsMaterialHandler : MonoBehaviour
{
    //TODO: Look into just calling these from the PlayerMovementHandler.cs to simplify setting up the object
    [SerializeField, Tooltip("This is the collider that becomes active while the player IS crouched.")]
    private BoxCollider2D m_crouchingPlayerCollider;

    [SerializeField, Tooltip("This is the collider that becomes active while the player IS NOT crouched.")]
    private BoxCollider2D m_primaryPlayerCollider;

    private GroundCheck m_groundCheck;
    private Rigidbody2D m_playerRigidbody;
    private PlayerMovementHandler m_playerMovementHandler;
    private PlayerHealthSystem m_playerHealthSystem;

    /// <summary>
    /// This is for storing the friction value of the PhysMat 
    /// that's been assigned to the player's collider
    /// </summary>
    private float m_playerPhysMatFriction;

    private void Awake()
    {
        // Gather required component references and values
        m_groundCheck = GetComponentInChildren<GroundCheck>();
        m_primaryPlayerCollider = GetComponent<BoxCollider2D>();        
        m_playerRigidbody = GetComponent<Rigidbody2D>();
        m_playerMovementHandler = GetComponent<PlayerMovementHandler>();
        m_playerHealthSystem = GetComponent<PlayerHealthSystem>();        
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (m_primaryPlayerCollider.enabled == true)
        {
            if (m_primaryPlayerCollider.sharedMaterial.friction != 0.0f)
            {
                // The Collider and Rigidbody components should have the same friction value so where you retrieve it shouldn't matter
                m_playerPhysMatFriction = m_primaryPlayerCollider.sharedMaterial.friction;
            }
            else if ((m_primaryPlayerCollider.sharedMaterial.friction == 0) || (m_playerRigidbody.sharedMaterial.friction == 0))
            {
                m_primaryPlayerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = 1.0f;
                m_playerPhysMatFriction = m_primaryPlayerCollider.sharedMaterial.friction;
            }
        }

        if (m_crouchingPlayerCollider.enabled == true)
        {
            if (m_crouchingPlayerCollider.sharedMaterial.friction != 0.0f)
            {
                // The Collider and Rigidbody components should have the same friction value so where you retrieve it shouldn't matter
                m_playerPhysMatFriction = m_crouchingPlayerCollider.sharedMaterial.friction;
            }
            else if ((m_crouchingPlayerCollider.sharedMaterial.friction == 0) || (m_playerRigidbody.sharedMaterial.friction == 0))
            {
                m_crouchingPlayerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = 1.0f;
                m_playerPhysMatFriction = m_crouchingPlayerCollider.sharedMaterial.friction;
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePhysicsMaterial();
    }

    /// <summary>
    /// Update the active physics material on the player.
    /// </summary>
    private void UpdatePhysicsMaterial()
    {
        if (m_playerHealthSystem.IsAlive)
        {
            // Set the friction of the player's collider to 0 to keep them from sticking to walls
            if (!m_groundCheck.IsGrounded && !m_groundCheck.IsOnMovingPlatform)
            {
                if (m_primaryPlayerCollider.enabled == true)
                {
                    m_primaryPlayerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = 0.0f;
                }

                if (m_crouchingPlayerCollider.enabled == true)
                {
                    m_crouchingPlayerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = 0.0f;
                }
            }

            // Reset the player's friction to it's original value when the player is on the ground and not trying to move
            if ((m_groundCheck.IsGrounded || m_groundCheck.IsOnMovingPlatform) && m_playerMovementHandler.PlayerIsNotMoving)
            {
                if (m_primaryPlayerCollider.enabled == true)
                {
                    m_primaryPlayerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = m_playerPhysMatFriction;
                }

                if (m_crouchingPlayerCollider.enabled == true)
                {
                    m_crouchingPlayerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = m_playerPhysMatFriction;
                }

                m_primaryPlayerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = m_playerPhysMatFriction;
            }
        }
        else if (!m_playerHealthSystem.IsAlive)
        {
            if (!m_playerHealthSystem.IsBeingKnockedBack && (m_groundCheck.IsGrounded || m_groundCheck.IsOnMovingPlatform))
            {
                if (m_primaryPlayerCollider.enabled == true)
                {
                    m_primaryPlayerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = m_playerPhysMatFriction;
                }

                if (m_crouchingPlayerCollider.enabled == true)
                {
                    // Increase the friction of the player when they die in order to keep them from sliding after death
                    m_crouchingPlayerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = m_playerPhysMatFriction;
                }

                // Increase the friction of the player when they die in order to keep them from sliding after death, a magic number was used since we just need a high number here and nothing specific
                //m_primaryPlayerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = m_playerPhysMatFriction;
            }
        }
    }
}
