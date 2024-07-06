using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : MonsPiece
{
    int tileDistance=2;
    public bool CanCollectBomb(ref MonsPiece[,] board, int x, int y)
    {

        if (board[x, y].monsPieceDataType.monsPieceType == MonsPieceType.bombOrPortion)
        {
            return true;
        }

        return false;
    }
    List<Vector2Int> availableMoves = new List<Vector2Int>();
    public override List<Vector2Int> GetAvailableMoves(ref MonsPiece[,] board, int tileCount)
    {
        availableMoves = new List<Vector2Int>();
        int direction = (monsPieceDataType.team == 0) ? 1 : -1;
        bool isSpiritInResetPosition = (monsPieceDataType.currentX == monsPieceDataType.resetPos.x && monsPieceDataType.currentY == monsPieceDataType.resetPos.y);

        if (monsPieceDataType.isCarryingBomb)
        {
            tileDistance = 3;
        }
        else
        {
            tileDistance = 2;
        }
        // Check if Spirit is in reset position
        if (!isSpiritInResetPosition && !monsPieceDataType.mySpecialAbilityUsed)
        {
            // Check for nearby pieces within 2 spaces
            for (int dx = -tileDistance; dx <= tileDistance; dx++)
            {
                for (int dy = -tileDistance; dy <= tileDistance; dy++)
                {
                    // Skip the current position
                    if (dx == 0 && dy == 0)
                        continue;

                    int newX = monsPieceDataType.currentX + dx;
                    int newY = monsPieceDataType.currentY + dy;

                    // Check if the new position is within the bounds of the board and if there's a piece present
                    print("spirit tile count: " + tileCount + "new x: " + newX + " new y: "+ newY);
                    if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && board[newX, newY] != null && board[newX, newY].monsPieceDataType.monsPieceType!=MonsPieceType.bombOrPortion /*&& Mathf.Abs(newX)>1 && Mathf.Abs(newY)>1*/)
                    {
                        
                        availableMoves.Add(new Vector2Int(newX, newY));
                    }
                }
            }

            //if(availableMoves!=null)
            //            return availableMoves; // Return the position of the nearby piece

            // If no nearby piece found, show normal movement
            ShowNormalMoment(direction,tileCount, ref board);

        }
        else
        {
            ShowNormalMoment(direction,tileCount,ref board);
     
        }

        return availableMoves;
    }

    private void ShowNormalMoment(int direction ,int tileCount, ref MonsPiece[,] board)
    {

        // Spirit is in reset position, show normal movement
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                // Skip the current position
                if (dx == 0 && dy == 0)
                    continue;

                int newX = monsPieceDataType.currentX + dx * direction;
                int newY = monsPieceDataType.currentY + dy * direction;

                // Check if the new position is within the bounds of the board
                if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && (board[newX, newY] == null || (board[newX, newY]!=null && board[newX, newY].monsPieceDataType.monsPieceType == MonsPieceType.bombOrPortion)))
                {
                    availableMoves.Add(new Vector2Int(newX, newY));
                }
            }
        }
    }
}
