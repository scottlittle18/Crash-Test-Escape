using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InanimateObjectOnConveyor : MonoBehaviour
{
    private Rigidbody2D m_inanimateRigidbody;

    [SerializeField, Tooltip("This is the conveyor belt that this object will be moved by.")]
    private ConveyorBelt m_assignedConveyor;

    // Start is called before the first frame update
    void Start()
    {
        m_inanimateRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_assignedConveyor.ConveyorBeltActive)
        {
            Vector2 stoppingVelocity = m_inanimateRigidbody.velocity;
            stoppingVelocity = new Vector2(0.0f, m_inanimateRigidbody.velocity.y);
            m_inanimateRigidbody.velocity = stoppingVelocity;
        }
    }
}
