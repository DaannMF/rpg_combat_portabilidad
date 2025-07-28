using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding {
    public static List<GridCell> FindPath(GridSystem gridSystem, GridCell start, GridCell target, int maxDistance) {
        if (start == null || target == null || start.Equals(target))
            return null;

        int directDistance = start.GetManhattanDistance(target);
        if (directDistance > maxDistance)
            return null;


        if (gridSystem.IsPositionOccupied(target))
            return null;

        Queue<GridCell> queue = new Queue<GridCell>();
        Dictionary<GridCell, GridCell> cameFrom = new Dictionary<GridCell, GridCell>();
        Dictionary<GridCell, int> distances = new Dictionary<GridCell, int>();

        queue.Enqueue(start);
        cameFrom[start] = null;
        distances[start] = 0;

        while (queue.Count > 0) {
            GridCell current = queue.Dequeue();

            if (current.Equals(target))
                return ReconstructPath(cameFrom, start, target);

            for (int dx = -1; dx <= 1; dx++) {
                for (int dy = -1; dy <= 1; dy++) {
                    if (dx == 0 && dy == 0) continue;

                    int newX = current.x + dx;
                    int newY = current.y + dy;

                    GridCell neighbor = gridSystem.GetGridCell(newX, newY);
                    if (neighbor == null || !gridSystem.IsValidPosition(neighbor))
                        continue;


                    if (gridSystem.IsPositionOccupied(neighbor) && !neighbor.Equals(target))
                        continue;

                    bool isDiagonal = (dx != 0 && dy != 0);
                    int movementCost = isDiagonal ? 2 : 1;
                    int newDistance = distances[current] + movementCost;

                    if (distances.ContainsKey(neighbor) && distances[neighbor] <= newDistance)
                        continue;


                    if (newDistance > maxDistance)
                        continue;


                    distances[neighbor] = newDistance;
                    cameFrom[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return null;
    }

    private static List<GridCell> ReconstructPath(Dictionary<GridCell, GridCell> cameFrom, GridCell start, GridCell target) {
        List<GridCell> path = new List<GridCell>();
        GridCell current = target;

        while (current != null && !current.Equals(start)) {
            path.Add(current);
            cameFrom.TryGetValue(current, out current);
        }

        path.Reverse();
        return path;
    }

    public static List<GridCell> GetValidMovePositions(GridSystem gridSystem, GridCell fromCell, int maxDistance) {
        List<GridCell> validPositions = new List<GridCell>();

        for (int x = fromCell.x - maxDistance; x <= fromCell.x + maxDistance; x++) {
            for (int y = fromCell.y - maxDistance; y <= fromCell.y + maxDistance; y++) {
                if (!gridSystem.IsValidPosition(x, y)) continue;

                GridCell targetCell = gridSystem.GetGridCell(x, y);
                if (targetCell == null || targetCell.Equals(fromCell)) continue;

                int dx = Mathf.Abs(targetCell.x - fromCell.x);
                int dy = Mathf.Abs(targetCell.y - fromCell.y);
                int minCost = Mathf.Max(dx, dy) + Mathf.Min(dx, dy);

                if (minCost <= maxDistance) {
                    List<GridCell> path = FindPath(gridSystem, fromCell, targetCell, maxDistance);
                    if (path != null && path.Count > 0)
                        validPositions.Add(targetCell);
                }
            }
        }

        return validPositions;
    }

    public static bool HasValidPath(GridSystem gridSystem, GridCell start, GridCell target, int maxDistance) {
        List<GridCell> path = FindPath(gridSystem, start, target, maxDistance);
        return path != null && path.Count > 0;
    }
}