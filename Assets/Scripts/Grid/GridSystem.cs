using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridSystem : MonoBehaviour {
    [Header("Grid Settings")]
    [SerializeField] private int width = 4;
    [SerializeField] private int height = 6;
    [SerializeField] private Transform cellPrefab;

    [Header("Container (Optional)")]
    [SerializeField] private RectTransform container;
    [SerializeField] private float manualCellSize = 1f;
    [SerializeField] private float containerToWorldScale = 0.01f;
    [SerializeField] private float cellSpacing = 0f;

    [Header("Character Positioning")]
    [SerializeField] private Vector2 characterSpriteOffset = new Vector2(0, 0.2f);

    private GridCell[,] grid;
    private Dictionary<GridCell, Character> characterPositions;
    private Vector2 gridOrigin;
    private float calculatedCellWidth;
    private float calculatedCellHeight;

    private void Awake() {
        InitializeGrid();
    }

    private void InitializeGrid() {
        CalculateCellSizeAndOrigin();

        grid = new GridCell[width, height];
        characterPositions = new Dictionary<GridCell, Character>();

        CreateGridCells();
    }

    private void CalculateCellSizeAndOrigin() {
        if (container != null)
            CalculateFromContainer();
        else {
            calculatedCellWidth = manualCellSize;
            calculatedCellHeight = manualCellSize;
            CalculateOriginFromCamera();
        }
    }

    private void CalculateFromContainer() {
        Canvas canvas = container.GetComponentInParent<Canvas>();
        if (canvas == null) {
            calculatedCellWidth = manualCellSize;
            calculatedCellHeight = manualCellSize;
            CalculateOriginFromCamera();
            return;
        }

        // Get container size in UI units
        Rect containerRect = container.rect;
        float containerUIWidth = containerRect.width;
        float containerUIHeight = containerRect.height;

        // Convert UI size to world size using scale factor
        float containerWorldWidth = containerUIWidth * containerToWorldScale;
        float containerWorldHeight = containerUIHeight * containerToWorldScale;

        // Calculate available space for cells (subtracting spacing)
        float spacingWidthTotal = (width - 1) * cellSpacing;
        float spacingHeightTotal = (height - 1) * cellSpacing;

        float availableWidth = containerWorldWidth - spacingWidthTotal;
        float availableHeight = containerWorldHeight - spacingHeightTotal;

        // Calculate cell size
        float cellWidth = availableWidth / width;
        float cellHeight = availableHeight / height;
        calculatedCellWidth = cellWidth;
        calculatedCellHeight = cellHeight;

        // Ensure minimum reasonable cell size
        calculatedCellWidth = Mathf.Max(calculatedCellWidth, 0.1f);
        calculatedCellHeight = Mathf.Max(calculatedCellHeight, 0.1f);

        // Get world position of container's bottom-left corner
        Vector3[] containerWorldCorners = new Vector3[4];
        container.GetWorldCorners(containerWorldCorners);
        gridOrigin = containerWorldCorners[0] + new Vector3(calculatedCellWidth * 0.5f, calculatedCellHeight * 0.5f, 0);

    }

    private void CalculateOriginFromCamera() {
        Camera cam = Camera.main;
        if (cam == null) {
            gridOrigin = Vector2.zero;
            return;
        }

        float totalWidth = width * calculatedCellWidth;
        float totalHeight = height * calculatedCellHeight;

        Vector2 bottomLeft = cam.ScreenToWorldPoint(Vector2.zero);
        Vector2 topRight = cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, cam.pixelHeight));

        Vector2 centerWorld = (bottomLeft + topRight) * 0.5f;
        gridOrigin = centerWorld - new Vector2(totalWidth * 0.5f, totalHeight * 0.5f);
    }

    private void CreateGridCells() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Vector2 worldPosition = GetWorldPosition(x, y);

                GameObject cellObject = Instantiate(cellPrefab, worldPosition, Quaternion.identity, transform).gameObject;
                GridCell cell = cellObject.GetComponent<GridCell>();

                if (cell == null)
                    cell = cellObject.AddComponent<GridCell>();

                cell.Initialize(x, y, worldPosition);
                cell.SetCellSize(calculatedCellWidth, calculatedCellHeight);
                grid[x, y] = cell;
            }
        }
    }

    public Vector2 GetWorldPosition(int x, int y) {
        float cellWidthWithSpacing = calculatedCellWidth + cellSpacing;
        float cellHeightWithSpacing = calculatedCellHeight + cellSpacing;
        return gridOrigin + new Vector2(x * cellWidthWithSpacing, y * cellHeightWithSpacing);
    }

    public Vector2 GetWorldPosition(GridCell cell) {
        return GetWorldPosition(cell.x, cell.y);
    }

    public Vector2 GetCharacterWorldPosition(int x, int y) {
        float cellWidthWithSpacing = calculatedCellWidth + cellSpacing;
        float cellHeightWithSpacing = calculatedCellHeight + cellSpacing;
        Vector2 cellCenter = gridOrigin + new Vector2(x * cellWidthWithSpacing, y * cellHeightWithSpacing);
        return cellCenter + characterSpriteOffset;
    }

    public Vector2 GetCharacterWorldPosition(GridCell cell) {
        return GetCharacterWorldPosition(cell.x, cell.y);
    }

    public bool IsValidPosition(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public bool IsValidPosition(GridCell cell) {
        return IsValidPosition(cell.x, cell.y);
    }

    public bool IsPositionOccupied(GridCell cell) {
        return characterPositions.ContainsKey(cell);
    }

    public bool CanMoveToPosition(GridCell cell) {
        return IsValidPosition(cell) && !IsPositionOccupied(cell);
    }

    public void SetCharacterPosition(Character character, GridCell cell) {
        if (characterPositions.ContainsValue(character)) {
            GridCell oldCell = characterPositions.FirstOrDefault(x => x.Value == character).Key;
            characterPositions.Remove(oldCell);
        }

        characterPositions[cell] = character;
    }

    public void RemoveCharacterFromGrid(Character character) {
        if (characterPositions.ContainsValue(character)) {
            GridCell oldCell = characterPositions.FirstOrDefault(x => x.Value == character).Key;
            characterPositions.Remove(oldCell);
        }
    }

    public List<GridCell> GetValidMovePositions(GridCell fromCell, int maxDistance) {
        List<GridCell> validPositions = new List<GridCell>();

        for (int x = fromCell.x - maxDistance; x <= fromCell.x + maxDistance; x++) {
            for (int y = fromCell.y - maxDistance; y <= fromCell.y + maxDistance; y++) {
                if (!IsValidPosition(x, y)) continue;

                GridCell targetCell = GetGridCell(x, y);
                if (targetCell == null || targetCell.Equals(fromCell)) continue;

                int distance = fromCell.GetChebyshevDistance(targetCell);
                if (distance <= maxDistance && !IsPositionOccupied(targetCell)) {
                    validPositions.Add(targetCell);
                }
            }
        }

        return validPositions;
    }

    public List<GridCell> GetAvailablePositions() {
        List<GridCell> availablePositions = new List<GridCell>();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                GridCell cell = GetGridCell(x, y);
                if (cell != null && !IsPositionOccupied(cell)) {
                    availablePositions.Add(cell);
                }
            }
        }

        return availablePositions;
    }

    public GridCell GetGridCell(int x, int y) {
        if (!IsValidPosition(x, y)) return null;
        return grid[x, y];
    }
}