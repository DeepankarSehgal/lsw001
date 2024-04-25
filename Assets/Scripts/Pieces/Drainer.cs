using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drainer : MonsPiece
{

    public bool isCarryingMana = false;
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
        List<Vector2Int> r = new List<Vector2Int>();
        int direction = (team == 0) ? 1 : -1;


        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                // Skip the case where both dx and dy are 0 (no movement)
                if (dx == 0 && dy == 0)
                    continue;

                int newX = currentX + dx * direction;
                int newY = currentY + dy * direction;

               

                // Check if the new position is within bounds and empty
                if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount  && (board[newX, newY] == null 
                                                                                        || board[newX, newY].monsPieceType == MonsPieceType.mana 
                                                                                        || board[newX, newY].monsPieceType == MonsPieceType.supermana
                                                                                        || CanCollectBomb(ref board, newX, newY)))
                {
                    r.Add(new Vector2Int(newX, newY));
                }
            }
        }


        //if (0 <= currentX + direction && currentX + direction <= tileCount && 0 <= currentY + direction && currentY + direction <= tileCount && (board[currentX + direction, currentY + direction] == null || board[currentX + direction, currentY + direction].monsPieceType == MonsPieceType.mana || CanCollectBomb(ref board,currentX+direction,currentY+direction)))
        //{
        //    r.Add(new Vector2Int(currentX + direction, currentY + direction)); // Diagonal up-right
        //}
        //if (0 <= currentX + direction && currentX + direction <= tileCount && 0 <= currentY - direction && currentY - direction <= tileCount)
        //{   if(board[currentX + direction, currentY - direction] == null || board[currentX + direction, currentY - direction].monsPieceType == MonsPieceType.mana || CanCollectBomb(ref board, currentX + direction, currentY - direction))
        //    {
        //        r.Add(new Vector2Int(currentX + direction, currentY - direction));
        //    } // Diagonal down-right
        //}
        //if (0 <= currentX - direction && currentX - direction <= tileCount && 0 <= currentY + direction && currentY + direction <= tileCount && (board[currentX - direction, currentY + direction] == null || board[currentX - direction, currentY + direction].monsPieceType == MonsPieceType.mana || CanCollectBomb(ref board, currentX - direction, currentY + direction)))
        //{
        //    r.Add(new Vector2Int(currentX - direction, currentY + direction)); // Diagonal up-left
        //}
        //if (0 <= currentX - direction && currentX - direction <= tileCount && 0 <= currentY - direction && currentY - direction <= tileCount && (board[currentX - direction, currentY - direction] == null || board[currentX - direction, currentY - direction].monsPieceType == MonsPieceType.mana || CanCollectBomb(ref board, currentX - direction, currentY - direction)))
        //{
        //    r.Add(new Vector2Int(currentX - direction, currentY - direction)); // Diagonal down-left
        //}

        return r;
    }

    

}
