using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int startingGridPosition = new Vector2Int(0, 1);
    [SerializeField] private GridManager gridManager;

    [Header("Attack Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector2 shootDirection = Vector2.right;
    [SerializeField] private string targetTag = "Enemy";

    private Vector2Int gridPosition;
    private PlayerInputWrapper inputWrapper;

    private float moveCooldown = 0.2f;  // Time in seconds between movements
    private float lastMoveTime = 0f;

    void Start()
    {
        gridPosition = startingGridPosition;
        transform.position = gridManager.GetWorldPosition(gridPosition);

        inputWrapper = GetComponent<PlayerInputWrapper>();
    }


    
void Update()
{
    HandleMovement();

    if (inputWrapper.ShootPressed)
    {
        Shoot();
    }
}




void HandleMovement()
{
    Vector2 input = inputWrapper.MovementInput;

    // Create movement direction vector
    Vector2Int moveDelta = Vector2Int.zero;
    const float moveThreshold = 0.5f;

    // Only process movement if enough time has passed since the last movement
    if (Time.time - lastMoveTime >= moveCooldown)
    {
        // Check vertical movement
        if (input.y > moveThreshold)
            moveDelta.y = 1; // Move up by 1 tile
        else if (input.y < -moveThreshold)
            moveDelta.y = -1; // Move down by 1 tile

        // Check horizontal movement
        if (input.x < -moveThreshold)
            moveDelta.x = -1; // Move left by 1 tile
        else if (input.x > moveThreshold)
            moveDelta.x = 1; // Move right by 1 tile

        // If there is valid movement, update the position
        if (moveDelta != Vector2Int.zero)
        {
            Vector2Int newPosition = gridPosition + moveDelta;

            if (gridManager.IsInsideGrid(newPosition) && newPosition.x < gridManager.columns / 2)
            {
                gridPosition = newPosition;
                transform.position = gridManager.GetWorldPosition(gridPosition);
            }

            lastMoveTime = Time.time; // Update the time of the last movement
        }
    }
}


    void Shoot()
    {
        Vector3 spawnPos = transform.position;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        proj.GetComponent<Projectile>().Initialize(shootDirection, targetTag, true);
    }
}
