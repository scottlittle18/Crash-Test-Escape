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
    private float m_stoppingFriction;
    
    private GameObject m_conveyorMovementObject;

    [SerializeField, Tooltip("This is the collider that is not a trigger.")]
    private BoxCollider2D m_mainConveyorCollider;
    
    private float m_conveyerTimer;

    private bool m_conveyorBeltActive;
    public bool ConveyorBeltActive
    {
        get { return m_conveyorBeltActive; }
        private set
        {
            m_conveyorBeltActive = m_conveyorMovementObject.activeSelf;
        }
    }

    private void Awake()
    {
        m_conveyorMovementObject = transform.GetChild(0).gameObject;

        // Manually placing since there are now two colliders on this object
        //m_mainConveyorCollider = GetComponent<BoxCollider2D>();

        // This fixes a bug that causes the conveyor belt to malfunction and apply no friction
        //      if the friction of the PhysMat2D attatched to this object equals 0
        if (m_mainConveyorCollider.sharedMaterial.friction == 0)
        {
            // The value of 0.5f was chosen based on testing and should be changed if any adjustments are made to the original PhysMat2D
            m_mainConveyorCollider.sharedMaterial.friction = m_stoppingFriction;
        }

        //ResetConveyorDelay();
    }

    private void Start()
    {
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
            m_mainConveyorCollider.sharedMaterial.friction = m_stoppingFriction;
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
