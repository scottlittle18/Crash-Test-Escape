using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listens for and Handles Running Input and Movement respectively.
/// 
/// Requires:
///     - Rigidbody2D Component
///     - GroundCheck Script & Child Object
///     - Health System that tells this script that the associated character is alive
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementHandler : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField, Tooltip("How fast will the player be able to accelerate to their maximum running speed? \n\n(Setting this equal to, or greater than, the max move speed with mean they will immediately reach their max veloctiy)")]
    private float m_runningAccelerationRate = 0.0f;

    [SerializeField, Tooltip("How fast will the player be able to run around?")]
    private float m_maxMoveSpeed = 0.0f;

    [SerializeField, Tooltip("This determines how close to zero the player's velocity needs to be to flip the sprite in the Sprite Renderer.")]
    private float m_turningSpriteFlipThreshold = 0.0f;

    [SerializeField, Tooltip("This is the collider that becomes active while the player IS crouched.")]
    private BoxCollider2D m_crouchingPlayerCollider;

    [SerializeField, Tooltip("This is the collider that becomes active while the player IS NOT crouched.")]
    private BoxCollider2D m_primaryPlayerCollider;

    [SerializeField, Tooltip("How long should the player be unable to move for after being crushed by a piston?")]
    private float m_isCrushedDuration;

    [Header("Particle Effects")]

    [SerializeField, Tooltip("The particle effect prefab that will be instantiated when the player returns to their normal size after being crushed.")]
    private GameObject m_returnToNormalSizePoof;

    [SerializeField, Tooltip("The particle effect prefab that will be instantiated when the player gets crushed.")]
    private GameObject m_gotCrushedPoof;
    #endregion------------

    #region Standard Local Member Variables
    private bool m_isfacingLeft, m_isCrouched = false;

    //TODO Addition for stopping player movement when they're shoving
    private bool m_isShoving = false;

    // Used to determine whether the player was crushed or not
    private bool m_isCrushed = false;
    #endregion------------

    #region Component Variable Containers
    private ConveyorBelt m_conveyorBelt = null;
    private Rigidbody2D m_playerRigidbody;
    private SpriteRenderer m_playerSpriteRenderer;
    private GroundCheck m_groundCheck;
    private Animator m_playerAnim;

    // Player Systems
    private PlayerHealthSystem m_playerHealthSystem;
    // PlayerJumpHandler.MaxJumpSpeed is needed in order to clamp the player's velocity
    private PlayerJumpHandler m_playerJumpHandler;
    private PlayerShoveHandler m_shoveHandler;
    #endregion------------

    /// <summary>
    /// An internal class that contains all movement related input fields required for this script.
    /// 
    /// *QUICK NOTE* - I know I don't actually NEED an internal class for such little input, but
    ///     I'm just keeping it around as a simple example of how to make one just in case
    ///     I actually do end up needing one for another part of the project
    /// </summary>
    internal class InputListener
    {
        // Used to controler horizontal movement
        public float m_horizontalMoveInput;
        // Used for crouching
        public float m_verticalInput;
    }

    //Create a variable for the InputListener class
    private InputListener m_inputListener = new InputListener();

    //Used by the PlayerPhysicsMaterialHandler.cs script to see if the player is trying to move
    private bool m_playerIsNotMoving;
    public bool PlayerIsNotMoving
    {
        get
        {
            m_playerIsNotMoving = Mathf.Approximately(m_inputListener.m_horizontalMoveInput, 0.0f);

            //TODO: Debug for PlayerIsNotMoving property
            Debug.Log($"PlayerIsNotMoving == {m_playerIsNotMoving}");

            return m_playerIsNotMoving;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        //Set up all necessary component variables
        InitializePlayerComponents();
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_playerHealthSystem.IsAlive && !m_playerHealthSystem.IsBeingKnockedBack)
        {
            //--Listeners--
            if (m_shoveHandler.IsShoving == false)
            {
                HorizontalMoveInputListener();
                CrouchInputListener();
            }
            
            //If input to the horizontal axis is detected, update the look direction to keep the sprite facing the last direction the player moved in
            if (!Mathf.Approximately(m_inputListener.m_horizontalMoveInput, 0.0f))
                UpdateLookDirection();
        }
        else if ((!m_playerHealthSystem.IsAlive && !m_playerHealthSystem.IsBeingKnockedBack))
        {
            //m_inputListener.m_horizontalMoveInput = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        if (m_isShoving && !Mathf.Approximately(m_playerRigidbody.velocity.x, 0.0f))
        {
            StopPlayerMovement();
        }

        if (!m_playerHealthSystem.IsBeingKnockedBack && !m_shoveHandler.IsShoving)
        {
            // Apply horizontal player movement input.
            HorizontalMoveInputHandler();

            // Crouch
            CrouchInputHandler();
        }
    }
    
    /// <summary>
    /// Used to Initialize any necessary component variables (e.g. Rigidbody2D, BoxCollider2D, etc.)
    /// </summary>
    private void InitializePlayerComponents()
    {
        // Initialize Player Systems (e.g. health, jump, attack, etc.)
        m_playerHealthSystem = GetComponent<PlayerHealthSystem>();
        // Jump component is needed in order to properly clamp the player's max velocity on the x & y axis
        m_playerJumpHandler = GetComponent<PlayerJumpHandler>();
        m_shoveHandler = GetComponent<PlayerShoveHandler>();

        m_playerRigidbody = GetComponent<Rigidbody2D>();
        m_playerSpriteRenderer = GetComponent<SpriteRenderer>();
        m_groundCheck = GetComponentInChildren<GroundCheck>();
        m_playerAnim = GetComponent<Animator>();
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
    /// Listens for when the player is pressing down to crouch
    /// </summary>
    private void CrouchInputListener()
    {
        m_inputListener.m_verticalInput = Input.GetAxisRaw("Vertical");
    }
    #endregion

    #region ________________________________________________________________HANDLERS__________________________
    /// <summary>
    /// Handles and applies the horizontal input received from the player.
    /// 
    /// *NOTE* - This needs access to the MaxJumpSpeed property in the PlayerJumpHandler.
    /// </summary>
    private void HorizontalMoveInputHandler()
    {
        if (!Mathf.Approximately(m_inputListener.m_horizontalMoveInput, 0.0f))
        {
            if (m_isCrushed == false)
            {
                m_playerRigidbody.AddForce(Vector2.right * m_inputListener.m_horizontalMoveInput * m_runningAccelerationRate);
                Vector2 clampedVelocity = m_playerRigidbody.velocity;
                clampedVelocity.x = Mathf.Clamp(m_playerRigidbody.velocity.x, -m_maxMoveSpeed, m_maxMoveSpeed);
                clampedVelocity.y = Mathf.Clamp(m_playerRigidbody.velocity.y, Mathf.NegativeInfinity, m_playerJumpHandler.MaxJumpSpeed);
                m_playerRigidbody.velocity = clampedVelocity;
            }
            else if (m_isCrushed == true)
            {
                // This is the player's move speed after they've been crushed by an object; it causes the player's move speed to be halved
                m_playerRigidbody.AddForce(Vector2.right * (m_inputListener.m_horizontalMoveInput * 0.5f) * m_runningAccelerationRate);
                Vector2 clampedVelocity = m_playerRigidbody.velocity;
                clampedVelocity.x = Mathf.Clamp(m_playerRigidbody.velocity.x, -m_maxMoveSpeed * 0.5f, m_maxMoveSpeed * 0.5f);
                clampedVelocity.y = Mathf.Clamp(m_playerRigidbody.velocity.y, Mathf.NegativeInfinity, m_playerJumpHandler.MaxJumpSpeed);
                m_playerRigidbody.velocity = clampedVelocity;
            }
        }
        // IF NO MOVEMENT INPUT is detected but the player is still moving, then STOP PLAYER MOVEMENT
        else if (Mathf.Approximately(m_inputListener.m_horizontalMoveInput, 0.0f))
        {
            if ((m_groundCheck.IsGrounded || m_shoveHandler.IsShoving) && !m_groundCheck.IsOnMovingPlatform)
            {
                // Stop it!!!
                StopPlayerMovement();
            }
            if (m_groundCheck.IsOnMovingPlatform && m_conveyorBelt != null)
            {
                // If the player is not trying to move and the conveyor belt is off
                if (!m_conveyorBelt.GetComponent<ConveyorBelt>().ConveyorBeltActive)
                {
                    StopPlayerMovement();
                }
            }
        }
    }

    /// <summary>
    /// Listens for the crouching input
    /// </summary>
    private void CrouchInputHandler()
    {
        CrouchCheck();
        m_playerAnim.SetBool("IsCrouched", m_isCrouched);
    }

    /// <summary>
    /// Checks and handles what should happen when the player goes to crouch
    /// </summary>
    private void CrouchCheck()
    {
        // Reset variable so it represents the most current input
        m_isCrouched = m_inputListener.m_verticalInput < 0;
        
        if (m_isCrouched)
        {
            m_crouchingPlayerCollider.enabled = true;
            m_primaryPlayerCollider.enabled = false;
        }
        else
        {
            m_primaryPlayerCollider.enabled = true;
            m_crouchingPlayerCollider.enabled = false;
        }
    }
    #endregion

    /// <summary>
    /// Updates the direction the sprite should be facing based on the horizontal player input
    /// </summary>
    private void UpdateLookDirection()
    {
        if (!m_isCrushed)
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
            else if (m_inputListener.m_horizontalMoveInput >= -m_turningSpriteFlipThreshold && m_inputListener.m_horizontalMoveInput <= m_turningSpriteFlipThreshold)
            {
                if (!m_isfacingLeft)
                    m_playerSpriteRenderer.flipX = true;
                else
                    m_playerSpriteRenderer.flipX = false;
            }
        }
    }

    /// <summary>
    /// Used to instantly stop the player's movement
    /// </summary>
    private void StopPlayerMovement()
    {
        // Meant for Debugging Movement on Conveyorbelt Walk
        //Debug.Log($"Player is being stopped");

        Vector2 stoppingVelocity = m_playerRigidbody.velocity;

        // IF the player is NOT trying to Move and is NOT Jumping...
        if (Mathf.Approximately(m_inputListener.m_horizontalMoveInput, 0.0f) && !m_playerJumpHandler.IsJumping)
        {
            // IF the player IS on the Ground and NOT on a Moving Platform
            if (m_groundCheck.IsGrounded && !m_groundCheck.IsOnMovingPlatform)
            {
                stoppingVelocity.x = 0.0f;
                stoppingVelocity.y = 0.0f;
            }
            // ELSE IF the player IS on a Moving Platform and the reference to the ConveyorBelt script IS VALID
            else if (m_groundCheck.IsOnMovingPlatform && m_conveyorBelt.GetComponent<ConveyorBelt>() != null)
            {
                //m_conveyorBelt = m_conveyorBelt.GetComponent<ConveyorBelt>();

                // IF the belt's SurfaceEffector2D is active
                if (m_conveyorBelt.GetComponent<ConveyorBelt>().ConveyorBeltActive)
                {
                    stoppingVelocity.x = 8.0f; // <-- The value from the SurfaceEffector2D component on the conveyor belt object
                }
                else
                {
                    //stoppingVelocity.x = 0.0f;
                }
            }
        }
        
        m_playerRigidbody.velocity = stoppingVelocity;
    }

    IEnumerator IsCrushedTimer()
    {
        yield return new WaitForSecondsRealtime(m_isCrushedDuration);
        m_isCrushed = false;
        m_playerAnim.SetBool("IsCrushed", false);
        ParticlePoof(m_returnToNormalSizePoof);
    }

    private void ParticlePoof(GameObject particleSystem)
    {
        float offset;
        // Base the offset on the SpriteRenderer.FlipX value(the magic number 0.5f is meant to offset the particle effect ever so slightly)
        offset = (m_playerSpriteRenderer.flipX == true) ? 0.5f : -0.5f;

        // Instantiate poofing particle effect
        Instantiate(particleSystem, new Vector3(m_groundCheck.transform.position.x + offset, m_groundCheck.transform.position.y, 0.0f), particleSystem.transform.rotation);
    }

    #region OnTrigger Methods

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "MovingPlatforms")
        {
            if (collision.GetComponent<ConveyorBelt>() != null)
            {
                m_conveyorBelt = collision.GetComponent<ConveyorBelt>();

                // If the player is not trying to move and the conveyor belt is off
                if (Mathf.Approximately(m_inputListener.m_horizontalMoveInput, 0.0f) && !m_conveyorBelt.ConveyorBeltActive)
                {
                    StopPlayerMovement();
                }
            }
        }

        if (collision.tag == "Crusher")
        {
            if (collision.gameObject.GetComponent<Animator>() != null)
            {
                if (collision.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("CrushingPiston_Extended"))
                {
                    m_isCrushed = true;
                    m_playerAnim.SetBool("IsCrushed", true);
                    StartCoroutine("IsCrushedTimer");
                    ParticlePoof(m_gotCrushedPoof);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatforms")
        {
            if (collision.GetComponent<ConveyorBelt>() != null)
            {
                m_conveyorBelt = collision.GetComponent<ConveyorBelt>();

                // If the player is not trying to move and the conveyor belt is off
                if (Mathf.Approximately(m_inputListener.m_horizontalMoveInput, 0.0f) && !m_conveyorBelt.ConveyorBeltActive)
                {
                    StopPlayerMovement();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatforms")
        {
            if (m_conveyorBelt != null)
            {
                m_conveyorBelt = null;
            }
        }
    }
    #endregion
}