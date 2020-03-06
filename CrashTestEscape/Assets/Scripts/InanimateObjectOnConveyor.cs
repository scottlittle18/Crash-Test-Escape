using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InanimateObjectOnConveyor : MonoBehaviour
{
    private Rigidbody2D m_inanimateRigidbody;

    [SerializeField, Tooltip("This is the conveyor belt that this object will be moved by.")]
    private ConveyorBelt m_assignedConveyor;

    [SerializeField, Tooltip("How long after entering a KillZone will this object be destroyed?.")]
    private float m_selfDestructTimer = 1.0f;

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSecondsRealtime(m_selfDestructTimer);
        Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_inanimateRigidbody = GetComponent<Rigidbody2D>();
        m_inanimateRigidbody.freezeRotation = true;
        m_assignedConveyor = GameObject.FindObjectOfType<ConveyorBelt>().GetComponent<ConveyorBelt>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_assignedConveyor.ConveyorBeltActive && m_inanimateRigidbody.freezeRotation != true)
        {
            Vector2 stoppingVelocity = m_inanimateRigidbody.velocity;
            stoppingVelocity = new Vector2(0.0f, m_inanimateRigidbody.velocity.y);
            m_inanimateRigidbody.velocity = stoppingVelocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "KillZone")
        {
            StartCoroutine("SelfDestruct");
        }
    }
}
