using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : MonsPiece
{

    public bool CanCollectBomb(ref MonsPiece[,] board, int x, int y)
    {

        if (board[x, y].monsPieceType == MonsPieceType.bombOrPortion)
        {
            return true;
        }

        return false;
    }

    public override List<Vector2Int> GetAvailableMoves(ref MonsPiece[,] board, int tileCount)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();
        int direction = (team == 0) ? 1 : -1;
        bool isSpiritInResetPosition = (currentX == resetPos.x && currentY == resetPos.y);

        // Check if Spirit is in reset position
        if (!isSpiritInResetPosition)
        {
            // Check for nearby pieces within 2 spaces
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    // Skip the current position
                    if (dx == 0 && dy == 0)
                        continue;

                    int newX = currentX + dx;
                    int newY = currentY + dy;

                    // Check if the new position is within the bounds of the board and if there's a piece present
                    if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && board[newX, newY] != null)
                    {
                        availableMoves.Add(new Vector2Int(newX, newY));
                    }
                }
            }

            if(availableMoves!=null)
                        return availableMoves; // Return the position of the nearby piece

            // If no nearby piece found, show normal movement
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    // Skip the current position
                    if (dx == 0 && dy == 0)
                        continue;

                    int newX = currentX + dx * direction;
                    int newY = currentY + dy * direction;

                    // Check if the new position is within the bounds of the board
                    if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && (board[newX, newY] == null || CanCollectBomb(ref board, newX, newY)))
                    {
                        availableMoves.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }
        else
        {
            // Spirit is in reset position, show normal movement
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    // Skip the current position
                    if (dx == 0 && dy == 0)
                        continue;

                    int newX = currentX + dx * direction;
                    int newY = currentY + dy * direction;

                    // Check if the new position is within the bounds of the board
                    if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && board[newX, newY] == null)
                    {
                        availableMoves.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }

        return availableMoves;
    }

}
