using System;
using UnityEngine;

[Serializable]
public class GridCell : MonoBehaviour {
    [Header("Position")]
    public int x;
    public int y;

    [Header("Visual Settings")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private SpriteRenderer cellSpriteRenderer;

    [Header("Colors")]
    [SerializeField] private Color defaultColor = Color.white;

    private Vector2 worldPosition;

    private void Awake() {
        if (cellSpriteRenderer == null) {
            cellSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void Initialize(int gridX, int gridY, Vector2 worldPos) {
        x = gridX;
        y = gridY;
        worldPosition = worldPos;
        transform.position = worldPosition;

        if (cellSpriteRenderer != null && defaultSprite != null) {
            cellSpriteRenderer.sprite = defaultSprite;
            cellSpriteRenderer.color = defaultColor;
        }
    }

    public void SetCellSize(float cellSize) {
        SetCellSize(cellSize, cellSize);
    }

    public void SetCellSize(float cellWidth, float cellHeight) {
        if (cellSpriteRenderer != null)
            cellSpriteRenderer.size = new Vector2(cellWidth, cellHeight);
    }

    public bool Equals(GridCell other) {
        return x == other.x && y == other.y;
    }

    public int GetManhattanDistance(GridCell other) {
        return Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y);
    }

    public int GetChebyshevDistance(GridCell other) {
        return Mathf.Max(Mathf.Abs(x - other.x), Mathf.Abs(y - other.y));
    }
}