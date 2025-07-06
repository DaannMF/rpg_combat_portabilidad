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

    public Vector2 GetWorldPosition() {
        return worldPosition;
    }

    public bool Equals(GridCell other) {
        return x == other.x && y == other.y;
    }

    public int GetManhattanDistance(GridCell other) {
        return Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y);
    }

    public override string ToString() {
        return $"GridCell({x}, {y})";
    }
}