using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to control the motion detection systems and animations
/// </summary>
public class MotionDetector : MonoBehaviour
{
    private PolygonCollider2D m_detectionZoneTrigger;
    private Animator m_detectorAnim;

    [SerializeField, Tooltip("How Long will the motion sensor be active for?")]
    private float m_detectorDuration;

    [SerializeField, Tooltip("How Long will the motion sensor wait before activating the crusher?")]
    private float m_pistonBufferTime = 0.25f;

    [SerializeField, Tooltip("The Animator that controls the piston that's paired with this MotionDetection System?")]
    private Animator m_pistonAnim;

    private float m_timer = 0.0f;

    private void Awake()
    {
        m_detectionZoneTrigger = GetComponent<PolygonCollider2D>();
        m_detectorAnim = GetComponentInParent<Animator>();
        m_pistonAnim.SetBool("IsActive", false);

        ResetBaseTimer();
    }

    // Update is called once per frame
    void Update()
    {
        // MotionDetector Animation state switching
        if (Time.time > m_timer)
        {
            m_detectorAnim.SetBool("IsOn", !m_detectorAnim.GetBool("IsOn"));
            ResetBaseTimer();
        }

        UpdateDetectionZone();
    }

    /// <summary>
    /// Controls when the trigger volume that makes up the detection zone turns on
    /// </summary>
    private void UpdateDetectionZone()
    {
        if (m_detectorAnim.GetBool("IsOn"))
        {
            m_detectionZoneTrigger.enabled = true;
        }
        else if (!m_detectorAnim.GetBool("IsOn"))
        {
            // Reset the IsAlerted state of the MotionDetector Animator
            if (m_detectorAnim.GetBool("IsAlerted"))
            {
                m_detectorAnim.SetBool("IsAlerted", false);
            }
            m_detectionZoneTrigger.enabled = false;
        }
    }

    /// <summary>
    /// Reset the timer associated with the motion detector
    /// </summary>
    private void ResetBaseTimer()
    {
        m_timer = Time.time + m_detectorDuration;
    }

    /// <summary>
    /// Controls the length of the delay between when the motion detector is alerted to the player and when the piston will come down to crush them
    /// </summary>
    /// <returns></returns>
    IEnumerator PistonSmashDelay()
    {
        // Wait before crushing
        yield return new WaitForSecondsRealtime(m_pistonBufferTime);

        // If the motion detector is still alerted to the player's presence then they can crush them
        if (m_detectorAnim.GetBool("IsAlerted"))
        {
            m_pistonAnim.SetBool("IsActive", true);
        }

        // Wait before retracting
        yield return new WaitForSecondsRealtime(m_pistonAnim.GetCurrentAnimatorClipInfo(0).Length * 0.5f); // 0.5f used to keep the animation from running more than once.

        // Retract Piston
        m_pistonAnim.SetBool("IsActive", false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.GetComponent<PlayerMovementHandler>() != null)
            {
                // TODO: Debugging the motion detection system.
                Debug.Log($"Motion sensor sees the player as moving: {collision.GetComponent<PlayerMovementHandler>().PlayerIsNotMoving}");

                if (!collision.GetComponent<PlayerMovementHandler>().PlayerIsNotMoving)
                {
                    // If the motion detector is on
                    if (m_detectorAnim.GetBool("IsOn"))
                    {
                        m_detectorAnim.SetBool("IsAlerted", true);
                        StartCoroutine("PistonSmashDelay");
                    }
                }
                else
                {
                    // If the motion detector is on
                    if (m_detectorAnim.GetBool("IsOn"))
                    {
                        m_detectorAnim.SetBool("IsAlerted", false);
                        StopCoroutine("PistonSmashDelay");
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (m_detectorAnim.GetBool("IsOn"))
            {
                m_detectorAnim.SetBool("IsAlerted", false);
                StopCoroutine("PistonSmashDelay");
            }
        }
    }
}