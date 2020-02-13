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
    private float m_onTime;

    [SerializeField, Tooltip("How Long will the motion sensor be off for?")]
    private float m_offTime;

    [SerializeField, Tooltip("How long will the player have to stop moving or escape before the crusher does it's thing?")]
    private float m_playerBufferTime;

    private float m_bufferTimer = 0.0f;
    private float m_timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_detectionZoneTrigger = GetComponentInChildren<PolygonCollider2D>();
        m_detectorAnim = GetComponent<Animator>();

        ResetCooldownTimer();
    }

    // Update is called once per frame
    void Update()
    {
        // Camera WAS OFF and is being TURNED ON
        if (Time.time > m_offTime)
        {
            m_detectorAnim.SetBool("IsOn", true);
        }

        // Camera WAS ON and is being TURNED OFF
        if (Time.time > m_onTime)
        {
            m_detectorAnim.SetBool("IsOn", false);
            ResetCooldownTimer();
        }
    }

    private void ResetCooldownTimer()
    {
        m_timer = Time.time + m_offTime;
    }

    private void ResetActiveTimer()
    {
        m_timer = Time.time + m_onTime;
    }
}


// Psuedo-Code
/*
 *  - Wait for a period of time
 *  - Turn on and look for the player
 *      - IsOn = true;
 *      - DetectionTriggerVolume is On/Armed
 *  - Continue looking for player for a set period of time
 *  - If player enters the detection zone and is moving then wait a short period of time
 *  - Crush player if theyre still moving in the detection zone after the short period of time is up
 *  - Else If IsOn == false and IsAlerted == false then return to original state
 * 
 */