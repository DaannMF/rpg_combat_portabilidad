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
    [SerializeField] private Color validMoveColor = Color.green;
    [SerializeField] private Color invalidMoveColor = Color.red;
    [SerializeField] private Color currentPlayerColor = Color.blue;

    [Header("Mobile Input")]
    [SerializeField] private BoxCollider2D clickCollider;

    private Vector2 worldPosition;
    private bool isInteractable = false;
    private Color originalColor;

    // Events for mobile input
    public static event Action<GridCell> OnCellClicked;

    private void Awake() {
        if (cellSpriteRenderer == null) {
            cellSpriteRenderer = GetComponent<SpriteRenderer>();
        }

#if UNITY_ANDROID || UNITY_IOS
        if (clickCollider == null) {
            clickCollider = GetComponent<BoxCollider2D>();
            if (clickCollider == null) {
                clickCollider = gameObject.AddComponent<BoxCollider2D>();
            }
        }
#endif

        originalColor = defaultColor;
    }

    private void OnMouseDown() {
        if (isInteractable) OnCellClicked?.Invoke(this);
    }

    public void Initialize(int gridX, int gridY, Vector2 worldPos) {
        x = gridX;
        y = gridY;
        worldPosition = worldPos;
        transform.position = worldPosition;

        if (cellSpriteRenderer != null && defaultSprite != null) {
            cellSpriteRenderer.sprite = defaultSprite;
            cellSpriteRenderer.color = defaultColor;
            originalColor = defaultColor;
        }
    }

    public void SetCellSize(float cellWidth, float cellHeight) {
        if (cellSpriteRenderer != null)
            cellSpriteRenderer.size = new Vector2(cellWidth, cellHeight);

        if (clickCollider != null)
            clickCollider.size = new Vector2(0.5f, 0.5f);
    }

    public void SetInteractable(bool interactable) {
        isInteractable = interactable;
    }

    public void SetAsValidMove() {
        if (cellSpriteRenderer != null) {
            cellSpriteRenderer.color = validMoveColor;
        }
        SetInteractable(true);
    }

    public void SetAsInvalidMove() {
        if (cellSpriteRenderer != null) {
            cellSpriteRenderer.color = invalidMoveColor;
        }
        SetInteractable(false);
    }

    public void SetAsCurrentPlayer() {
        if (cellSpriteRenderer != null) {
            cellSpriteRenderer.color = currentPlayerColor;
        }
        SetInteractable(false);
    }

    public void ResetToDefault() {
        if (cellSpriteRenderer != null) {
            cellSpriteRenderer.color = originalColor;
        }
        SetInteractable(false);
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