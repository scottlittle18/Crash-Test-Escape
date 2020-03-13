using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoveHandler : MonoBehaviour
{
    private Animator m_playerAnim;

    private bool m_shoveInput;
    private bool m_isShoving;
    public bool IsShoving
    {
        get { return m_isShoving; }
        set { m_isShoving = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        m_playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ShoveInputListener();
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
