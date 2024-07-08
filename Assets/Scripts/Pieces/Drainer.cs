using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drainer : MonsPiece
{

    
    int tileDistance = 1;
    public bool CanCollectBomb(ref MonsPiece[,] board, int x, int y)
    {

        if (board[x, y].monsPieceDataType.monsPieceType == MonsPieceType.bombOrPortion)
        {
            return true;
        }

        return false;
    }
    public override List<Vector2Int> GetAvailableMoves(ref MonsPiece[,] board, int tileCount)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        int direction = (monsPieceDataType.team == 0) ? 1 : -1;

        if (monsPieceDataType.isCarryingBomb)
        {
            tileDistance = 2;
        }
        else
        {
            tileDistance = 1;
        }



        if (monsPieceDataType.isCarryingBomb)
        {
            tileDistance = 1;
            for (int i = 0; i < 2; i++)
            {

                for (int dx = -tileDistance; dx <= tileDistance; dx++)
                {
                    for (int dy = -tileDistance; dy <= tileDistance; dy++)
                    {
                        // Skip the case where both dx and dy are 0 (no movement)
                        if (dx == 0 && dy == 0)
                            continue;

                        int newX = monsPieceDataType.currentX + dx * direction;
                        int newY = monsPieceDataType.currentY + dy * direction;


                        bool isNotCarryingAnything = (!monsPieceDataType.isCarryingMana && !monsPieceDataType.isCarryingSuperMana && !monsPieceDataType.isCarryingOppMana);
                        // Check if the new position is within bounds and empty
                        if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && (board[newX, newY] == null
                                                                                                || isNotCarryingAnything && board[newX, newY].monsPieceDataType.monsPieceType == MonsPieceType.mana
                                                                                                || isNotCarryingAnything && board[newX, newY].monsPieceDataType.monsPieceType == MonsPieceType.supermana
                                                                                                || CanCollectBomb(ref board, newX, newY)))
                        {

                            r.Add(new Vector2Int(newX, newY));
                        }
                    }
                }
                tileDistance++;
            }
            }
        else
        {
            for (int dx = -tileDistance; dx <= tileDistance; dx++)
            {
                for (int dy = -tileDistance; dy <= tileDistance; dy++)
                {
                    // Skip the case where both dx and dy are 0 (no movement)
                    if (dx == 0 && dy == 0)
                        continue;

                    int newX = monsPieceDataType.currentX + dx * direction;
                    int newY = monsPieceDataType.currentY + dy * direction;


                    bool isNotCarryingAnything = (!monsPieceDataType.isCarryingMana && !monsPieceDataType.isCarryingSuperMana && !monsPieceDataType.isCarryingOppMana);
                    // Check if the new position is within bounds and empty
                    if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && (board[newX, newY] == null
                                                                                            || isNotCarryingAnything && board[newX, newY].monsPieceDataType.monsPieceType == MonsPieceType.mana
                                                                                            || isNotCarryingAnything && board[newX, newY].monsPieceDataType.monsPieceType == MonsPieceType.supermana
                                                                                            || CanCollectBomb(ref board, newX, newY)))
                    {

                        r.Add(new Vector2Int(newX, newY));
                    }
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
