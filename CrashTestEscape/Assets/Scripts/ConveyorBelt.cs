using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
    
    /// <summary>
    /// The object that actually applies the conveyor belt effect using a SurfaceEffector2D and a BoxCollider2D set to use effector.
    /// 
    /// *NOTE* - This value must be manually assigned in the editor since there are two colliders on this object.
    /// </summary>
    private GameObject m_conveyorMovementObject;
    
    private float m_conveyerTimer;

    private bool m_conveyorBeltActive;
    public bool ConveyorBeltActive
    {
        get { return m_conveyorBeltActive = m_conveyorMovementObject.activeSelf; }
    }

    private void Awake()
    {
        m_conveyorMovementObject = transform.GetChild(0).gameObject;

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
    }

    private void ResetConveyorDelay()
    {
        m_conveyerTimer = Time.time + m_movementDelay;
    }
}
