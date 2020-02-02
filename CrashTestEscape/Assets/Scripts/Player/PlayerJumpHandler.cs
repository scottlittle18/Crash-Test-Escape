using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listens for and Handles Jumping Input and Movement respectively.
/// 
/// Requires:
///     - Rigidbody2D Component
///     - GroundCheck Script & Child Object
///     - Health System that tells this script that the associated character is alive
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJumpHandler : MonoBehaviour
{
    [Header("Jump Settings")]

    [SerializeField, Tooltip("How fast will the player be able to accelerate to their maximum jump speed?")]
    private float m_jumpingAccelerationRate = 0.0f;

    [SerializeField, Tooltip("What is the maximum speed that the player will be able to jump upward?")]
    private float m_maxJumpSpeed = 0.0f;

    public float MaxJumpSpeed { get { return m_maxJumpSpeed; } private set { m_maxJumpSpeed = value; } }

    [SerializeField, Tooltip("Adjusts the length of time that the jump input is accepted for. (This is modeled after Mario's jump mechanic).")]
    private float m_jumpLength = 0.0f;

    private bool m_isJumping = false;

    private Rigidbody2D m_playerRigidbody;

    private GroundCheck m_groundCheck;

    private PlayerHealthSystem m_playerHealthSystem;

    /// <summary>
    /// An internal class that contains all movement related input fields required for this script.
    /// </summary>
    internal class InputListener
    {
        //public float m_horizontalMoveInput;

        public bool m_jumpInput;
    }

    //Create a variable for the InputListener class
    private InputListener m_inputListener = new InputListener();


    // Start is called before the first frame update
    void Start()
    {
        //Set up all necessary component variables
        InitializePlayerComponents();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerHealthSystem.IsAlive)
        {
            //--Listeners--
            JumpInputListener();
        }
    }

    private void FixedUpdate()
    {
        //Handle forces associated with jumping
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
        m_groundCheck = GetComponentInChildren<GroundCheck>();
    }

    /// <summary>
    /// Listens for jump input. This data is used by the JumpInputHandler() to apply the jump motion to the player.
    /// </summary>
    private void JumpInputListener()
    {
        m_inputListener.m_jumpInput = Input.GetButton("Jump");
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
            m_playerRigidbody.AddForce(Vector2.up * m_maxJumpSpeed * m_jumpingAccelerationRate, ForceMode2D.Force);
            StartCoroutine(JumpTimeLimiter());
        }

        //While Jumping
        if (m_isJumping && m_inputListener.m_jumpInput)
        {
            Debug.Log($"Player is still jumping...");
            //Keep the player's vertical velocity equal to their jump velocity
            //m_playerRigidbody.velocity = new Vector2(m_playerRigidbody.velocity.x, m_playerJumpSpeed * Time.deltaTime);
            m_playerRigidbody.AddForce(Vector2.up * m_maxJumpSpeed * m_jumpingAccelerationRate, ForceMode2D.Force);
        }

        // When the player releases the jump input
        if (Input.GetButtonUp("Jump"))
        {
            Debug.Log($"Player has stopped jumping...");
            StopCoroutine(JumpTimeLimiter());
            m_isJumping = false;
        }
    }
}
