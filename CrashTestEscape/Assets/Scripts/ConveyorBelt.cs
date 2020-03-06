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

    [SerializeField, Tooltip("This is the list of objects that will be spawned onto the conveyor belt.")]
    private GameObject[] m_spawnableObjectList;

    [SerializeField, Tooltip("This is the transform of the object that marks the spawn location.")]
    private Transform m_spawnerTransform;

    /// <summary>
    /// The object that actually applies the conveyor belt effect using a SurfaceEffector2D and a BoxCollider2D set to use effector.
    /// 
    /// *NOTE* - This value must be manually assigned in the editor since there are two colliders on this object.
    /// </summary>
    private GameObject m_conveyorMovementObject;

    private GameObject m_spawnedParentObject;

    private int m_lastObjectSpawned = 0;

    private float m_conveyerTimer;

    private bool m_conveyorBeltActive;
    public bool ConveyorBeltActive
    {
        get { return m_conveyorBeltActive = m_conveyorMovementObject.activeSelf; }
    }

    private void Awake()
    {
        m_conveyorMovementObject = transform.GetChild(0).gameObject;
        m_spawnedParentObject = GameObject.Find("Environmental Assets");

        ResetConveyorDelay();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > m_conveyerTimer)
        {
            m_conveyorMovementObject.SetActive(!m_conveyorMovementObject.gameObject.activeSelf);

            SpawnNewConveyorObject();

            ResetConveyorDelay();
        }
    }

    /// <summary>
    /// Used to spawn a new object when the conveyor belt's SurfaceEffected Collider turns on.
    /// </summary>
    private void SpawnNewConveyorObject()
    {
        if (ConveyorBeltActive)
        {
            if (m_lastObjectSpawned == 0)
            {
                Instantiate(m_spawnableObjectList[1], new Vector3(m_spawnerTransform.position.x, m_spawnerTransform.position.y, 0.0f), Quaternion.identity, m_spawnedParentObject.transform);
                m_lastObjectSpawned = 1;
            }
            else if (m_lastObjectSpawned == 1)
            {
                Instantiate(m_spawnableObjectList[0], new Vector3(m_spawnerTransform.position.x, m_spawnerTransform.position.y, 0.0f), Quaternion.identity, m_spawnedParentObject.transform);
                m_lastObjectSpawned = 0;
            }
        }
    }

    private void ResetConveyorDelay()
    {
        m_conveyerTimer = Time.time + m_movementDelay;
    }
}
