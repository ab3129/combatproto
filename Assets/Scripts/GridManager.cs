using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int columns = 6;
    public int rows = 3;

    public float tileSize = 1f;
    public float tileSpacing = 0.1f; // Spacing between tiles

    public GameObject tilePrefab;

    private Tile[,] grid;

    public Color playerSideColor = Color.blue;
    public Color enemySideColor = Color.red;

    void Start()
    {
        GenerateGrid();
        CenterCamera();
    }

    void GenerateGrid()
    {
        grid = new Tile[columns, rows];

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                float xOffset = x * (tileSize + tileSpacing);
                float yOffset = y * (tileSize + tileSpacing);
                Vector3 spawnPos = new Vector3(xOffset, yOffset, 0);

                GameObject tileObj = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                Tile tile = tileObj.GetComponent<Tile>();
                grid[x, y] = tile;

                if (x < columns / 2)
                    tile.SetColor(playerSideColor);
                else
                    tile.SetColor(enemySideColor);

                tile.gridPosition = new Vector2Int(x, y);
            }
        }
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        float xOffset = gridPosition.x * (tileSize + tileSpacing);
        float yOffset = gridPosition.y * (tileSize + tileSpacing);
        return new Vector3(xOffset, yOffset, 0);
    }

    public bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < columns && pos.y >= 0 && pos.y < rows;
    }

    void CenterCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float width = (columns - 1) * (tileSize + tileSpacing);
            float height = (rows - 1) * (tileSize + tileSpacing);
            Vector3 centerPosition = new Vector3(width / 2f, height / 2f, -10f);
            mainCamera.transform.position = centerPosition;
            mainCamera.orthographicSize = Mathf.Max(width, height) / 2.5f; // Adjust zoom level
        }
    }
}
