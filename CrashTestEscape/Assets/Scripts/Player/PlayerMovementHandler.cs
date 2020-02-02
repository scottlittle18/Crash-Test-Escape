using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listens for and Handles Running Input and Movement respectively.
/// 
/// Requires:
///     - GroundCheck Script & Object
///     - Health System that tells this script that the associated character is alive
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementHandler : MonoBehaviour
{
    #region Serialized Fields
    [Header("Movement Settings")]
    
    [SerializeField, Tooltip("How fast will the player be able to accelerate to their maximum running speed? \n\n(Setting this equal to, or greater than, the max move speed with mean they will immediately reach their max veloctiy)")]
    private float m_runningAccelerationRate = 0.0f;

    [SerializeField, Tooltip("How fast will the player be able to run around?")]
    private float m_maxMoveSpeed = 0.0f;

    [Header("Jump Settings")]

    //[SerializeField, Tooltip("How fast will the player be able to accelerate to their maximum jump speed?")]
    //private float m_jumpingAccelerationRate = 0.0f;

    [SerializeField, Tooltip("What is the maximum speed that the player will be able to jump upward?")]
    private float m_maxJumpSpeed = 0.0f;

    [SerializeField, Tooltip("Adjusts the length of time that the jump input is accepted for. (This is modeled after Mario's jump mechanic).")]
    private float m_jumpLength = 0.0f;

    [SerializeField, Tooltip("This determines how close to zero the player's velocity needs to be to flip the sprite in the Sprite Renderer.")]
    private float m_turningSpriteFlipThreshold = 0.0f;
    #endregion------------

    #region Standard Local Member Variables (m_ == A local member of a class)
    private bool m_isfacingLeft;
    private bool m_isJumping = false;
    // This is for storing the friction value of the PhysMat that's been assigned to the player's collider.
    private float m_playerPhysMatFriction;
    #endregion------------

    #region Component Variable Containers
    private Rigidbody2D m_playerRigidbody;
    private SpriteRenderer m_playerSpriteRenderer;
    private GroundCheck m_groundCheck;
    //TODO: Swap with proper collider(s) later
    private BoxCollider2D m_playerCollider;

    // Player Systems
    private PlayerHealthSystem m_playerHealthSystem;
    #endregion------------

    /// <summary>
    /// An internal class that contains all movement related input fields required for this script.
    /// </summary>
    internal class InputListener
    {
        public float m_horizontalMoveInput;

        public bool m_jumpInput;
    }

    //Create a variable for the InputListener class
    private InputListener m_inputListener = new InputListener();

    // Start is called before the first frame update
    private void Start()
    {
        //Set up all necessary component variables
        InitializePlayerComponents();
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_playerHealthSystem.IsAlive)
        {
            //--Listeners--
            HorizontalMoveInputListener();
            JumpInputListener();

            //If input to the horizontal axis is detected, update the look direction to keep the sprite facing the last direction the player moved in
            if (!Mathf.Approximately(m_inputListener.m_horizontalMoveInput, 0.0f))
                UpdateLookDirection();
        }

        UpdatePhysicsMaterial();
    }

    private void FixedUpdate()
    {
        //Apply horizontal player movement input.
        HorizontalMoveInputHandler();
        JumpInputHandler();
    }

    /// <summary>
    /// Used to Initialize any necessary component variables (e.g. Rigidbody2D, BoxCollider2D, etc.)
    /// </summary>
    private void InitializePlayerComponents()
    {
        // Initialize Player Systems (e.g. health, attack, etc.)
        m_playerHealthSystem = GetComponent<PlayerHealthSystem>();

        m_playerRigidbody = GetComponent<Rigidbody2D>();
        m_playerSpriteRenderer = GetComponent<SpriteRenderer>();
        m_groundCheck = GetComponentInChildren<GroundCheck>();
        m_playerCollider = GetComponent<BoxCollider2D>();
        m_playerPhysMatFriction = m_playerCollider.friction;
    }

    /// <summary>
    /// Update the active physics material on the player.
    /// </summary>
    private void UpdatePhysicsMaterial()
    {
        // Set the friction of the player's collider to 0 to keep them from sticking to walls
        if (!m_groundCheck.IsGrounded || (m_groundCheck.IsGrounded && !Mathf.Approximately(m_inputListener.m_horizontalMoveInput, 0.0f)))
        {
            m_playerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = 0.0f;
        }
        else if (m_groundCheck.IsGrounded) // Reset the player's friction to it's original value when the player is not on the ground
        {
            m_playerCollider.sharedMaterial.friction = m_playerRigidbody.sharedMaterial.friction = m_playerPhysMatFriction;
        }
    }
    
    #region _________________________________________________________________LISTENERS______________________________
    /// <summary>
    /// Listens for the input received from the player. This data is used by the HorizontalMoveInputHandler() to apply this to the movement of the player-character.
    /// </summary>
    private void HorizontalMoveInputListener()
    {
        m_inputListener.m_horizontalMoveInput = Input.GetAxisRaw("Horizontal");
    }

    /// <summary>
    /// Listens for jump input. This data is used by the JumpInputHandler() to apply the jump motion to the player.
    /// </summary>
    private void JumpInputListener()
    {
        m_inputListener.m_jumpInput = Input.GetButton("Jump");
    }
    #endregion

    #region ______________________________________________________________________HANDLERS__________________________
    /// <summary>
    /// Handles and applies the horizontal input received from the player.
    /// </summary>
    private void HorizontalMoveInputHandler()
    {
        // TODO: If a dash ability is added this will need to be encapsulated in something like if (!m_isDashing){}
        //Accelerate player and clamp their velocity
        m_playerRigidbody.AddForce(Vector2.right * m_inputListener.m_horizontalMoveInput * m_runningAccelerationRate);
        Vector2 clampedVelocity = m_playerRigidbody.velocity;
        clampedVelocity.x = Mathf.Clamp(m_playerRigidbody.velocity.x, -m_maxMoveSpeed, m_maxMoveSpeed);
        clampedVelocity.y = Mathf.Clamp(m_playerRigidbody.velocity.y, Mathf.NegativeInfinity, m_maxJumpSpeed);
        m_playerRigidbody.velocity = clampedVelocity;

        // If no movement input is detected but the player is still moving, this code block will stop the player's horizontal movement when the player is no longer holding a movement button
        if (m_inputListener.m_horizontalMoveInput == 0.0f && m_playerRigidbody.velocity.x != 0.0f)
        {
            m_playerRigidbody.velocity = new Vector2(0.0f, m_playerRigidbody.velocity.y);
        }

        //TODO: Remove For Polish (Move Player by setting velocity)
        //m_playerRigidbody.velocity = new Vector3(m_inputListener.m_horizontalMoveInput * m_maxMoveSpeed * Time.deltaTime, m_playerRigidbody.velocity.y, 0);
    }

    /// <summary>
    /// Limits the length of time the player will be able to jump for.
    /// </summary>
    /// <returns>Waits for the given length of time [Adjustable In-Editor]</returns>
    private IEnumerator JumpTimeLimiter()
    {
        m_isJumping = true;
        yield return new WaitForSecondsRealtime(m_jumpLength);
        m_isJumping = false;
    }

    /// <summary>
    /// Handles and applies the jump input received from the player
    /// </summary>
    private void JumpInputHandler()
    {
        //TODO: Debugging Jump
        Debug.Log($"JumpInputHandler() Entered...");

        // If the player is trying to jump
        if (m_groundCheck.IsGrounded && m_inputListener.m_jumpInput)
        {
            Debug.Log($"Player is Trying to jump...");
            //m_playerRigidbody.velocity = new Vector2(m_playerRigidbody.velocity.x, m_playerJumpSpeed * Time.deltaTime);
            m_playerRigidbody.AddForce(Vector2.up * m_maxJumpSpeed * m_runningAccelerationRate, ForceMode2D.Force);
            StartCoroutine(JumpTimeLimiter());
        }

        //While Jumping
        if (m_isJumping && m_inputListener.m_jumpInput)
        {
            Debug.Log($"Player is still jumping...");
            //Keep the player's vertical velocity equal to their jump velocity
            //m_playerRigidbody.velocity = new Vector2(m_playerRigidbody.velocity.x, m_playerJumpSpeed * Time.deltaTime);
            m_playerRigidbody.AddForce(Vector2.up * m_maxJumpSpeed * m_runningAccelerationRate, ForceMode2D.Force);
        }

        // When the player releases the jump input
        if (Input.GetButtonUp("Jump"))
        {
            Debug.Log($"Player has stopped jumping...");
            StopCoroutine(JumpTimeLimiter());
            m_isJumping = false;
        }
    }

    /// <summary>
    /// Updates the direction the sprite should be facing based on the horizontal player input
    /// </summary>
    private void UpdateLookDirection()
    {
        if (m_inputListener.m_horizontalMoveInput > m_turningSpriteFlipThreshold)
        {
            m_playerSpriteRenderer.flipX = m_isfacingLeft = false;
            m_isfacingLeft = false;
        }
        else if (m_inputListener.m_horizontalMoveInput < -m_turningSpriteFlipThreshold)
        {
            m_playerSpriteRenderer.flipX = true;
            m_isfacingLeft = true;
        }
        else if (m_inputListener.m_horizontalMoveInput >= - m_turningSpriteFlipThreshold && m_inputListener.m_horizontalMoveInput <= m_turningSpriteFlipThreshold)
        {
            if (!m_isfacingLeft)
                m_playerSpriteRenderer.flipX = true;
            else
                m_playerSpriteRenderer.flipX = false;
        }
    }
    #endregion
}