using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to manage the settings of the Physics Materials associated with the player
/// </summary>
public class PlayerPhysicsMaterialHandler : MonoBehaviour
{
    private GroundCheck m_groundCheck;
    private BoxCollider2D m_playerCollider;
    private Rigidbody2D m_playerRigidbody;
    private PlayerMovementHandler m_playerMovementHandler;
    private PlayerHealthSystem m_playerHealthSystem;

    /// <summary>
    /// This is for storing the friction value of the PhysMat 
    /// that's been assigned to the player's collider
    /// </summary>
    private float m_playerPhysMatFriction;
    
    // Start is called before the first frame update
    void Start()
    {
        // Gather required component references and values
        m_groundCheck = GetComponentInChildren<GroundCheck>();
        m_playerCollider = GetComponent<BoxCollider2D>();
        m_playerPhysMatFriction = m_playerCollider.sharedMaterial.friction;
        m_playerRigidbody = GetComponent<Rigidbody2D>();
        m_playerMovementHandler = GetComponent<PlayerMovementHandler>();
        m_playerHealthSystem = GetComponent<PlayerHealthSystem>();
    }

    // Update is called once per frame
    void Update()
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
            if (!m_groundCheck.IsGrounded || ((m_groundCheck.IsGrounded || m_groundCheck.IsOnMovingPlatform) && m_playerMovementHandler.PlayerIsNotMoving))
            {
                m_playerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = 0.0f;
            }
            else if ((m_groundCheck.IsGrounded || m_groundCheck.IsOnMovingPlatform) && !m_playerMovementHandler.PlayerIsNotMoving) // Reset the player's friction to it's original value when the player is not on the ground
            {
                m_playerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = m_playerPhysMatFriction;
            }
        }
        else if (!m_playerHealthSystem.IsAlive)
        {
            if (!m_playerHealthSystem.IsBeingKnockedBack && (m_groundCheck.IsGrounded || m_groundCheck.IsOnMovingPlatform))
            {
                // Increase the friction of the player when they die in order to keep them from sliding after death, a magic number was used since we just need a high number here and nothing specific
                m_playerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = m_playerPhysMatFriction;
            }
        }
    }
}
