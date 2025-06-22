using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}
