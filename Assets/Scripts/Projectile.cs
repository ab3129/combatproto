using UnityEngine;

/// <summary>
/// Moves in a straight line; the moment it enters a new panel it checks
/// whether a target with the right tag is standing there.  
/// No physics engine required.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]   // purely visual – no collider!
public class Projectile : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 5f;                // world-units per second
    private Vector2 direction;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    private string targetTag;

    /* ------------------------------------------------------------ */
    private GridManager gm;
    private Vector2Int currentTile;          // last panel we were in

    /// <param name="dir">World-space direction (need not be axis-aligned).</param>
    /// <param name="target">Tag we’re allowed to damage (e.g. "Enemy").</param>
    public void Initialize(Vector2 dir, string target)
    {
        direction  = dir.normalized;
        targetTag  = target;
        gm         = GridManager.I;

        currentTile = WorldToTile(transform.position);
    }

    void Update()
    {
        // 1 · Move
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // 2 · Figure out which panel we’re in now
        Vector2Int tile = WorldToTile(transform.position);

        // 3 · If we stepped into a new panel, run the hit test
        if (tile != currentTile)
        {
            currentTile = tile;
            CheckForHit(tile);
        }

        // 4 · Out-of-bounds? destroy self
        if (!gm.IsInsideGrid(tile))
            Destroy(gameObject);
    }

    /* =====================  Helpers  ===================== */

    Vector2Int WorldToTile(Vector3 worldPos)
    {
        float step = gm.tileSize + gm.tileSpacing;
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / step),
            Mathf.RoundToInt(worldPos.y / step)
        );
    }

    void CheckForHit(Vector2Int tile)
    {
        if (!gm.IsInsideGrid(tile)) return;

        foreach (GameObject obj in gm.GetOccupants(tile))
        {
            if (!obj.CompareTag(targetTag)) continue;

            if (obj.TryGetComponent(out HealthSystem hp))
                hp.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }
    }
}
