using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface used for hazards (spikes, car crash test zones, etc.)
/// </summary>
public interface IHazardous
{
    void CauseDamage(int damageToCause);
}
