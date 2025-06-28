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
    Vector3    _startWorldPos; // World position at the start of the current tile movement
    Vector3    _targetWorldPos; // World position at the end of the current tile movement
    static readonly List<Vector2Int> single = new(1) { default };

    public void Init(AttackDescriptor atk, Faction fac, Vector2Int dir)
    {
        Debug.Log($"Projectile Init: Received AttackDescriptor: {(atk == null ? "NULL" : atk.name)}, faction: {fac}");
        
        attack    = atk;
        faction   = fac;
        direction = dir;

        // Initialize tile and move one step immediately
        tile = GridManager.I.WorldToTile(transform.position);
        _startWorldPos = GridManager.I.GetWorldPos(tile); // Set initial start position
        _targetWorldPos = GridManager.I.GetWorldPos(tile + direction); // Set initial target position
        Debug.Log($"Projectile Init: Starting at world pos {transform.position}, tile {tile}, faction {faction}");
        Debug.Log($"Projectile Init: AttackData after assignment: {(attack == null ? "NULL" : attack.name)}");
        StepOneTile(); // Perform the first logical step
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
            if (gameObject == null) return; // Projectile might be destroyed after StepOneTile
        }

        // Smoothly interpolate visual position
        transform.position = Vector3.Lerp(_startWorldPos, _targetWorldPos, accumulator);
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

        // Update world positions for smooth interpolation
        _startWorldPos = GridManager.I.GetWorldPos(tile);
        _targetWorldPos = GridManager.I.GetWorldPos(tile + direction);

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

            // Handle different attack types
            switch (pair.attack.AttackData.attackType)
            {
                case AttackType.SingleTarget:
                    // Already handled by the line above
                    break;
                case AttackType.VerticalSplash:
                    Debug.Log($"Applying vertical splash from tile {tile}");
                    foreach (var areaEffect in pair.attack.AttackData.areaEffects)
                    {
                        Vector2Int affectedTile = tile + areaEffect.offset;
                        Debug.Log($"Checking affected tile: {affectedTile}");
                        var splashPairs = GridManager.I.GetPairsToResolveForTile(affectedTile);
                        foreach (var splashPair in splashPairs)
                        {
                            if (splashPair.target.Faction != pair.attack.Faction)
                            {
                                Debug.Log($"Applying splash damage to {splashPair.target.GetType().Name} on tile {affectedTile}");
                                splashPair.target.ApplyDamage(areaEffect.BuildDamageEvent());
                            }
                        }
                    }
                    break;
                // Add more cases for other attack types as needed
            }
            
            Destroy(gameObject); // Destroy projectile after all damage (primary and area) is applied
            return; // Exit after first hit and its area effects
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
