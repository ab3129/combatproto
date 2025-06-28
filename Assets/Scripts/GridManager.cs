using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Layout")] public int   columns     = 6;
    [Header("Layout")] public int   rows        = 3;
    [Header("Layout")] public float tileSize    = 1f;
    [Header("Layout")] public float tileSpacing = 0.1f;

    [Header("Visuals")] [SerializeField] private GameObject tilePrefab;
    [Header("Visuals")] [SerializeField] private Color playerSide = new(0.20f, 0.40f, 1f);
    [Header("Visuals")] [SerializeField] private Color enemySide  = new(1f, 0.30f, 0.30f);

    // Singleton
    public static GridManager I { get; private set; }

    // Logical grid
    private PanelState[] panels;
    private readonly HashSet<IGridObject> tmpSet = new();

    // Visual grid
    private Tile[,] tiles;

    void Awake()
    {
        if (I != null && I != this) { Destroy(this); return; }
        I = this;

        // 1 ▸ allocate logical panel data
        panels = new PanelState[columns * rows];
        for (int i = 0; i < panels.Length; ++i)
        {
            panels[i].objects = new HashSet<IGridObject>();
            panels[i].attacks = new HashSet<IAttackSource>();
        }

        // 2 ▸ instantiate visible grid
        BuildVisualGrid();
        FitCamera();
    }

    // -----------------------------------------------------------------
    // Visual Grid
    // -----------------------------------------------------------------
    void BuildVisualGrid()
    {
        if (tilePrefab == null)
        {
            Debug.LogWarning("GridManager → tilePrefab not assigned – grid will be invisible.");
            return;
        }

        tiles = new Tile[columns, rows];
        float step = tileSize + tileSpacing;

        for (int x = 0; x < columns; ++x)
        for (int y = 0; y < rows;    ++y)
        {
            Vector2Int gp = new(x, y);
            Vector3    wp = new(x * step, y * step, 0f);

            GameObject go = Instantiate(tilePrefab, wp, Quaternion.identity, transform);
            go.name       = $"Tile_{x}_{y}";

            if (!go.TryGetComponent(out Tile t))
            {
                Debug.LogError("Tile prefab missing ‘Tile’ script.");
                Destroy(go);
                continue;
            }

            Color baseCol = (x < columns / 2) ? playerSide : enemySide;
            t.Init(gp, baseCol);
            tiles[x, y] = t;
        }
    }
    
    // GridManager.cs (add inside the class)
    void FitCamera()
    {
        var cam = Camera.main;
        if (cam == null || !cam.orthographic) return;

        float step = tileSize + tileSpacing;
        Vector2 gridSize = new(columns * step, rows * step);

        // Position the camera so it looks at the middle of the grid
        Vector3 centre = new Vector3(
            (gridSize.x - step) * 0.5f,   // centre X
            (gridSize.y - step) * 0.5f,   // centre Y
            cam.transform.position.z);    // keep current Z
        cam.transform.position = centre;

        // Choose an ortho size that fits the larger of width/height
        float halfGridH = gridSize.y * 0.5f;
        float halfGridW = gridSize.x * 0.5f / cam.aspect;
        cam.orthographicSize = Mathf.Max(halfGridH, halfGridW) + 0.5f; // +margin
    }


    public bool IsInsideGrid(Vector2Int t)
    {
        bool inside = t.x >= 0 && t.x < columns && t.y >= 0 && t.y < rows;
        if (!inside)
        {
            Debug.LogWarning($"IsInsideGrid: Tile {t} is outside grid (columns: {columns}, rows: {rows})");
        }
        return inside;
    }
    public Vector3  GetWorldPos(Vector2Int t)
    {
        float step = tileSize + tileSpacing;
        return new Vector3(t.x * step, t.y * step, 0f);
    }
    public Vector2Int WorldToTile(Vector3 pos)
    {
        float step = tileSize + tileSpacing;
        return new Vector2Int(Mathf.RoundToInt(pos.x / step), Mathf.RoundToInt(pos.y / step));
    }

    // -----------------------------------------------------------------
    // Occupancy helpers
    // -----------------------------------------------------------------
    public void UpdateOccupancy(IGridObject obj, IList<Vector2Int> oldTiles, IList<Vector2Int> newTiles)
    {
        foreach (var t in oldTiles)
        {
            if (IsInsideGrid(t))
            {
                panels[Index(t)].objects.Remove(obj);
                Debug.Log($"GridManager: Removed object {obj.GetType().Name} ({obj.Faction}) from tile {t}. Objects on tile: {panels[Index(t)].objects.Count}");
            }
        }
        foreach (var t in newTiles)
        {
            if (IsInsideGrid(t))
            {
                panels[Index(t)].objects.Add(obj);
                Debug.Log($"GridManager: Added object {obj.GetType().Name} ({obj.Faction}) to tile {t}. Objects on tile: {panels[Index(t)].objects.Count}");
            }
        }
    }
    public void AddAttack(IAttackSource atk, Vector2Int tile)
    {
        if (IsInsideGrid(tile))
        {
            panels[Index(tile)].attacks.Add(atk);
            Debug.Log($"GridManager: Added attack {atk.GetType().Name} ({atk.Faction}) to tile {tile}. Attacks on tile: {panels[Index(tile)].attacks.Count}");
        }
    }
    public void RemoveAttack(IAttackSource atk, Vector2Int tile)
    {
        if (IsInsideGrid(tile))
        {
            panels[Index(tile)].attacks.Remove(atk);
            Debug.Log($"GridManager: Removed attack {atk.GetType().Name} ({atk.Faction}) from tile {tile}. Attacks on tile: {panels[Index(tile)].attacks.Count}");
        }
    }

    // -----------------------------------------------------------------
    // Combat resolution query
    // -----------------------------------------------------------------
    public IEnumerable<CombatPair> GetPairsToResolve()
    {
        for (int i = 0; i < panels.Length; ++i)
        {
            // This method is now deprecated. Use GetPairsToResolveForTile in a loop if needed.
            // For now, it will still function but the primary combat resolution will be per-tile.
            var p = panels[i];
            if (p.attacks.Count == 0 || p.objects.Count == 0) continue;

            foreach (var atk in p.attacks)
            {
                tmpSet.Clear();
                foreach (var o in p.objects) tmpSet.Add(o); // snapshot

                foreach (var obj in tmpSet)
                    if (obj is IDamageable d && obj.Faction != atk.Faction)
                        yield return new CombatPair(atk, d);
            }
        }
    }

    public IEnumerable<CombatPair> GetPairsToResolveForTile(Vector2Int tile)
    {
        if (!IsInsideGrid(tile)) yield break;

        var p = panels[Index(tile)];
        if (p.attacks.Count == 0 || p.objects.Count == 0) yield break;

        foreach (var atk in p.attacks)
        {
            tmpSet.Clear();
            foreach (var o in p.objects) tmpSet.Add(o); // snapshot

            foreach (var obj in tmpSet)
                if (obj is IDamageable d && obj.Faction != atk.Faction)
                    yield return new CombatPair(atk, d);
        }
    }

    // -----------------------------------------------------------------
    // Internals
    // -----------------------------------------------------------------
    private int Index(Vector2Int t) => t.x + t.y * columns;

    private struct PanelState
    {
        public HashSet<IGridObject>  objects;
        public HashSet<IAttackSource> attacks;
    }
}

public readonly struct CombatPair
{
    public readonly IAttackSource attack;
    public readonly IDamageable   target;

    public CombatPair(IAttackSource atk, IDamageable tgt)
    {
        attack = atk;
        target = tgt;
    }
}
