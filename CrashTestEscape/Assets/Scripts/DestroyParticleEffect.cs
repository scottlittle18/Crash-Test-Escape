using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleEffect : MonoBehaviour
{
    private ParticleSystem m_thisParticleEffect;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Particle poof debug
        Debug.Log("I AM ALIVE!!!");
        m_thisParticleEffect = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy the instantiated particle system once it's finished playing
        if (m_thisParticleEffect.isPlaying)
        {
            Debug.Log("I'm still alive!");

            return;
        }
        else
        {
            Debug.Log("I die now...");

            Destroy(this.gameObject);
        }
    }
}
