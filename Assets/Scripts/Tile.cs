// Tile.cs
using UnityEngine;

/// <summary>
/// Holds grid coordinates and a cached SpriteRenderer so we can
/// colour, highlight, or reset the tile very cheaply.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    public  Vector2Int GridPos   { get; private set; }

    private SpriteRenderer sr;
    private Color baseColour;

    /// <summary>Called once by GridManager right after instantiation.</summary>
    public void Init(Vector2Int gridPos, Color startColour)
    {
        GridPos    = gridPos;
        baseColour = startColour;

        if (sr == null) sr = GetComponent<SpriteRenderer>();
        sr.color = baseColour;
    }

    /* ---------- optional helpers for telegraphs / cursor ---------- */

    public void Highlight(Color c) => sr.color = c;
    public void Reset()            => sr.color = baseColour;
}
