using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 direction;
    private string targetTag;
    private bool isPlayerProjectile;
    private GridManager gridManager;

    [SerializeField] private int damage = 1; // Adjustable damage amount

    public void Initialize(Vector2 dir, string target, bool fromPlayer)
    {
        direction = dir.normalized;
        targetTag = target;
        isPlayerProjectile = fromPlayer;
        gridManager = FindFirstObjectByType<GridManager>();
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (gridManager != null)
        {
            Vector3 pos = transform.position;
            Vector2Int gridPos = new Vector2Int(
                Mathf.RoundToInt(pos.x / (gridManager.tileSize + gridManager.tileSpacing)),
                Mathf.RoundToInt(pos.y / (gridManager.tileSize + gridManager.tileSpacing))
            );

            if (!gridManager.IsInsideGrid(gridPos))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            HealthSystem health = other.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject); // Remove projectile on hit
        }
    }
}
