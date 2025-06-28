using UnityEngine;

[CreateAssetMenu(menuName = "Combat/AttackDescriptor")]
public class AttackDescriptor : ScriptableObject
{
    [Min(0)] public int   damage    = 1;
    public   float        knockBack = 0f;
    public   StatusEffect statusEffect;

    public DamageEvent BuildDamageEvent() =>
        new DamageEvent(damage, knockBack, statusEffect);
}
 