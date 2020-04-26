using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoveHandler : MonoBehaviour
{
    private Animator m_playerAnim;
    private Rigidbody2D m_playerBody;

    private bool m_shoveInput;
    private bool m_isShoving = false;
    public bool IsShoving
    {
        get { return m_isShoving; }
        set { m_isShoving = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        m_playerAnim = GetComponent<Animator>();
        m_playerBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ShoveInputListener();
    }

    private void FixedUpdate()
    {
        if (IsShoving)
        {
            m_playerBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else if (!IsShoving)
        {
            m_playerBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    /// <summary>
    ///  Listens for the attack/shove input
    /// </summary>
    public void ShoveInputListener()
    {
        m_shoveInput = Input.GetButtonDown("Fire1");

        if (m_shoveInput)
        {
            m_playerAnim.ResetTrigger("StopShoving");
            m_playerAnim.SetTrigger("Shove");
            m_isShoving = true;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            m_playerAnim.ResetTrigger("Shove");
            m_playerAnim.SetTrigger("StopShoving");
            m_isShoving = false;
        }
    }
}
