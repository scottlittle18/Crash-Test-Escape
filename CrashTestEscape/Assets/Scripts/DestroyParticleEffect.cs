using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleEffect : MonoBehaviour
{
    private ParticleSystem m_thisParticleEffect;

    // Start is called before the first frame update
    void Start()
    {
        m_thisParticleEffect = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy the instantiated particle system once it's finished playing
        if (m_thisParticleEffect.isPlaying)
            return;

        Destroy(this.gameObject);
    }
}
