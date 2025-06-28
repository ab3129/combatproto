using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class EnemyController : MonoBehaviour
{
    [Header("Combat")]
    public AttackDescriptor shot;
    public GameObject       projectilePrefab;
    public float            attackCooldown = 2f;

    [Header("Grid placement")]
    public Vector2Int startingTile = new(5, 1);

    Vector2Int   tile;
    float        timer;
    HealthSystem health;

    void Start()
    {
        health = GetComponent<HealthSystem>();

        tile              = startingTile;
        transform.position = GridManager.I.GetWorldPos(tile);
        health.MoveToTile(tile);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Fire();
            timer = attackCooldown;
        }
    }

    void Fire()
    {
        var proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity)
                   .GetComponent<Projectile>();

        proj.Init(shot, Faction.Enemy, Vector2Int.left);
    }
}
