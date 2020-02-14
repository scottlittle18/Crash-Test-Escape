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
    
    private float m_timer = 0.0f;

    private void Awake()
    {
        m_detectionZoneTrigger = GetComponent<PolygonCollider2D>();
        m_detectorAnim = GetComponentInParent<Animator>();

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
            m_detectionZoneTrigger.enabled = false;
        }
    }

    private void ResetBaseTimer()
    {
        m_timer = Time.time + m_onTime;
    }
}