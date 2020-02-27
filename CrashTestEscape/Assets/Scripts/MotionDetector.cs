﻿using System;
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
    private float m_onTime;

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

    private void ResetBaseTimer()
    {
        m_timer = Time.time + m_onTime;
    }

    IEnumerator PistonSmashDelay()
    {
        Debug.Log("Waiting to crush Player...");
        yield return new WaitForSecondsRealtime(m_pistonBufferTime);
        m_pistonAnim.SetBool("IsActive", true);
        Debug.Log("CRUSHING PLAYER!!!");
        yield return new WaitForSecondsRealtime(m_pistonAnim.GetCurrentAnimatorClipInfo(0).Length * 0.5f); // 0.5f used to keep the animation from running more than once.
        m_pistonAnim.SetBool("IsActive", false);
        Debug.Log("Stop Crushing Player");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.GetComponent<PlayerMovementHandler>() != null)
            {
                // TODO: Debugging the motion detection system.
                Debug.Log($"Motion sensor sees the player as moving: {collision.GetComponent<PlayerMovementHandler>().PlayerIsNotMoving}");

                if (!collision.GetComponent<PlayerMovementHandler>().PlayerIsNotMoving && !Mathf.Approximately(collision.GetComponent<Rigidbody2D>().velocity.x, 0.0f))
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