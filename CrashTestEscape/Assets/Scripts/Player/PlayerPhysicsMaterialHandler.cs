using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysicsMaterialHandler : MonoBehaviour
{
    private GroundCheck m_groundCheck;
    private BoxCollider2D m_playerCollider;
    private Rigidbody2D m_playerRigidbody;
    private PlayerMovementHandler m_playerMovementHandler;

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
        m_playerPhysMatFriction = m_playerCollider.friction;
        m_playerRigidbody = GetComponent<Rigidbody2D>();
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
        // Set the friction of the player's collider to 0 to keep them from sticking to walls
        if (!m_groundCheck.IsGrounded || (m_groundCheck.IsGrounded && !m_playerMovementHandler.HorizontalMoveInputReceived))
        {
            m_playerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = 0.0f;
        }
        else if (m_groundCheck.IsGrounded) // Reset the player's friction to it's original value when the player is not on the ground
        {
            m_playerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = m_playerPhysMatFriction;
        }
    }
}
