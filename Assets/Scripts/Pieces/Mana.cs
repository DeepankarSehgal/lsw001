using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : MonsPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref MonsPiece[,] board, int tileCount)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        int direction = (team == 0) ? 1 : -1;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                int newX = currentX + dx * direction;
                int newY = currentY + dy * direction;

                 if(newX == 5 && newY == 5)
                    continue;

                if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && board[newX, newY] == null)
                {
                    r.Add(new Vector2Int(newX, newY));
                }
            }
        }

        return r;

    }
}
