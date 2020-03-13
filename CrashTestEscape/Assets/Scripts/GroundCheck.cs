using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to detect if the attached character is on the ground
/// 
/// Implamentation Steps:
///     - Create an Empty Child GameObject for the GameObject that will be using this
///     - (OPTIONAL) Name the Empty Child GameObject that was created in the previous step "GroundCheck"
///     - Add a BoxCollider2D component to the Empty Child Object created in the first step
///     - Set the new BoxCollider2D component's IsTrigger property to true in-editor
///     - Edit the BoxCollider2D so that it is situated at the bottom of the parent Gameobject and is the correct size
///     - **IMPORTANT NOTE** - Ensure that the BoxCollider2D doesn't stop short of the parent GameObject's non-trigger collider!
///     - Create a new tag specifically named "Ground"
///     - Create a new layer specifically named "Ground"
///     - Test to see if it works
///     - Debug if it doesn't; Otherwise, you're done! ☻
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class GroundCheck : MonoBehaviour
{
    private BoxCollider2D m_groundCheckCollider;
    private Animator m_playerAnim;

    //This allows the IsGrounded variable to be read from the associated object's movement script.
    private bool m_isGrounded;
    /// <summary>
    /// Used to check if the object that this is attatched to is on the ground or not
    /// </summary>
    public bool IsGrounded
    {
        get { return m_isGrounded; }
        private set { m_isGrounded = value; }
    }

    //This allows the IsOnMovingPlatform variable to be read from the associated object's movement script.
    private bool m_isOnMovingPlatform;
    /// <summary>
    /// Used to determine whether or not the player is on a moving platform or conveyor belt
    /// </summary>
    public bool IsOnMovingPlatform
    {
        get { return m_isOnMovingPlatform; }
        private set { m_isOnMovingPlatform = value; }
    }

    private void Awake()
    {
        m_groundCheckCollider = GetComponent<BoxCollider2D>();
        m_playerAnim = transform.parent.GetComponent<Animator>();

        // If the ground check collider was not set to be a trigger in the inspector, set it to be one
        if (m_groundCheckCollider.isTrigger != true)
        {
            m_groundCheckCollider.isTrigger = true;
        }
    }

    private void Update()
    {
        m_playerAnim.SetBool("IsGrounded", IsGrounded);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("Player is on the ground");
            IsGrounded = true;
            IsOnMovingPlatform = false;
        }

        if (collision.gameObject.tag == "MovingPlatforms")
        {
            Debug.Log("Player is on a moving platform");
            IsGrounded = false;
            IsOnMovingPlatform = true;
        }

        if (collision.gameObject.tag == "ImmobileDummy")
        {
            Debug.Log("Player is on an Immobile Dummy");
            IsGrounded = true;
            IsOnMovingPlatform = true;
        }

        if (collision.gameObject.tag == "Environmental Props")
        {
            Debug.Log("Player is on an Environmental Prop");
            IsGrounded = true;
            IsOnMovingPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("Player is NOT on the ground");
            IsGrounded = false;
            IsOnMovingPlatform = false;
        }

        if (collision.gameObject.tag == "MovingPlatforms")
        {
            Debug.Log("Player is NOT on a moving platform");
            IsGrounded = false;
            IsOnMovingPlatform = false;
        }

        if (collision.gameObject.tag == "ImmobileDummy")
        {
            Debug.Log("Player is NOT on an Immobile Dummy");
            IsGrounded = false;
            IsOnMovingPlatform = false;
        }

        if (collision.gameObject.tag == "Environmental Props")
        {
            Debug.Log("Player is NOT on an Environmental Prop");
            IsGrounded = false;
            IsOnMovingPlatform = false;
        }
    }
}
