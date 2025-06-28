using System.Collections.Generic;
using UnityEngine;

public interface IGridObject
{
    IReadOnlyList<Vector2Int> Tiles { get; }
    Faction                   Faction { get; }
}

public interface IDamageable : IGridObject
{
    void ApplyDamage(DamageEvent dmg);
}

public interface IAttackSource : IGridObject
{
    AttackDescriptor AttackData { get; }
}
