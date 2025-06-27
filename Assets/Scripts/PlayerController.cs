using UnityEngine;

[RequireComponent(typeof(PlayerInputWrapper))]
public class PlayerController : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int startingGridPosition = new(0, 1);
    [SerializeField] private GridManager gridManager;

    [Header("Attack Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector2 shootDirection = Vector2.right;
    [SerializeField] private string targetTag = "Enemy";

    /* ------------------------------------------------------------ */
    private Vector2Int gridPos;
    private PlayerInputWrapper input;
    private float  moveCooldown = 0.20f;
    private float  lastMoveTime;

    /* =============================  Mono ============================= */

    void Start()
    {
        input   = GetComponent<PlayerInputWrapper>();
        gridPos = startingGridPosition;

        transform.position = gridManager.GetWorldPosition(gridPos);
        gridManager.Register(gameObject, gridPos);          // ← NEW
    }

    void Update()
    {
        HandleMovement();

        if (input.ShootPressed)
            Shoot();
    }

    void OnDestroy()
    {
        // make sure we disappear from the grid even if destroyed by scene reload etc.
        gridManager.Unregister(gameObject, gridPos);        // ← NEW
    }

    /* ==========================  Movement  ========================== */

    void HandleMovement()
    {
        if (Time.time - lastMoveTime < moveCooldown) return;

        Vector2  raw = input.MovementInput;
        Vector2Int delta = Vector2Int.zero;

        const float thresh = 0.5f;
        if (raw.y  >  thresh) delta.y =  1;
        if (raw.y  < -thresh) delta.y = -1;
        if (raw.x  >  thresh) delta.x =  1;
        if (raw.x  < -thresh) delta.x = -1;

        if (delta == Vector2Int.zero) return;

        Vector2Int next = gridPos + delta;

        bool onPlayerSide = next.x < gridManager.columns / 2;
        if (gridManager.IsInsideGrid(next) && onPlayerSide)
        {
            gridManager.Unregister(gameObject, gridPos);    // ← NEW
            gridPos = next;
            transform.position = gridManager.GetWorldPosition(gridPos);
            gridManager.Register(gameObject, gridPos);      // ← NEW 
            lastMoveTime = Time.time;
        }
    }

    /* ============================  Attack  ============================ */

    void Shoot()
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Initialize(shootDirection, targetTag);
    }
}
