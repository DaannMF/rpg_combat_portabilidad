using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridSystem : MonoBehaviour {
    [Header("Grid Settings")]
    [SerializeField] private int width = 4;
    [SerializeField] private int height = 6;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Transform cellPrefab;

    private GridCell[,] grid;
    private Dictionary<GridCell, Character> characterPositions;
    private Vector2 gridOrigin;

    private void Awake() {
        InitializeGrid();
    }

    private void InitializeGrid() {
        grid = new GridCell[width, height];
        characterPositions = new Dictionary<GridCell, Character>();

        CalculateGridOrigin();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Vector2 worldPosition = GetWorldPosition(x, y);

                GameObject cellObject = Instantiate(cellPrefab, worldPosition, Quaternion.identity, transform).gameObject;
                GridCell cell = cellObject.GetComponent<GridCell>();

                if (cell == null)
                    cell = cellObject.AddComponent<GridCell>();

                cell.Initialize(x, y, worldPosition);
                grid[x, y] = cell;
            }
        }
    }

    private void CalculateGridOrigin() {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) {
            Debug.LogWarning("No main camera found. Using default origin (0, 0).");
            gridOrigin = Vector3.zero;
            return;
        }

        Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        float halfCellSize = cellSize * 0.5f + 0.2f;
        gridOrigin = new Vector3(bottomLeft.x + halfCellSize, bottomLeft.y + halfCellSize, 0);
    }

    public Vector2 GetWorldPosition(int x, int y) {
        return gridOrigin + new Vector2(x * cellSize, y * cellSize);
    }

    public Vector2 GetWorldPosition(GridCell cell) {
        return GetWorldPosition(cell.x, cell.y);
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

    public List<GridCell> GetValidMovePositions(GridCell fromCell, int maxDistance) {
        List<GridCell> validPositions = new List<GridCell>();

        for (int x = fromCell.x - maxDistance; x <= fromCell.x + maxDistance; x++) {
            for (int y = fromCell.y - maxDistance; y <= fromCell.y + maxDistance; y++) {
                if (!IsValidPosition(x, y)) continue;

                GridCell targetCell = GetGridCell(x, y);
                if (targetCell == null || targetCell.Equals(fromCell)) continue;

                int distance = fromCell.GetManhattanDistance(targetCell);
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
        if (IsValidPosition(x, y)) {
            return grid[x, y];
        }
        return null;
    }
}