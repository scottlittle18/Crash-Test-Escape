using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to control the intermittent movement of the conveyor belt
/// </summary>
public class ConveyorBelt : MonoBehaviour
{
    /// <summary>
    /// How long should the conveyor belt stop for. Default value == 1.
    /// </summary>
    [SerializeField, Tooltip("This is the BoxCollider2D that is used by the SurfaceEffector2D component. Default value == 1")]
    private float m_movementDelay = 1;

    [SerializeField]
    private GameObject m_conveyorMovementObject;

    private BoxCollider2D m_mainConveyorCollider;

    private float m_colliderOriginalFriction;

    private bool m_conveyorBeltActive;
    public bool ConveyorBeltActive
    {
        get { return m_conveyorBeltActive; }
        private set
        {
            m_conveyorBeltActive = m_conveyorMovementObject.activeSelf;
        }
    }

    [SerializeField]
    private float m_stoppedFriction = 1; // Default Value == 1

    private float m_conveyerTimer;

    private void Awake()
    {
        m_mainConveyorCollider = GetComponent<BoxCollider2D>();
        m_colliderOriginalFriction = m_mainConveyorCollider.sharedMaterial.friction;
        ResetConveyorDelay();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > m_conveyerTimer)
        {
            m_conveyorMovementObject.SetActive(!m_conveyorMovementObject.gameObject.activeSelf);
            ResetConveyorDelay();
        }

        if (m_conveyorMovementObject.gameObject.activeSelf == false)
        {
            // If the conveyor belt is not active, set the friction of the material to a higher number
            m_mainConveyorCollider.sharedMaterial.friction = m_stoppedFriction;
        }
        else if (m_conveyorMovementObject.gameObject.activeSelf == true)
        {
            m_mainConveyorCollider.sharedMaterial.friction = 0.0f;
        }
    }

    private void ResetConveyorDelay()
    {
        m_conveyerTimer = Time.time + m_movementDelay;
    }
}
