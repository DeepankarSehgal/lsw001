using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonsPiece
{
    int tileDistance = 1;
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
        if (isCarryingBomb)
        {
            tileDistance = 3;
        }
        else
        {
            tileDistance = 2;
        }
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

                if (newX == 5 && newY == 5)
                    continue;

                // Check if the new position is within bounds and empty
                if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && (board[newX, newY] == null || CanCollectBomb(ref board,newX,newY)))
                {
                    r.Add(new Vector2Int(newX, newY));
                }
            }
        }


       


        Vector2Int[] movements = new Vector2Int[4];
        movements[0] = new Vector2Int(currentX, currentY + direction * 2);
        movements[1] = new Vector2Int(currentX, currentY - direction * 2);
        movements[2] = new Vector2Int(currentX + direction * 2, currentY);
        movements[3] = new Vector2Int(currentX - direction * 2, currentY);

        for(int i = 0 ; i < 4; i++)
        {
            //if( 0 <= movements[i].y && movements[i].y <= tileCount && 
        }

        //if(0 <= currentY + direction * 2 && currentY + direction * 2 < tileCount && board[currentX,c])



        if (0 <= currentY + direction * tileDistance && currentY + direction * tileDistance < tileCount && board[currentX, currentY + direction * tileDistance] != null && board[currentX, currentY + direction * tileDistance].team != team && board[currentX, currentY + direction * tileDistance].monsPieceType != MonsPieceType.mana && !checkAngel(ref board, currentX, currentY + direction * tileDistance))
        {
            r.Add(new Vector2Int(currentX, currentY + direction * tileDistance));
        }

        if (0 <= currentX + direction * tileDistance && currentX + direction * tileDistance < tileCount && board[currentX + direction * tileDistance, currentY] != null && board[currentX + direction * tileDistance, currentY].team != team && board[currentX + direction * tileDistance, currentY].monsPieceType != MonsPieceType.mana && !checkAngel(ref board, currentX + direction * tileDistance, currentY))
        {
            r.Add(new Vector2Int(currentX + direction * tileDistance, currentY));
        }

        if (0 <= currentX - direction * tileDistance && currentX - direction * tileDistance < tileCount && board[currentX - direction * tileDistance, currentY] != null && board[currentX - direction * tileDistance, currentY].team != team && board[currentX - direction * tileDistance, currentY].monsPieceType != MonsPieceType.mana && !checkAngel(ref board, currentX - direction * tileDistance, currentY))
        {
            r.Add(new Vector2Int(currentX - direction * tileDistance, currentY));
        }

        if (0 <= currentY - direction * tileDistance && currentY - direction * tileDistance < tileCount && board[currentX, currentY - direction * tileDistance] != null && board[currentX, currentY - direction * tileDistance].team != team && board[currentX, currentY - direction * tileDistance].monsPieceType != MonsPieceType.mana && !checkAngel(ref board, currentX, currentY - direction * tileDistance))
        {
            r.Add(new Vector2Int(currentX, currentY - direction * tileDistance));
        }


        return r;


       


    }


    public bool checkAngel(ref MonsPiece[,] board, int x,int y)
    {
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int newX = x + i;
                int newY = y + j;

                if (0 <= newX && newX < 11 && 0 <= newY && newY < 11  && board[newX, newY] != null && board[newX, newY].monsPieceType == MonsPieceType.angel)
                    return true;
            }
        }

        return false;
    }



    //public override void HighlightValidMovePositions(MonsPiece monsPiece)
    //{
    //    Debug.Log("demon highlught called");
    //    base.HighlightValidMovePositions(monsPiece);

    //    List<Vector2> validMovePositions = GetValidMovePositions(monsPiece);

    //    foreach (Vector2 position in validMovePositions)
    //    {
    //        Debug.Log(position);
    //        // Check if there's an opponent piece exactly 2 spaces away horizontally or vertically
    //        if (IsOpponentPieceAtDistance(position, 2))
    //        {
    //            Debug.Log("1");
    //            HighlightCell(position);
    //        }
    //    }
    //}

    //private bool IsOpponentPieceAtDistance(Vector2 position, int distance)
    //{
    //    // Check if there's an opponent piece at the specified distance horizontally or vertically
    //    int currentX = (int)position.x;
    //    int currentY = (int)position.y;

    //    // Check horizontally
    //    for (int i = 1; i <= distance; i++)
    //    {
    //        int x = currentX + i;
    //        if (x < 11 && Board.instance.cells[x, currentY] != null && Board.instance.monsPiece[x, currentY].team != team)
    //            return true;

    //        x = currentX - i;
    //        if (x >= 0 && Board.instance.monsPiece[x, currentY] != null && Board.instance.monsPiece[x, currentY].team != team)
    //            return true;
    //    }

    //    // Check vertically
    //    for (int i = 1; i <= distance; i++)
    //    {
    //        int y = currentY + i;
    //        if (y < 11 && Board.instance.monsPiece[currentX, y] != null && Board.instance.monsPiece[currentX, y].team != team)
    //            return true;

    //        y = currentY - i;
    //        if (y >= 0 && Board.instance.monsPiece[currentX, y] != null && Board.instance.monsPiece[currentX, y].team != team)
    //            return true;
    //    }

    //    return false;
    //}
}
