using UnityEngine;
using System.Collections.Generic;

public interface Computer
{
    (int, int) Move(int[,] grid);
}

public class RandomComputer : Computer
{
    public (int, int) Move(int[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int x = Random.Range(0, width);
        int y = Random.Range(0, height);

        while (grid[x, y] != 0)
        {
            x = Random.Range(0, width);
            y = Random.Range(0, height);
        }

        return (x, y);
    }
}

public class SmartComputer : Computer
{
    public (int, int) Move(int[,] grid)
    {
        int n = grid.GetLength(0);

        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {
                if (grid[x, y] == 0)
                {
                    grid[x, y] = 2;
                    if (IsWinningMove(grid, 2))
                    {
                        grid[x, y] = 0;
                        return (x, y);
                    }
                    grid[x, y] = 0;
                }
            }
        }

        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {
                if (grid[x, y] == 0)
                {
                    grid[x, y] = 1;
                    if (IsWinningMove(grid, 1))
                    {
                        grid[x, y] = 0;
                        return (x, y);
                    }
                    grid[x, y] = 0;
                }
            }
        }

        List<(int, int)> goodMoves = new List<(int, int)>();

        for (int x = 1; x < n - 1; x++)
        {
            for (int y = 1; y < n - 1; y++)
            {
                if (grid[x, y] != 0)
                    continue;

                if (HasNeighbor(grid, x, y))
                    goodMoves.Add((x, y));
            }
        }

        if (goodMoves.Count > 0)
        {
            return goodMoves[Random.Range(0, goodMoves.Count)];
        }

        int rx, ry;
        do
        {
            rx = Random.Range(0, n);
            ry = Random.Range(0, n);
        }
        while (grid[rx, ry] != 0);

        return (rx, ry);
    }

    private bool HasNeighbor(int[,] grid, int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                if (grid[x + dx, y + dy] != 0)
                    return true;
            }
        }

        return false;
    }

    private bool IsWinningMove(int[,] grid, int player)
    {
        int n = grid.GetLength(0);

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (grid[i, j] != player)
                    continue;

                if (CheckDirection(grid, i, j, 1, 0, player)) return true;
                if (CheckDirection(grid, i, j, 0, 1, player)) return true;
                if (CheckDirection(grid, i, j, 1, 1, player)) return true;
                if (CheckDirection(grid, i, j, 1, -1, player)) return true;
            }
        }

        return false;
    }

    private bool CheckDirection(int[,] grid, int x, int y, int dx, int dy, int player)
    {
        int n = grid.GetLength(0);
        int count = 0;

        for (int k = 0; k < 5; k++)
        {
            int nx = x + dx * k;
            int ny = y + dy * k;

            if (nx < 0 || ny < 0 || nx >= n || ny >= n)
                return false;

            if (grid[nx, ny] == player)
                count++;
            else
                return false;
        }

        return count == 5;
    }
}