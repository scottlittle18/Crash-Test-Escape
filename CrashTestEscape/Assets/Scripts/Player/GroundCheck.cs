using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to detect if the attached character is on the ground
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

    private void Update()
    {
        //IsGrounded == true if the player is on an object in the "Ground" LayerMask.
        //IsGrounded = Physics2D.OverlapCircle(this.transform.position, groundCheckRadius, whatIsGround);
    }

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
