using UnityEngine;

[RequireComponent(typeof(PlayerInputWrapper))]
[RequireComponent(typeof(HealthSystem))]
public class PlayerController : MonoBehaviour
{
    [Header("Combat")]
    public AttackDescriptor basicShot;
    public GameObject       projectilePrefab;

    [Header("Grid movement")]
    public Vector2Int startingTile = new(0, 1);
    public float      moveCooldown = 0.20f;

    Vector2Int        tile;
    float             lastMoveTime;
    PlayerInputWrapper input;
    HealthSystem       health;

    void Start()
    {
        input  = GetComponent<PlayerInputWrapper>();
        health = GetComponent<HealthSystem>();

        tile              = startingTile;
        transform.position = GridManager.I.GetWorldPos(tile);
        health.MoveToTile(tile);
    }

    void Update()
    {
        HandleMovement();

        if (input.ShootPressed)
        {
            Shoot();     // small helper to reset the flag (see below)
        }
    }

    void HandleMovement()
    {
        if (Time.time - lastMoveTime < moveCooldown) return;

        Vector2 raw = input.MovementInput;
        const float thresh = 0.5f;

        Vector2Int delta = Vector2Int.zero;
        if (raw.y >  thresh) delta.y =  1;
        if (raw.y < -thresh) delta.y = -1;
        if (raw.x >  thresh) delta.x =  1;
        if (raw.x < -thresh) delta.x = -1;
        if (delta == Vector2Int.zero) return;

        Vector2Int next = tile + delta;
        bool onPlayerSide = next.x < GridManager.I.columns / 2;

        if (onPlayerSide && GridManager.I.IsInsideGrid(next))
        {
            tile = next;
            transform.position = GridManager.I.GetWorldPos(tile);
            health.MoveToTile(tile);
            lastMoveTime = Time.time;
        }
    }

    void Shoot()
    {
        Debug.Log($"Player shooting from position {transform.position}, tile {tile}");
        var proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity)
                   .GetComponent<Projectile>();

        proj.Init(basicShot, Faction.Player, Vector2Int.right);
        
    }
}
