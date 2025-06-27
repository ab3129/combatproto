using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds the panel grid and keeps a fast lookup table that says
/// “which GameObjects are standing on panel (x,y) right now?”.
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("Grid layout")]
    public int columns = 6;
    public int rows    = 3;
    public float tileSize    = 1f;
    public float tileSpacing = 0.1f;       // gap between tiles
    public GameObject tilePrefab;

    [Header("Visuals")]
    public Color playerSideColor = Color.blue;
    public Color enemySideColor  = Color.red;

    /* ------------------------------------------------------------ */
    public static GridManager I { get; private set; }

    /// <summary>grid[x,y] → Tile component for quick colour changes, etc.</summary>
    private Tile[,] grid;

    /// <summary>Which things occupy which panel right now.</summary>
    private readonly Dictionary<Vector2Int, List<GameObject>> occupancy = new();

    void Awake() => I = this;

    void Start()
    {
        GenerateGrid();
        CenterCamera();
    }

    /* =====================  Public lookup API  ===================== */

    /// <summary>Add obj to the given panel (call when it spawns or moves).</summary>
    public void Register(GameObject obj, Vector2Int tile)
    {
        if (!occupancy.TryGetValue(tile, out var list))
            list = occupancy[tile] = new List<GameObject>();

        if (!list.Contains(obj))
            list.Add(obj);
    }

    /// <summary>Remove obj from the given panel (call before it moves or dies).</summary>
    public void Unregister(GameObject obj, Vector2Int tile)
    {
        if (occupancy.TryGetValue(tile, out var list))
            list.Remove(obj);
    }

    /// <summary>All objects currently standing on this panel.</summary>
    public IEnumerable<GameObject> GetOccupants(Vector2Int tile) =>
        occupancy.TryGetValue(tile, out var list) ? list : System.Array.Empty<GameObject>();

    public bool IsInsideGrid(Vector2Int pos) =>
        pos.x >= 0 && pos.x < columns && pos.y >= 0 && pos.y < rows;

    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        float x = gridPos.x * (tileSize + tileSpacing);
        float y = gridPos.y * (tileSize + tileSpacing);
        return new Vector3(x, y, 0f);
    }

    /* =====================  Private helpers  ===================== */

    void GenerateGrid()
    {
        grid = new Tile[columns, rows];

        for (int x = 0; x < columns; ++x)
        for (int y = 0; y < rows;   ++y)
        {
            Vector3 pos = GetWorldPosition(new Vector2Int(x, y));
            GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);

            Tile tile = tileObj.GetComponent<Tile>();
            grid[x, y] = tile;

            tile.SetColor(x < columns / 2 ? playerSideColor : enemySideColor);
            tile.gridPosition = new Vector2Int(x, y);
        }
    }

    void CenterCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float width  = (columns - 1) * (tileSize + tileSpacing);
        float height = (rows    - 1) * (tileSize + tileSpacing);

        cam.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        cam.orthographicSize   = Mathf.Max(width, height) / 2.5f;   // tune to taste
    }
}
