using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is meant to be used on spike hazards
/// </summary>
public class SpikePitHazard : MonoBehaviour
{
    [SerializeField, Tooltip("How much damage will the player take from touching this hazard?")]
    private int m_damage = 0;

    [SerializeField, Tooltip("How powerful will the UPWARD knockback force that will be applied to the player be?")]
    private float m_upwardKnockbackForce = 0.0f;

    [SerializeField, Tooltip("How powerful will the LATERAL knockback force that will be applied to the player be?")]
    private float m_lateralKnockbackForce = 0.0f;

    [SerializeField, Tooltip("How long will the player be immobillized for after being knockedback?")]
    private float m_knockbackTimer = 0.0f;

    //TODO: Replace with the proper collider type.
    private Collider2D m_spikeCollider;

    private void Awake()
    {
        CheckForZeroValues();

        m_spikeCollider = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Used to assign default values to variables in case we forget to set it in the editor while testing
    /// </summary>
    private void CheckForZeroValues()
    {
        // Check if a proper value has been assigned to the Damage variable in the editor, if it's not then assign it a value of 1
        if (m_damage <= 0)
        {
            Debug.Log("Default Damage Value Used!");
            m_damage = 1;
        }

        if (m_upwardKnockbackForce <= 0)
        {
            Debug.Log("Default Knockback Force Used!");
            m_upwardKnockbackForce = 1;
        }

        if( m_knockbackTimer <= 0)
        {
            Debug.Log("Default Knockback Timer Length Applied!");
            m_knockbackTimer = 1.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Detected!");
            // Used to determine which side of the hazard the player is on
            bool isOnLeftSide = false;

            if (collision.gameObject.transform.position.x <= transform.position.x)
            {
                isOnLeftSide = true;
            }
            else if (collision.gameObject.transform.position.x > transform.position.x)
            {
                isOnLeftSide = false;
            }
            else
            {
                // TODO: Debug.Log
                Debug.Log("The Player's position could not be determined");
            }

            collision.gameObject.SendMessage("TakeDamage", m_damage);
            collision.gameObject.GetComponent<PlayerHealthSystem>().DetermineAndApplyKnockbackForce(m_upwardKnockbackForce, m_lateralKnockbackForce, isOnLeftSide, m_knockbackTimer);
            //collision.gameObject.SendMessage("ApplyKnockbackForce", m_knockbackForce, isOnLeftSide);
            //collision.gameObject.SendMessage("ApplyKnockbackForce", isOnLeftSide);
        }
    }

    // TODO: Determine how to make this object's collider detect the player
    // Ideas:
    //  - Don't make this object's collider a trigger and instead as a PhysMat that would make it slightly bouncy,
    //      then stop player input for a moment while it applies a slight backward knockback force (TBD based
    //      on what is needed for each spike hazard). This also works because we don't want the player
    //      to be able to walk through a spike trap.
}
