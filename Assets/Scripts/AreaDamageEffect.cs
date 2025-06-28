using UnityEngine;
using System;

[Serializable]
public struct AreaDamageEffect
{
    public Vector2Int offset;
    [Min(0)] public int damage;
    public StatusEffect statusEffect;

    public DamageEvent BuildDamageEvent() =>
        new DamageEvent(damage, 0f, statusEffect); // Knockback is 0 for area effects for now
}