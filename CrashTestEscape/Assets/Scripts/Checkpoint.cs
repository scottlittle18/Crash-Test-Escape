using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// Purpose:
///     This is used to keep track of which checkpoint the player will respawn at upon death.
/// </summary>
public class Checkpoint : MonoBehaviour
{    
    private bool m_isActivated;

    //TODO: Animate Checkpoint
    //private Animator m_anim;

    private PlayerHealthSystem m_playerHealth;
    /// <summary>
    /// Property Used to Activate or Deactivate checkpoints
    /// </summary>
    public bool IsActivated
    {
        get
        {
            return m_isActivated;
        }
        set
        {
            m_isActivated = value;
            //TODO: Animate Checkpoint
            //UpdateAnimation();
        }
    }

    private void Awake()
    {
        //TODO: Animate Checkpoint
        //m_anim = GetComponent<Animator>();
    }

    private void Start()
    {
        IsActivated = false;        
    }

    /// <summary>
    /// Update the animator of the checkpoint.
    /// </summary>
    private void UpdateAnimation()
    {
        //TODO: Animate Checkpoint
        //m_anim.SetBool("isActivated", IsActivated);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_playerHealth = collision.GetComponent<PlayerHealthSystem>();
            m_playerHealth.CurrentCheckpoint = this;
        }
    }
}
