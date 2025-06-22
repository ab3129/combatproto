using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int startingGridPosition = new Vector2Int(5, 1);
    [SerializeField] private GridManager gridManager;

    [Header("Attack Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector2 shootDirection = Vector2.left;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float attackCooldown = 2f;

    private Vector2Int gridPosition;
    private float attackTimer = 0f;
    private float moveTimer = 0f;
    private float moveInterval = 2f;

    private void Start()
    {
        gridPosition = startingGridPosition;
        transform.position = gridManager.GetWorldPosition(gridPosition);
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;
        moveTimer -= Time.deltaTime;

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

    private void Attack()
    {
        Vector3 spawnPos = transform.position;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        proj.GetComponent<Projectile>().Initialize(shootDirection, targetTag, false);
    }

    private void RandomMove()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        Vector2Int newPos = gridPosition;
        int attempts = 0;

        while (attempts < directions.Length)
        {
            Vector2Int tryMove = directions[Random.Range(0, directions.Length)];
            newPos = gridPosition + tryMove;

            // Prevent enemy from moving onto player-side tiles
            bool isOnEnemySide = newPos.x >= gridManager.columns / 2;

            if (gridManager.IsInsideGrid(newPos) && isOnEnemySide)
            {
                gridPosition = newPos;
                transform.position = gridManager.GetWorldPosition(gridPosition);
                break;
            }

            attempts++;
        }
    }
}
