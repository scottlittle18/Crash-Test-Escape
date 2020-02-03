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

    private void Awake()
    {
        m_groundCheckCollider = GetComponent<BoxCollider2D>();

        // If the ground check collider was not set to be a trigger in the inspector, set it to be one
        if (m_groundCheckCollider.isTrigger != true)
        {
            m_groundCheckCollider.isTrigger = true;
        }
    }

    //private void Update()
    //{
    //    //Physics2D.Overlap Method -- Using a normal collider instead for accuracy
    //    IsGrounded == true if the player is on an object in the "Ground" LayerMask.
    //    IsGrounded = Physics2D.OverlapCircle(this.transform.position, groundCheckRadius, whatIsGround);
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            IsGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            IsGrounded = false;
        }
    }
}
