using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour, IAttackSource
{
    // ───── Combat data ────────────────────────────────────────────────
    [SerializeField] AttackDescriptor attack;
    [SerializeField] Faction          faction;
    [SerializeField] Vector2Int       direction = Vector2Int.right;

    // ───── Movement ───────────────────────────────────────────────────
    [Tooltip("Tiles crossed per second.  Adjust in Inspector or per prefab.")]
    [Min(0.1f)]
    [SerializeField] float tilesPerSecond = 10f;

    Vector2Int tile;          // current grid tile
    float      accumulator;   // ∑ fixedDeltaTime · tilesPerSecond
    static readonly List<Vector2Int> single = new(1) { default };

    public void Init(AttackDescriptor atk, Faction fac, Vector2Int dir)
    {
        Debug.Log($"Projectile Init: Received AttackDescriptor: {(atk == null ? "NULL" : atk.name)}, faction: {fac}");
        
        attack    = atk;
        faction   = fac;
        direction = dir;

        // Initialize tile and move one step immediately
        tile = GridManager.I.WorldToTile(transform.position);
        Debug.Log($"Projectile Init: Starting at world pos {transform.position}, tile {tile}, faction {faction}");
        Debug.Log($"Projectile Init: AttackData after assignment: {(attack == null ? "NULL" : attack.name)}");
        StepOneTile();
    }

    // ───── Unity life-cycle ───────────────────────────────────────────
    void Start()
    {
        // Removed GridManager.I.AddAttack(this, tile); as it's now handled in Init()
    }

    void FixedUpdate()
    {
        accumulator += Time.fixedDeltaTime * tilesPerSecond;

        while (accumulator >= 1f)
        {
            accumulator -= 1f;
            StepOneTile();
        }
    }

    void StepOneTile()
    {
        GridManager.I.RemoveAttack(this, tile);

        tile += direction;
        if (!GridManager.I.IsInsideGrid(tile))
        {
            Debug.Log($"Projectile destroyed: Outside grid at tile {tile}");
            Destroy(gameObject);
            return;
        }

        transform.position = GridManager.I.GetWorldPos(tile);
        GridManager.I.AddAttack(this, tile);

        Debug.Log($"Projectile moved to tile {tile}, checking for targets...");
        
        // Log what's on this tile before combat resolution
        var pairs = GridManager.I.GetPairsToResolveForTile(tile);
        var pairsList = new System.Collections.Generic.List<CombatPair>(pairs);
        Debug.Log($"Found {pairsList.Count} combat pairs on tile {tile}");

        // Resolve combat immediately upon entering a new tile
        foreach (var pair in pairsList)
        {
            Debug.Log($"Combat: {pair.attack.Faction} attacking {pair.target.Faction} on tile {tile}");
            
            // Debug null reference issue
            if (pair.attack == null)
            {
                Debug.LogError("pair.attack is null!");
                continue;
            }
            if (pair.attack.AttackData == null)
            {
                Debug.LogError($"pair.attack.AttackData is null for {pair.attack.GetType().Name}!");
                continue;
            }
            
            Debug.Log($"Damage amount: {pair.attack.AttackData.damage}");
            pair.target.ApplyDamage(pair.attack.AttackData.BuildDamageEvent());
            Destroy(gameObject); // Destroy projectile after hit
            return; // Exit after first hit
        }
        
        if (pairsList.Count == 0)
        {
            Debug.Log($"No valid targets found on tile {tile} for {faction} projectile");
        }
    }

    void OnDestroy()
    {
        Debug.Log($"Projectile destroyed at tile {tile}");
        if (GridManager.I != null && GridManager.I.IsInsideGrid(tile))
            GridManager.I.RemoveAttack(this, tile);
    }

    // ───── IAttackSource / IGridObject implementation ────────────────
    public AttackDescriptor          AttackData => attack;
    public Faction                   Faction    => faction;
    public IReadOnlyList<Vector2Int> Tiles      { get { single[0] = tile; return single; } }
}
