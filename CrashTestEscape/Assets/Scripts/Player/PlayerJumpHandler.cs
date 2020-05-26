using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Listens for and Handles Jumping Input and Movement respectively. This specific script is meant to only implement
/// a basic jump for the player's character.
/// 
/// Requires:
///     - A Rigidbody2D Component attatched to the same object as this script.
///     - A script that's specifically called GroundCheck, which needs to be attached to a child object.
///     - A tag in the engine called "Ground" ( with that exact spelling, otherwise it won't work .)
///     - Objects with colliders that act as the ground for the player and have the "Ground" tag, which was mentioned
///     in the previous step, assigned to them.
///     - A Health System that tells this script that the character this script is attached to is alive.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJumpHandler : MonoBehaviour
{
    [SerializeField, Tooltip("This is the initial force that is applied when the player goes to jump.")]
    private float m_jumpForce = 1.0f;

    [SerializeField, Tooltip("This is the maximum velocity that the player will have when jumping.")]
    private float m_maxJumpSpeed = 1.0f;

    public float MaxJumpSpeed { get { return m_maxJumpSpeed; } private set { m_maxJumpSpeed = value; } }

    private bool m_isJumping = false;

    public bool IsJumping { get { return m_isJumping; } private set { m_isJumping = value; } }

    /// <summary>
    /// This should be assigned to the input for jumping
    /// </summary>
    private bool m_jumpInput;
    //public bool JumpInput { get { return m_jumpInput; } }

    /// <summary>
    /// Determines whether or not the player can jump again based on whether they're on the ground or not
    /// </summary>
    private bool m_canJump;

    #region Unity Components
        private Rigidbody2D m_playerRigidbody;
        private Animator m_PlayerAnim;
    #endregion

    #region Custom Scripts, Systems, and Objects
        private GroundCheck m_groundCheck;
        private PlayerHealthSystem m_playerHealthSystem;
    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        //Set up all necessary component variables
        InitializePlayerComponents();
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_groundCheck.IsGrounded || m_groundCheck.IsOnMovingPlatform)
        {
            m_canJump = true;
        }
        else
            m_canJump = false;

        if (m_playerHealthSystem.IsAlive && !m_playerHealthSystem.IsBeingKnockedBack && m_canJump)
        {
            // Listens For Input from Player
            JumpInputListener();
        }

        m_PlayerAnim.SetBool("IsJumping", (m_playerRigidbody.velocity.y > 0.01f && IsJumping));
    }

    private void FixedUpdate()
    {
        if (!m_playerHealthSystem.IsBeingKnockedBack)
        {
            //Handle the application of forces associated with jumping
            JumpInputHandler();
        }
    }

    /// <summary>
    /// Used to Initialize any necessary component variables (e.g. Rigidbody2D, BoxCollider2D, etc.)
    /// </summary>
    private void InitializePlayerComponents()
    {
        // Initialize Player Systems (e.g. health, attack, etc.)
        m_playerHealthSystem = GetComponent<PlayerHealthSystem>();

        m_playerRigidbody = GetComponent<Rigidbody2D>();
        m_groundCheck = GetComponentInChildren<GroundCheck>();
        m_PlayerAnim = GetComponent<Animator>();
    }

    /// <summary>
    /// Listens for jump input. This data is used by the JumpInputHandler() to apply the jump motion to the player.
    /// </summary>
    private void JumpInputListener()
    {
        m_jumpInput = Input.GetButtonDown("Jump");
    }

    /// <summary>
    /// Handles and applies the jump input received from the player
    /// </summary>
    private void JumpInputHandler()
    {
        // If the player is trying to jump
        if ((m_groundCheck.IsGrounded || m_groundCheck.IsOnMovingPlatform) && m_jumpInput)
        {
            m_playerRigidbody.AddForce(Vector2.up * m_jumpForce, ForceMode2D.Impulse);

            IsJumping = true;
        }

        // When the player releases the jump input
        if ((m_groundCheck.IsGrounded || m_groundCheck.IsOnMovingPlatform) && !m_jumpInput)
        {
            IsJumping = false;
        }
    }
}
