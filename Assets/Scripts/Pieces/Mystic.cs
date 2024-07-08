using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mystic : MonsPiece
{
   
    int tileDistance = 2;
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
            tileDistance = 3;
        }
        else
        {
            tileDistance = 2;
        }

        //Calculating the movable position in 1 tile space
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                // Skip the case where both dx and dy are 0 (no movement)
                if (dx == 0 && dy == 0)
                    continue;

                int newX = monsPieceDataType.currentX + dx * direction;
                int newY = monsPieceDataType.currentY + dy * direction;

                if (newX == 5 && newY == 5)//to avoid superMana
                    continue;

                // Check if the new position is within bounds and empty
                if(0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount)
                print("Mystic check board place " + board[newX, newY]);
                if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && (board[newX, newY] == null || CanCollectBomb(ref board, newX, newY)))
                {
                    r.Add(new Vector2Int(newX, newY));
                    print("Available moves for mystic: " + new Vector2Int(newX, newY));
                }
            }
        }

        if (monsPieceDataType.isCarryingBomb)
        {
            tileDistance = 2;
            for (int i = 0; i < 2; i++)
            {

                if (0 <= monsPieceDataType.currentX + direction * tileDistance && monsPieceDataType.currentX + direction * tileDistance < tileCount
          && 0 <= monsPieceDataType.currentY + direction * tileDistance && monsPieceDataType.currentY + direction * tileDistance < tileCount
          && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance] != null
          && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team
          && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
          && !checkAngel(ref board, monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance)
          )
                {
                    if (board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                        r.Add(new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance));
                    print("Available moves for mystic: " + new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance));//here right diagonal gap

                }

                //2 tiles gap logic
                if (0 <= monsPieceDataType.currentX + direction * tileDistance && monsPieceDataType.currentX + direction * tileDistance < tileCount
                    && 0 <= monsPieceDataType.currentY - direction * tileDistance && monsPieceDataType.currentY - direction * tileDistance < tileCount
                    && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance] != null
                    && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team
                    && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
                     && !checkAngel(ref board, monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance)
                     )
                {
                    if (board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                        r.Add(new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance));
                    print("Available moves for mystic: " + new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance));
                }


                if (0 <= monsPieceDataType.currentX - direction * tileDistance && monsPieceDataType.currentX - direction * tileDistance < tileCount
                    && 0 <= monsPieceDataType.currentY + direction * tileDistance && monsPieceDataType.currentY + direction * tileDistance < tileCount
                    && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance] != null
                    && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team
                    && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
                     && !checkAngel(ref board, monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance)
                     )
                {
                    if (board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                        r.Add(new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance));
                    print("Available moves for mystic: " + new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance));//bottom right diagonal tile gap/ top left diagonal

                }


                if (0 <= monsPieceDataType.currentX - direction * tileDistance && monsPieceDataType.currentX - direction * tileDistance < tileCount
                    && 0 <= monsPieceDataType.currentY - direction * tileDistance && monsPieceDataType.currentY - direction * tileDistance < tileCount
                    && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance] != null
                    && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team
                    && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
                    && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
                     && !checkAngel(ref board, monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance)
                     )
                {
                    if (board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                        r.Add(new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance));
                    print("Available moves for mystic: " + new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance) + "check angel " + checkAngel(ref board, monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance));

                }

                tileDistance++;
            }
        }
        else
        {
            if (0 <= monsPieceDataType.currentX + direction * tileDistance && monsPieceDataType.currentX + direction * tileDistance < tileCount
     && 0 <= monsPieceDataType.currentY + direction * tileDistance && monsPieceDataType.currentY + direction * tileDistance < tileCount
     && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance] != null
     && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team
     && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
     && !checkAngel(ref board, monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance)
     )
            {
                if (board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                    r.Add(new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance));
                print("Available moves for mystic: " + new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance));//here right diagonal gap

            }

            //2 tiles gap logic
            if (0 <= monsPieceDataType.currentX + direction * tileDistance && monsPieceDataType.currentX + direction * tileDistance < tileCount
                && 0 <= monsPieceDataType.currentY - direction * tileDistance && monsPieceDataType.currentY - direction * tileDistance < tileCount
                && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance] != null
                && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team
                && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
                 && !checkAngel(ref board, monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance)
                 )
            {
                if (board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                    r.Add(new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance));
                print("Available moves for mystic: " + new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance));
            }


            if (0 <= monsPieceDataType.currentX - direction * tileDistance && monsPieceDataType.currentX - direction * tileDistance < tileCount
                && 0 <= monsPieceDataType.currentY + direction * tileDistance && monsPieceDataType.currentY + direction * tileDistance < tileCount
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance] != null
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
                 && !checkAngel(ref board, monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance)
                 )
            {
                if (board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                    r.Add(new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance));
                print("Available moves for mystic: " + new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance));//bottom right diagonal tile gap/ top left diagonal

            }


            if (0 <= monsPieceDataType.currentX - direction * tileDistance && monsPieceDataType.currentX - direction * tileDistance < tileCount
                && 0 <= monsPieceDataType.currentY - direction * tileDistance && monsPieceDataType.currentY - direction * tileDistance < tileCount
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance] != null
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
                 && !checkAngel(ref board, monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance)
                 )
            {
                if (board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                    r.Add(new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance));
                print("Available moves for mystic: " + new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance) + "check angel " + checkAngel(ref board, monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance));

            }
        }
         
        return r;
    }



    public bool checkAngel(ref MonsPiece[,] board, int x, int y)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int newX = x + i;
                int newY = y + j;

                if (0 <= newX && newX < 11 && 0 <= newY && newY < 11 && board[newX, newY] != null && board[newX, newY].monsPieceDataType.monsPieceType == MonsPieceType.angel && board[newX, newY].monsPieceDataType.team != monsPieceDataType.team)
                    return true;
            }
        }

        return false;
    }


    //public override void HighlightValidMovePositions(MonsPiece monsPiece)
    //{
    //    Debug.Log("Mystic highlight called");
    //    base.HighlightValidMovePositions(monsPiece);

    //    for (int i = 1; i <= 2; i++)
    //    {
    //        // Check diagonally
    //        CheckOpponentAtDistance(currentX + i, currentY + i);
    //        CheckOpponentAtDistance(currentX - i, currentY + i);
    //        CheckOpponentAtDistance(currentX + i, currentY - i);
    //        CheckOpponentAtDistance(currentX - i, currentY - i);
    //    }
    //}

    //private void CheckOpponentAtDistance(int targetX, int targetY)
    //{
    //    // Check if there's an opponent piece at the specified position
    //    if (targetX >= 0 && targetX < 11 && targetY >= 0 && targetY < 11)
    //    {
    //        MonsPiece targetPiece = Board.instance.monsPiece[targetX, targetY];
    //        if (targetPiece != null && targetPiece.team != team)
    //        {
    //            HighlightCell(new Vector2(targetX, targetY));
    //        }
    //    }
    //}
}
