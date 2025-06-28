using UnityEngine;
using System.Collections.Generic;
using System; // Required for [Serializable]

public enum AttackType
{
    SingleTarget,
    VerticalSplash,
    // Add more attack types as needed
}

[CreateAssetMenu(menuName = "Combat/AttackDescriptor")] // Moved attribute back to class
public class AttackDescriptor : ScriptableObject
{
    public AttackType attackType = AttackType.SingleTarget;
    [Min(0)] public int   damage    = 1;
    public   float        knockBack = 0f;
    public   StatusEffect statusEffect;
    public List<AreaDamageEffect> areaEffects;

    public DamageEvent BuildDamageEvent() =>
        new DamageEvent(damage, knockBack, statusEffect);
}
 