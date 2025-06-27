using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int startingGridPosition = new(5, 1);
    [SerializeField] private GridManager gridManager;

    [Header("Attack Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector2 shootDirection = Vector2.left;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float attackCooldown = 2f;

    /* ------------------------------------------------------------ */
    private Vector2Int gridPos;
    private float attackTimer;
    private float moveTimer;
    private const float moveInterval = 2f;

    /* =============================  Mono ============================= */

    void Start()
    {
        gridPos = startingGridPosition;
        transform.position = gridManager.GetWorldPosition(gridPos);
        gridManager.Register(gameObject, gridPos);          // ← NEW
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        moveTimer   -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackCooldown;
        }

        if (moveTimer <= 0f)
        {
            RandomMove();
            moveTimer = moveInterval;
        }
    }

    void OnDestroy()
    {
        gridManager.Unregister(gameObject, gridPos);        // ← NEW
    }

    /* ============================  Attack  ============================ */

    void Attack()
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Initialize(shootDirection, targetTag);
    }

    /* ============================  Movement  =========================== */

    void RandomMove()
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        // try up to 4 random directions before giving up
        for (int tries = 0; tries < dirs.Length; ++tries)
        {
            Vector2Int candidate = gridPos + dirs[Random.Range(0, dirs.Length)];
            bool onEnemySide = candidate.x >= gridManager.columns / 2;

            if (onEnemySide && gridManager.IsInsideGrid(candidate))
            {
                gridManager.Unregister(gameObject, gridPos);    // ← NEW
                gridPos = candidate;
                transform.position = gridManager.GetWorldPosition(gridPos);
                gridManager.Register(gameObject, gridPos);      // ← NEW
                return;
            }
        }
    }
}
