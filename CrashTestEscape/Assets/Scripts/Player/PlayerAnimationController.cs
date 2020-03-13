using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Rigidbody2D m_playerRigidbody;
    private float horizontal_MoveInput;
    private Animator m_playerAnim;

    private void Awake()
    {
        m_playerRigidbody = GetComponent<Rigidbody2D>();
        m_playerAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        horizontal_MoveInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        m_playerAnim.SetFloat("Horizontal_MoveSpeed", Mathf.Abs(horizontal_MoveInput));
        m_playerAnim.SetFloat("Vertical_MoveSpeed", m_playerRigidbody.velocity.y);
    }
}
