using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthSystem : MonoBehaviour, IDamageable
{
    [Header("HP")]
    public int maxHP = 10;
    public Faction faction;

    public int CurrentHP => hp;            // ← NEW
    public int MaxHP => maxHP;         // ← NEW
    public event System.Action<int, int> HealthChanged;   // current , max

    private int hp;
    private Vector2Int tile;
    private static readonly List<Vector2Int> single = new(1) { default };

    void Start()
    {
        hp = maxHP;
        tile = GridManager.I.WorldToTile(transform.position);
        Debug.Log($"HealthSystem Start: {gameObject.name} ({faction}) positioned at world {transform.position}, tile {tile}");
        GridManager.I.UpdateOccupancy(this, System.Array.Empty<Vector2Int>(), new[] { tile });

        HealthChanged?.Invoke(hp, maxHP);   // initial broadcast  ← NEW
    }

    public void ApplyDamage(DamageEvent dmg)
    {
        Debug.Log($"HealthSystem: {gameObject.name} ({faction}) taking {dmg.amount} damage at tile {tile}. HP: {hp} -> {hp - dmg.amount}");
        hp -= dmg.amount;
        HealthChanged?.Invoke(hp, maxHP);   // ← NEW

        if (hp <= 0)
        {
            Debug.Log($"HealthSystem: {gameObject.name} ({faction}) destroyed at tile {tile}");
            Destroy(gameObject);
        }
    }

    /* — IGridObject — */
    public IReadOnlyList<Vector2Int> Tiles { get { single[0] = tile; return single; } }
    public Faction Faction => faction;

    void OnDestroy()
    {
        GridManager.I.UpdateOccupancy(this, new[] { tile }, System.Array.Empty<Vector2Int>());
    }
public void MoveToTile(Vector2Int newTile)
{
    Debug.Log($"HealthSystem: {gameObject.name} ({faction}) moving from tile {tile} to {newTile}");
    GridManager.I.UpdateOccupancy(this, new[]{ tile }, new[]{ newTile });
    tile = newTile;
}

}
